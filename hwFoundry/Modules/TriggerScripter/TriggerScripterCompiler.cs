using Aga.Controls.Tree;
using hwFoundry.Modules.TriggerScripter.Nodes;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace hwFoundry.Modules.TriggerScripter
{
    internal class TriggerScripterCompiler
    {
        private int varId = -1;
        readonly List<int> varIds = new();
        readonly Dictionary<int, string> varSources = new();
        readonly Dictionary<int, int> triggerVarLinks = new();
        readonly Dictionary<STNodeOption, int> linkedVars = new();

        private int AddTrigger(BaseNode node, XElement triggers, XElement triggerVars)
        {
            // Trigger has already been processed
            if (triggerVarLinks.ContainsKey(node.id))
                return triggerVarLinks[node.id];

            // Specify required attributes
            TriggerNode triggerNode = (TriggerNode)node;
            Dictionary<XName, object> attrs = new()
            {
                { "ID",                 triggerNode.id },
                { "Name",               triggerNode.Name },
                { "Active",             triggerNode.Active },
                { "EvaluateFrequency",  0 },
                { "EvalLimit",          0 },
                { "CommentOut",         false },
                { "ConditionalTrigger", triggerNode.Conditional },
                { "X",                  triggerNode.Location.X },
                { "Y",                  triggerNode.Location.Y },
                { "GroupID",            -1 },
                { "TemplateID",         -1}
            };

            // Build Trigger node XElement with attributes
            XElement trigger = new("Trigger");
            foreach (KeyValuePair<XName, object> entry in attrs)
                trigger.Add(new XAttribute(entry.Key, entry.Value));

            // Make XElements for different trigger types
            XElement triggerCnd = new("TriggerConditions"); trigger.Add(triggerCnd);
            XElement triggerT = new("TriggerEffectsOnTrue"); trigger.Add(triggerT);
            XElement triggerF = new("TriggerEffectsOnFalse"); trigger.Add(triggerF);

            // WIP
            //triggerVars.Add(new XComment("==Trigger: " + ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text + "=="));

            // Append new trigger and add in new reference variable
            triggers.Add(trigger);
            AddVar(varId, "Trigger", node.nodeTitle, false, node.id.ToString(), node.nodeTitle, triggerVars);
            triggerVarLinks.Add(node.id, varId++);

            // Write condition type
            XElement cndElement = new(triggerNode.Conditional ? "Or" : "And");
            triggerCnd.Add(cndElement);

            // Add all conditions
            if (triggerNode.Sockets["Conditions"].ConnectionCount > 0)
                foreach (STNodeOption socket in triggerNode.Sockets["Conditions"].GetConnectedSockets())
                    AddCondition((BaseNode)socket.Owner, triggerNode.nodeTitle, cndElement, triggerVars);

            // Add all effects on true
            if (triggerNode.Sockets["Call On True"].ConnectionCount > 0)
            {
                BaseNode last = (BaseNode)triggerNode.Sockets["Call On True"].GetConnectedSockets()[0].Owner;
                while (last != null)
                {
                    //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
                    if (!last.nodeTitle.Contains("Trigger"))
                        AddEffect(last, triggerNode.nodeTitle, triggerT, triggerVars, 0);
                    else
                    {
                        if (last.Sockets["Trigger"].ConnectionCount > 0)
                        {
                            AddTrigger((BaseNode)last.Sockets["Trigger"].GetConnectedSockets()[0].Owner, triggers, triggerVars);
                            AddEffect(last, triggerNode.nodeTitle, triggerT, triggerVars, triggerVarLinks[((BaseNode)last.Sockets["Trigger"].GetConnectedSockets()[0].Owner).id]);
                        }
                    }

                    last = last.Sockets["Call"].ConnectionCount > 0 ? (BaseNode)last.Sockets["Call"].GetConnectedSockets()[0].Owner : null;
                }
            }

            // Add all effects on false
            if (triggerNode.Sockets["Call On False"].ConnectionCount > 0)
            {
                BaseNode last = (BaseNode)triggerNode.Sockets["Call On False"].GetConnectedSockets()[0].Owner;
                while (last != null)
                {
                    //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
                    //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
                    if (!last.nodeTitle.Contains("Trigger"))
                    {
                        AddEffect(last, triggerNode.nodeTitle, triggerF, triggerVars, 0);
                    }
                    else
                    {
                        if (last.Sockets["Trigger"].ConnectionCount > 0)
                        {
                            AddTrigger((BaseNode)last.Sockets["Trigger"].GetConnectedSockets()[0].Owner, triggers, triggerVars);
                            AddEffect(last, triggerNode.nodeTitle, triggerF, triggerVars, triggerVarLinks[((BaseNode)last.Sockets["Trigger"].GetConnectedSockets()[0].Owner).id]);
                        }
                    }

                    last = last.Sockets["Call"].ConnectionCount > 0 ? (BaseNode)last.Sockets["Call"].GetConnectedSockets()[0].Owner : null;
                }
            }

            return triggerVarLinks[node.id];
        }

        private void AddEffect(BaseNode node, string triggerName, XElement triggerTF, XElement varX, int triggerValueOverride)
        {
            if (node.handleAs != "Effect") return;

            // Construct root node
            XElement eff = new("Effect");
            eff.Add(new XAttribute("ID", node.id));
            eff.Add(new XAttribute("Type", node.nodeTitle));
            eff.Add(new XAttribute("DBID", ((SerializedEffect)node.data).dbid));
            eff.Add(new XAttribute("Version", ((SerializedEffect)node.data).version));
            eff.Add(new XAttribute("CommentOut", false));

            // Filter out TriggerActivate/Deactivate
            if (!node.nodeTitle.Contains("Trigger"))
            {
                //inputs
                foreach (Input i in ((SerializedEffect)node.data).inputs)
                {
                    int id;
                    // If socket has something attached
                    if (node.Sockets[i.name].ConnectionCount > 0)
                    {
                        // If attached node is a variable node, try adding the variable
                        if (node.Sockets[i.name].GetConnectedSockets()[0].Owner.GetType() == typeof(VariableNode))
                        {
                            VariableNode var = (VariableNode)node.Sockets[i.name].GetConnectedSockets()[0].Owner;
                            AddVar(var.id, var.typeTitle, var.Name, false, var.Value,
                                $"{triggerName}::{node.nodeTitle}", varX);
                            id = var.id;
                        }
                        // Attached node is not a variable
                        else
                        {
                            // If there is already a linked var for this socket
                            if (linkedVars.ContainsKey(node.Sockets[i.name].GetConnectedSockets()[0]))
                                id = linkedVars[node.Sockets[i.name].GetConnectedSockets()[0]];

                            // There is not a linked var for this socket, add one
                            else
                            {
                                AddVar(varId, i.valueType, $"linked{i.valueType}", false, null,
                                    $"{triggerName}::{node.nodeTitle}", varX);
                                id = varId;
                                linkedVars.Add(node.Sockets[i.name].GetConnectedSockets()[0], varId);
                                varId++;
                            }
                        }
                    }
                    // Socket is not attached to anything, create null var
                    else
                    {
                        AddVar(varId, i.valueType, $"null{i.valueType}", true, null,
                            $"{triggerName}::{node.nodeTitle}", varX);
                        id = varId;
                        varId++;
                    }

                    XElement input = new("Input");
                    input.Add(new XAttribute("Name", node.Sockets[i.name].Text));
                    input.Add(new XAttribute("SigID", i.sigId));
                    input.Add(new XAttribute("Optional", i.optional));
                    input.Value = id.ToString();
                    eff.Add(input);
                }
                foreach (Output o in ((SerializedEffect)node.data).outputs)
                {
                    int id;
                    if (node.Sockets[o.name].ConnectionCount > 0)
                    {
                        if (node.Sockets[o.name].GetConnectedSockets()[0].Owner.GetType() == typeof(VariableNode))
                        {
                            VariableNode v = (VariableNode)node.Sockets[o.name].GetConnectedSockets()[0].Owner;
                            AddVar(v.id, v.typeTitle, v.Name, false, v.Value,
                                $"{triggerName}::{node.nodeTitle}", varX);
                            id = v.id;
                        }
                        else
                        {
                            if (linkedVars.ContainsKey(node.Sockets[o.name]))
                                id = linkedVars[node.Sockets[o.name]];
                            else
                            {
                                AddVar(varId, o.valueType, $"linked{o.valueType}", false, null,
                                    $"{triggerName}::{node.nodeTitle}", varX);
                                id = varId;
                                linkedVars.Add(node.Sockets[o.name], varId);
                                varId++;
                            }
                        }
                    }
                    else
                    {
                        AddVar(varId, o.valueType, $"null{o.valueType}", true, null,
                            $"{triggerName}::{node.nodeTitle}", varX);
                        id = varId;
                        varId++;
                    }

                    XElement output = new("Output");
                    output.Add(new XAttribute("Name", node.Sockets[o.name].Text));
                    output.Add(new XAttribute("SigID", o.sigId));
                    output.Add(new XAttribute("Optional", o.optional));
                    output.Value = id.ToString();
                    eff.Add(output);
                }
            }
            else
            {
                XElement output = new("Input");
                output.Add(new XAttribute("Name", "Trigger"));
                output.Add(new XAttribute("SigID", 1));
                output.Add(new XAttribute("Optional", false));
                output.Value = triggerValueOverride.ToString();
                eff.Add(output);
            }

            triggerTF.Add(eff);
        }

        private void AddCondition(BaseNode node, string triggerName, XElement triggerCnd, XElement varX)
        {
            // Double check that this is a ConditionNode
            if (node.handleAs != "Condition") return;

            // Specify required attributes
            ConditionNode cndNode = (ConditionNode)node;
            Dictionary<XName, object> attrs = new()
            {
                { "ID",                cndNode.id },
                { "Type",              cndNode.nodeTitle },
                { "DBID",              ((SerializedCondition)cndNode.data).dbid },
                { "Version",           ((SerializedCondition)cndNode.data).version },
                { "CommentOut",        false },
                { "Invert",            cndNode.Inverted },
                { "Async",             false },
                { "AsyncParameterKey", 0}
            };

            // Build Condition node XElement with attributes
            XElement cnd = new("Condition");
            foreach (KeyValuePair<XName, object> entry in attrs)
                cnd.Add(new XAttribute(entry.Key, entry.Value));

            // Inputs
            foreach (Input i in ((SerializedCondition)cndNode.data).inputs)
            {
                int id;

                // If any nodes are attached to this condition node
                if (cndNode.Sockets[i.name].ConnectionCount > 0)
                {
                    // If attached node is an existing VariableNode
                    if (cndNode.Sockets[i.name].GetConnectedSockets()[0].Owner.GetType() == typeof(VariableNode))
                    {
                        // Try to create a variable from this VariableNode
                        VariableNode var = (VariableNode)node.Sockets[i.name].GetConnectedSockets()[0].Owner;
                        AddVar(var.id, var.typeTitle, var.Name, false, var.Value,
                            $"{triggerName}::{node.nodeTitle}", varX);
                        id = var.id;
                    }

                    // If the socket is attached to something else
                    else
                    {
                        // Check if there is already an invisible linked variable node
                        if (linkedVars.ContainsKey(node.Sockets[i.name].GetConnectedSockets()[0]))
                            id = linkedVars[node.Sockets[i.name].GetConnectedSockets()[0]];
                        else
                        {
                            // Add linked variable
                            AddVar(varId, i.valueType, $"linked{i.valueType}", false, null,
                                $"{triggerName}::{node.nodeTitle}", varX);
                            id = varId;
                            linkedVars.Add(node.Sockets[i.name].GetConnectedSockets()[0], varId);
                            varId++;
                        }
                    }
                }

                // Socket isn't connected to anything. Add a null variable (required)
                else
                {
                    AddVar(varId, i.valueType, $"null{i.valueType}", true, null,
                        $"{triggerName}::{node.nodeTitle}", varX);
                    id = varId;
                    varId++;
                }

                // Construct Input element
                XElement input = new("Input");
                input.Add(new XAttribute("Name", node.Sockets[i.name].Text));
                input.Add(new XAttribute("SigID", i.sigId));
                input.Add(new XAttribute("Optional", i.optional));
                input.Value = id.ToString();
                cnd.Add(input);
            }

            // Outputs
            foreach (Output o in ((SerializedCondition)cndNode.data).outputs)
            {
                int id = -1;

                // If any nodes are attached to this condition node
                if (cndNode.Sockets[o.name].ConnectionCount > 0)
                {
                    // If attached node is an existing VariableNode
                    if (cndNode.Sockets[o.name].GetConnectedSockets()[0].Owner.GetType() == typeof(VariableNode))
                    {
                        // Try to create a variable from this VariableNode
                        VariableNode var = (VariableNode)node.Sockets[o.name].GetConnectedSockets()[0].Owner;
                        AddVar(var.id, var.typeTitle, var.Name, false, var.Name,
                            $"{triggerName}::{node.nodeTitle}", varX);
                        id = var.id;
                    }

                    // If the socket is attached to something else
                    else
                    {
                        // Check if there is already an invisible linked variable node
                        if (linkedVars.ContainsKey(node.Sockets[o.name]))
                            id = linkedVars[node.Sockets[o.name]];
                        else
                        {
                            // Add linked variable
                            AddVar(varId, o.valueType, $"linked{o.valueType}", false, null,
                                $"{triggerName}::{node.nodeTitle}", varX);
                            id = varId;
                            linkedVars.Add(node.Sockets[o.name], varId);
                            varId++;
                        }
                    }
                }

                // Socket isn't connected to anything. Add a null variable (required)
                else
                {
                    AddVar(varId, o.valueType, $"null{o.valueType}", true, null,
                        $"{triggerName}::{node.nodeTitle}", varX);
                    id = varId;
                    varId++;
                }

                // Construct Input element
                XElement output = new("Output");
                output.Add(new XAttribute("Name", node.Sockets[o.name].Text));
                output.Add(new XAttribute("SigID", o.sigId));
                output.Add(new XAttribute("Optional", o.optional));
                output.Value = id.ToString();
                cnd.Add(output);
            }

            triggerCnd.Add(cnd);
        }

        private void AddVar(int id, string type, string name, bool isNull, string value, string sourceName, XElement varX)
        {
            // If variable already exists, return
            if (varIds.Contains(id)) return;

            // Required Attributes
            Dictionary<XName, object> attrs = new()
            {
                { "ID",     id },
                { "Type",   type },
                { "Name",   name },
                { "IsNull", isNull }
            };

            // Add all data to new TriggerVar
            XElement var = new("TriggerVar");
            foreach (KeyValuePair<XName, object> entry in attrs)
                var.Add(new XAttribute(entry.Key, entry.Value));

            // Set node value and add everything to proper locations
            if (value != null && value != string.Empty) var.SetValue(value);
            varX.Add(var); varSources.Add(id, sourceName); varIds.Add(id);
        }

        public void Compile(List<BaseNode> nodes, int lastVarId, string outPath)
        {
            varId = lastVarId;

            // Generate document and root node
            XDocument doc = new();
            XElement triggerSystem = new("TriggerSystem");

            // Add in required attributes
            Dictionary<XName, object> attrs = new()
            {
                { XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance" },
                { XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema" },
                { "Name",                   Path.GetFileName(outPath) },
                { "Type",                   "TriggerScript" },
                { "NextTriggerVarID",       lastVarId },
                { "NextTriggerID",          9999 },
                {  "NextConditionID",       9999 },
                {  "NextEffectID",          9999 }
            };

            // Apply attributes and add triggersystem to root
            foreach (KeyValuePair<XName, object> entry in attrs)
                triggerSystem.Add(new XAttribute(entry.Key, entry.Value));
            doc.Add(triggerSystem);

            // Fixed child nodes
            XElement triggerGroups = new("TriggerGroups"); triggerSystem.Add(triggerGroups);
            XElement triggerVars = new("TriggerVars"); triggerSystem.Add(triggerVars);
            XElement triggers = new("Triggers"); triggerSystem.Add(triggers);

            // Compile from entry points (active triggers)
            foreach (BaseNode node in nodes)
                if (node.handleAs == "Trigger")
                    if (((TriggerNode)node).Active)
                        AddTrigger(node, triggers, triggerVars);

            // Re-order all triggerVars by ID
            triggerVars.ReplaceNodes(
                triggerVars.Elements().OrderBy(y => int.Parse(y.Attribute("ID").Value)));

            // Finally save the new .triggerscript
            doc.Save(outPath);
        }
    }
}
