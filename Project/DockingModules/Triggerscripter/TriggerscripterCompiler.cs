using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SMHEditor.DockingModules.Triggerscripter.TriggerscripterPage;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class TriggerscripterCompiler
    {
        List<int> varIds = new List<int>();
        Dictionary<int, int> triggerVarLinks = new Dictionary<int, int>();
        Dictionary<TriggerscripterSocket_Output, int> linkedVars = new Dictionary<TriggerscripterSocket_Output, int>();
        Dictionary<int, string> varSources = new Dictionary<int, string>();
        int varId = -1;
        void AddVar(int id, string type, string name, bool isNull, string value, string sourceName, XElement varX)
        {
            if (varIds.Contains(id))
            {
                if (varSources[id] != sourceName)
                {
                }
                return;
            }
            else
            {
                XElement v = new XElement("TriggerVar");
                v.Add(new XAttribute("ID", id));
                v.Add(new XAttribute("Type", type));
                v.Add(new XAttribute("Name", name));
                v.Add(new XAttribute("IsNull", isNull));
                if (value != null && value != "") v.SetValue(value);
                varX.Add(v);
                varSources.Add(id, sourceName);
                varIds.Add(id);
            }
        }
        void AddEffect(TriggerscripterNode n, string triggerName, XElement triggerTF, XElement varX, int triggerValueOverride)
        {
            if (n.handleAs != "Effect") return;

            XElement eff = new XElement("Effect");
            eff.Add(new XAttribute("ID", n.id));
            eff.Add(new XAttribute("Type", n.nodeTitle));
            eff.Add(new XAttribute("DBID", ((Effect)n.data).dbid));
            eff.Add(new XAttribute("Version", ((Effect)n.data).version));
            eff.Add(new XAttribute("CommentOut", false));

            if (!n.nodeTitle.Contains("Trigger"))
            {
                foreach (Input i in ((Effect)n.data).inputs)
                {
                    int id;
                    if (n.sockets[i.name].connectedSockets.Count > 0)
                    {
                        if (n.sockets[i.name].connectedSockets[0].node is TriggerscripterNode_Variable)
                        {
                            TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[i.name].connectedSockets[0].node;
                            AddVar(
                                v.id,
                                v.typeTitle,
                                v.nameProperty.tb.Text,
                                false,
                                v.valueProperty.tb.Text,
                                triggerName + "::" + n.nodeTitle,
                                varX);
                            id = v.id;
                        }
                        else
                        {
                            if(linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[i.name].connectedSockets[0]))
                            {
                                id = linkedVars[(TriggerscripterSocket_Output)n.sockets[i.name].connectedSockets[0]];
                            }
                            else
                            {
                                AddVar(
                                    varId,
                                    i.valueType,
                                    "linked" + i.valueType,
                                    false,
                                    null,
                                    triggerName + "::" + n.nodeTitle,
                                    varX);
                                id = varId;
                                linkedVars.Add((TriggerscripterSocket_Output)n.sockets[i.name].connectedSockets[0], varId);
                                varId++;
                            }
                        }
                    }
                    else
                    {
                        AddVar(
                            varId,
                            n.sockets[i.name].valueType,
                            "null" + n.sockets[i.name].valueType,
                            true,
                            null,
                            triggerName + "::" + n.nodeTitle,
                            varX);
                        id = varId;
                        varId++;
                    }
                    XElement input = new XElement("Input");
                    input.Add(new XAttribute("Name", n.sockets[i.name].text));
                    input.Add(new XAttribute("SigID", i.sigId));
                    input.Add(new XAttribute("Optional", i.optional));
                    input.Value = id.ToString();
                    eff.Add(input);
                }
                foreach (Output o in ((Effect)n.data).outputs)
                {
                    int id;
                    if (n.sockets[o.name].connectedSockets.Count > 0)
                    {
                        if (n.sockets[o.name].connectedSockets[0].node is TriggerscripterNode_Variable)
                        {
                            TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[o.name].connectedSockets[0].node;
                            AddVar(
                                v.id,
                                v.typeTitle,
                                v.nameProperty.tb.Text,
                                false,
                                v.valueProperty.tb.Text,
                                triggerName + "::" + n.nodeTitle,
                                varX);
                            id = v.id;
                        }
                        else
                        {
                            if(linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[o.name]))
                            {
                                id = linkedVars[(TriggerscripterSocket_Output)n.sockets[o.name]];
                            }
                            else
                            {
                                AddVar(
                                    varId,
                                    o.valueType,
                                    "linked" + o.valueType,
                                    false,
                                    null,
                                    triggerName + "::" + n.nodeTitle,
                                    varX);
                                id = varId;
                                linkedVars.Add((TriggerscripterSocket_Output)n.sockets[o.name], varId);
                                varId++;
                            }
                        }
                    }
                    else
                    {
                        AddVar(
                            varId,
                            o.valueType,
                            "null" + o.valueType,
                            true,
                            null,
                            triggerName + "::" + n.nodeTitle,
                            varX);
                        id = varId;
                        varId++;
                    }
                    XElement output = new XElement("Output");
                    output.Add(new XAttribute("Name", n.sockets[o.name].text));
                    output.Add(new XAttribute("SigID", o.sigId));
                    output.Add(new XAttribute("Optional", o.optional));
                    output.Value = id.ToString();
                    eff.Add(output);
                }
            }
            else
            {
                XElement output = new XElement("Input");
                output.Add(new XAttribute("Name", "Trigger"));
                output.Add(new XAttribute("SigID", 1));
                output.Add(new XAttribute("Optional", false));
                output.Value = triggerValueOverride.ToString();
                eff.Add(output);
            }

            triggerTF.Add(eff);
        }
        void AddCondition(TriggerscripterNode n, string triggerName, XElement triggerCnd, XElement varX)
        {
            if (n.handleAs != "Condition") return;

            XElement cnd = new XElement("Condition");
            cnd.Add(new XAttribute("ID", n.id));
            cnd.Add(new XAttribute("Type", n.nodeTitle));
            cnd.Add(new XAttribute("DBID", ((Condition)n.data).dbid));
            cnd.Add(new XAttribute("Version", ((Condition)n.data).version));
            cnd.Add(new XAttribute("CommentOut", false));
            cnd.Add(new XAttribute("Invert", ((TriggerscripterNode_Condition)n).invertedProperty.state));
            cnd.Add(new XAttribute("Async", false));
            cnd.Add(new XAttribute("AsyncParameterKey", 0));

            foreach (Input i in ((Condition)n.data).inputs)
            {
                int id;
                if (n.sockets[i.name].connectedSockets.Count > 0)
                {
                    if (n.sockets[i.name].connectedSockets[0].node is TriggerscripterNode_Variable)
                    {
                        TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[i.name].connectedSockets[0].node;
                        AddVar(
                        v.id,
                        v.typeTitle,
                        v.nameProperty.tb.Text,
                        false,
                        v.valueProperty.tb.Text,
                        triggerName + "::" + n.nodeTitle,
                        varX);
                        id = v.id;
                    }
                    else
                    {
                        if (linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[i.name].connectedSockets[0]))
                        {
                            id = linkedVars[(TriggerscripterSocket_Output)n.sockets[i.name].connectedSockets[0]];
                        }
                        else
                        {
                            AddVar(
                                varId,
                                i.valueType,
                                "linked" + i.valueType,
                                false,
                                null,
                                triggerName + "::" + n.nodeTitle,
                                varX);
                            id = varId;
                            linkedVars.Add((TriggerscripterSocket_Output)n.sockets[i.name].connectedSockets[0], varId);
                            varId++;
                        }
                    }
                }
                else
                {
                    AddVar(
                        varId,
                        n.sockets[i.name].valueType,
                        "null" + n.sockets[i.name].valueType,
                        true,
                        null,
                        triggerName + "::" + n.nodeTitle,
                        varX);
                    id = varId;
                    varId++;
                }
                XElement input = new XElement("Input");
                input.Add(new XAttribute("Name", n.sockets[i.name].text));
                input.Add(new XAttribute("SigID", i.sigId));
                input.Add(new XAttribute("Optional", i.optional));
                input.Value = id.ToString();
                cnd.Add(input);
            }
            foreach (Output o in ((Condition)n.data).outputs)
            {
                int id;
                if (n.sockets[o.name].connectedSockets.Count > 0)
                {
                    if (n.sockets[o.name].connectedSockets[0].node is TriggerscripterNode_Variable)
                    {
                        TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[o.name].connectedSockets[0].node;
                        AddVar(
                        v.id,
                        v.typeTitle,
                        v.nameProperty.tb.Text,
                        false,
                        v.valueProperty.tb.Text,
                        triggerName + "::" + n.nodeTitle,
                        varX);
                        id = v.id;
                    }
                    else
                    {
                        if (linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[o.name]))
                        {
                            id = linkedVars[(TriggerscripterSocket_Output)n.sockets[o.name]];
                        }
                        else
                        {
                            AddVar(
                                varId,
                                o.valueType,
                                "linked" + o.valueType,
                                false,
                                null,
                                triggerName + "::" + n.nodeTitle,
                                varX);
                            id = varId;
                            linkedVars.Add((TriggerscripterSocket_Output)n.sockets[o.name], varId);
                            varId++;

                        }
                    }
                }
                else
                {
                    AddVar(
                        varId,
                        n.sockets[o.name].valueType,
                        "null" + n.sockets[o.name].valueType,
                        true,
                        null,
                        triggerName + "::" + n.nodeTitle,
                        varX);
                    id = varId;
                    varId++;
                }
                XElement output = new XElement("Output");
                output.Add(new XAttribute("Name", n.sockets[o.name].text));
                output.Add(new XAttribute("SigID", o.sigId));
                output.Add(new XAttribute("Optional", o.optional));
                output.Value = id.ToString();
                cnd.Add(output);
            }

            triggerCnd.Add(cnd);
        }
        int AddTrigger(TriggerscripterNode n, XElement triggers, XElement triggerVars)
        {
            if (triggerVarLinks.ContainsKey(n.id))
            {
                return triggerVarLinks[n.id];
            }
            else
            {
                XElement trigger = new XElement("Trigger");
                trigger.Add(new XAttribute("ID", ((TriggerscripterNode_Trigger)n).id));
                trigger.Add(new XAttribute("Name", ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text));
                trigger.Add(new XAttribute("Active", ((TriggerscripterNode_Trigger)n).activeProperty.state));
                trigger.Add(new XAttribute("EvaluateFrequency", 0));
                trigger.Add(new XAttribute("EvalLimit", 0));
                trigger.Add(new XAttribute("CommentOut", false));
                trigger.Add(new XAttribute("ConditionalTrigger", ((TriggerscripterNode_Trigger)n).conditionalTriggerProperty.state));
                trigger.Add(new XAttribute("X", n.x));
                trigger.Add(new XAttribute("Y", n.y));
                trigger.Add(new XAttribute("GroupID", -1));
                trigger.Add(new XAttribute("TemplateID", -1));
                XElement triggerCnd = new XElement("TriggerConditions");
                XElement triggerT = new XElement("TriggerEffectsOnTrue");
                XElement triggerF = new XElement("TriggerEffectsOnFalse");
                trigger.Add(triggerCnd);
                trigger.Add(triggerT);
                trigger.Add(triggerF);

                TriggerscripterNode_Trigger t = (TriggerscripterNode_Trigger)n;

                //triggerVars.Add(new XComment("==Trigger: " + ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text + "=="));
                triggers.Add(trigger);
                AddVar(varId, "Trigger", n.nodeTitle, false, n.id.ToString(), n.nodeTitle, triggerVars);
                triggerVarLinks.Add(n.id, varId);
                varId++;

                XElement cndElement;
                if (t.conditionalTypeProperty.state == false)
                    cndElement = new XElement("And");
                else
                    cndElement = new XElement("Or");

                triggerCnd.Add(cndElement);
                
                if (t.sockets["Conditions"].connectedSockets.Count > 0)
                {
                    foreach (TriggerscripterSocket s in t.sockets["Conditions"].connectedSockets)
                    {
                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + s.node.nodeTitle + " (Condition)"));
                        AddCondition(s.node, t.nodeTitle, cndElement, triggerVars);
                    }
                }
                if (t.sockets["Call On True"].connectedSockets.Count > 0)
                {
                    TriggerscripterNode last = t.sockets["Call On True"].connectedSockets[0].node;
                    while (last != null)
                    {
                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
                        if (!last.nodeTitle.Contains("Trigger"))
                        {
                            AddEffect(last, t.nodeTitle, triggerT, triggerVars, 0);
                        }
                        else
                        {
                            if (last.sockets["Trigger"].connectedSockets.Count > 0)
                            {
                                AddTrigger(last.sockets["Trigger"].connectedSockets[0].node, triggers, triggerVars);
                                AddEffect(last, t.nodeTitle, triggerT, triggerVars, triggerVarLinks[last.sockets["Trigger"].connectedSockets[0].node.id]);
                            }
                        }

                        if (last.sockets["Call"].connectedSockets.Count > 0) last = last.sockets["Call"].connectedSockets[0].node;
                        else last = null;
                    }
                }
                if (t.sockets["Call On False"].connectedSockets.Count > 0)
                {
                    TriggerscripterNode last = t.sockets["Call On False"].connectedSockets[0].node;
                    while (last != null)
                    {
                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
                        if (!last.nodeTitle.Contains("Trigger"))
                        {
                            AddEffect(last, t.nodeTitle, triggerF, triggerVars, 0);
                        }
                        else
                        {
                            if (last.sockets["Trigger"].connectedSockets.Count > 0)
                            {
                                AddTrigger(last.sockets["Trigger"].connectedSockets[0].node, triggers, triggerVars);
                                AddEffect(last, t.nodeTitle, triggerF, triggerVars, triggerVarLinks[last.sockets["Trigger"].connectedSockets[0].node.id]);
                            }
                        }

                        if (last.sockets["Call"].connectedSockets.Count > 0) last = last.sockets["Call"].connectedSockets[0].node;
                        else last = null;
                    }
                }
                
                return triggerVarLinks[n.id];
            }
        }

        public void Compile(List<TriggerscripterNode> nodes, int lastVarId, string outPath)
        {
            varId = lastVarId;

            XDocument x = new XDocument();
            XElement triggerSystem = new XElement("TriggerSystem");
            triggerSystem.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
            triggerSystem.Add(new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));
            triggerSystem.Add(new XAttribute("Name", Path.GetFileName(outPath)));
            triggerSystem.Add(new XAttribute("Type", "TriggerScript"));
            triggerSystem.Add(new XAttribute("NextTriggerVarID", lastVarId));
            triggerSystem.Add(new XAttribute("NextTriggerID", 9999));
            triggerSystem.Add(new XAttribute("NextConditionID", 9999));
            triggerSystem.Add(new XAttribute("NextEffectID", 9999));
            x.Add(triggerSystem);

            XElement triggerGroups = new XElement("TriggerGroups");
            triggerSystem.Add(triggerGroups);

            XElement triggerVars = new XElement("TriggerVars");
            triggerSystem.Add(triggerVars);

            XElement triggers = new XElement("Triggers");
            triggerSystem.Add(triggers);


            foreach(TriggerscripterNode n in nodes)
            {
                if(n.handleAs == "Trigger")
                {
                    if(((TriggerscripterNode_Trigger)n).activeProperty.state)
                    {
                        AddTrigger(n, triggers, triggerVars);
                    }
                }
            }
            
            triggerVars.ReplaceNodes(
                triggerVars.Elements().OrderBy(y => int.Parse(y.Attribute("ID").Value)));

            x.Save(outPath);
        }
    }
}
