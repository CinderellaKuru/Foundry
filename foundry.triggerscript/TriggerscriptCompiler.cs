//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace Foundry.Data.Triggerscript
//{
//	//TODO: move this class into TriggerscriptEditorPage (maybe?) [no.].
//	public class TriggerscriptCompiler
//    {
//        List<int> varIds = new List<int>();
//        Dictionary<int, int> triggerVarLinks = new Dictionary<int, int>();
//        Dictionary<TriggerscripterSocket_Output, int> linkedVars = new Dictionary<TriggerscripterSocket_Output, int>();
//        Dictionary<int, string> varSources = new Dictionary<int, string>();

//        int  varId = -1;
//        void AddVar(int id, string type, string name, bool isNull, string value, string sourceName, XElement varX)
//        {
//            //if var already exists, return
//            if (varIds.Contains(id))
//            {
//                return;
//            }
//            //else create var node
//            else
//            {
//                XElement v = new XElement("TriggerVar");
//                v.Add(new XAttribute("ID", id));
//                v.Add(new XAttribute("Type", type));
//                v.Add(new XAttribute("Name", name));
//                v.Add(new XAttribute("IsNull", isNull));
//                if (value != null && value != "") v.SetValue(value);
//                varX.Add(v);
//                varSources.Add(id, sourceName);
//                varIds.Add(id);
//            }
//        }
//        void AddEffect(TriggerscripterNode n, string triggerName, XElement triggerTF, XElement varX, int triggerValueOverride)
//        {
//            if (n.HandleAs != "Effect") return;

//            XElement eff = new XElement("Effect");
//            eff.Add(new XAttribute("ID", n.Id));
//            eff.Add(new XAttribute("Type", n.Name));
//            eff.Add(new XAttribute("DBID", ((SerializedEffect)n.Data).dbid));
//            eff.Add(new XAttribute("Version", ((SerializedEffect)n.Data).version));
//            eff.Add(new XAttribute("CommentOut", false));

//            //filter out TriggerActivate/Deactivate
//            if (!n.Name.Contains("Trigger"))
//            {
//                //inputs
//                foreach (Input i in ((SerializedEffect)n.Data).inputs)
//                {
//                    int id;
//                    //if socket has something attached
//                    if (n.sockets[i.name].ConnectedSockets.Count > 0)
//                    {
//                        //if attached node is a variable node, try adding the variable
//                        if (n.sockets[i.name].ConnectedSockets[0].OwnerNode is TriggerscripterNode_Variable)
//                        {
//                            TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[i.name].ConnectedSockets[0].OwnerNode;
//                            AddVar(
//                                v.Id,
//                                v.Type,
//                                v.Name,
//                                false,
//                                v.Value,
//                                triggerName + "::" + n.Name,
//                                varX);
//                            id = v.Id;
//                        }
//                        //attached node is not a variable
//                        else
//                        {
//                            //if there is already a linked var for this socket
//                            if(linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[i.name].ConnectedSockets[0]))
//                            {
//                                id = linkedVars[(TriggerscripterSocket_Output)n.sockets[i.name].ConnectedSockets[0]];
//                            }
//                            //there is not a linked var for this socket, add one
//                            else
//                            {
//                                AddVar(
//                                    varId,
//                                    i.valueType,
//                                    "linked" + i.valueType,
//                                    false,
//                                    null,
//                                    triggerName + "::" + n.Name,
//                                    varX);
//                                id = varId;
//                                linkedVars.Add((TriggerscripterSocket_Output)n.sockets[i.name].ConnectedSockets[0], varId);
//                                varId++;
//                            }
//                        }
//                    }
//                    //socket is not attached to anything, create null var
//                    else
//                    {
//                        AddVar(
//                            varId,
//                            n.sockets[i.name].ValueType,
//                            "null" + n.sockets[i.name].ValueType,
//                            true,
//                            null,
//                            triggerName + "::" + n.Name,
//                            varX);
//                        id = varId;
//                        varId++;
//                    }
//                    XElement input = new XElement("Input");
//                    input.Add(new XAttribute("Name", n.sockets[i.name].Text));
//                    input.Add(new XAttribute("SigID", i.sigId));
//                    input.Add(new XAttribute("Optional", i.optional));
//                    input.Value = id.ToString();
//                    eff.Add(input);
//                }
//                foreach (Output o in ((SerializedEffect)n.Data).outputs)
//                {
//                    int id;
//                    if (n.sockets[o.name].ConnectedSockets.Count > 0)
//                    {
//                        if (n.sockets[o.name].ConnectedSockets[0].OwnerNode is TriggerscripterNode_Variable)
//                        {
//                            TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[o.name].ConnectedSockets[0].OwnerNode;
//                            AddVar(
//                                v.Id,
//                                v.Type,
//                                v.Name,
//                                false,
//                                v.Value,
//                                triggerName + "::" + n.Name,
//                                varX);
//                            id = v.Id;
//                        }
//                        else
//                        {
//                            if(linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[o.name]))
//                            {
//                                id = linkedVars[(TriggerscripterSocket_Output)n.sockets[o.name]];
//                            }
//                            else
//                            {
//                                AddVar(
//                                    varId,
//                                    o.valueType,
//                                    "linked" + o.valueType,
//                                    false,
//                                    null,
//                                    triggerName + "::" + n.Name,
//                                    varX);
//                                id = varId;
//                                linkedVars.Add((TriggerscripterSocket_Output)n.sockets[o.name], varId);
//                                varId++;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        AddVar(
//                            varId,
//                            o.valueType,
//                            "null" + o.valueType,
//                            true,
//                            null,
//                            triggerName + "::" + n.Name,
//                            varX);
//                        id = varId;
//                        varId++;
//                    }
//                    XElement output = new XElement("Output");
//                    output.Add(new XAttribute("Name", n.sockets[o.name].Text));
//                    output.Add(new XAttribute("SigID", o.sigId));
//                    output.Add(new XAttribute("Optional", o.optional));
//                    output.Value = id.ToString();
//                    eff.Add(output);
//                }
//            }
//            else
//            {
//                XElement output = new XElement("Input");
//                output.Add(new XAttribute("Name", "Trigger"));
//                output.Add(new XAttribute("SigID", 1));
//                output.Add(new XAttribute("Optional", false));
//                output.Value = triggerValueOverride.ToString();
//                eff.Add(output);
//            }

//            triggerTF.Add(eff);
//        }
//        void AddCondition(TriggerscripterNode n, string triggerName, XElement triggerCnd, XElement varX)
//        {
//            if (n.HandleAs != "Condition") return;

//            //root node
//            XElement cnd = new XElement("Condition");
//            cnd.Add(new XAttribute("ID", n.Id));
//            cnd.Add(new XAttribute("Type", n.Name));
//            cnd.Add(new XAttribute("DBID", ((SerializedCondition)n.Data).dbid));
//            cnd.Add(new XAttribute("Version", ((SerializedCondition)n.Data).version));
//            cnd.Add(new XAttribute("CommentOut", false));
//            cnd.Add(new XAttribute("Invert", ((TriggerscripterNode_Condition)n).Inverted));
//            cnd.Add(new XAttribute("Async", false));
//            cnd.Add(new XAttribute("AsyncParameterKey", 0));

//            //inputs
//            foreach (Input i in ((SerializedCondition)n.Data).inputs)
//            {
//                int id;
//                //if there are any nodes attached to this condition node
//                if (n.sockets[i.name].ConnectedSockets.Count > 0)
//                {
//                    //if attached node is an existing variable node
//                    if (n.sockets[i.name].ConnectedSockets[0].OwnerNode is TriggerscripterNode_Variable)
//                    {
//                        //try to create variable from this var node
//                        TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[i.name].ConnectedSockets[0].OwnerNode;
//                        AddVar(
//                        v.Id,
//                        v.Type,
//                        v.Name,
//                        false,
//                        v.Value,
//                        triggerName + "::" + n.Name,
//                        varX);
//                        id = v.Id;
//                    }
//                    //if the socket is something else
//                    else
//                    {
//                        //check if there is already an invisible linked variable node
//                        if (linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[i.name].ConnectedSockets[0]))
//                        {
//                            id = linkedVars[(TriggerscripterSocket_Output)n.sockets[i.name].ConnectedSockets[0]];
//                        }
//                        else
//                        {
//                            //add linked var
//                            AddVar(
//                                varId,
//                                i.valueType,
//                                "linked" + i.valueType,
//                                false,
//                                null,
//                                triggerName + "::" + n.Name,
//                                varX);
//                            id = varId;
//                            linkedVars.Add((TriggerscripterSocket_Output)n.sockets[i.name].ConnectedSockets[0], varId);
//                            varId++;
//                        }
//                    }
//                }
//                //socket is not attached to anything. add a null variable
//                else
//                {
//                    AddVar(
//                        varId,
//                        n.sockets[i.name].ValueType,
//                        "null" + n.sockets[i.name].ValueType,
//                        true,
//                        null,
//                        triggerName + "::" + n.Name,
//                        varX);
//                    id = varId;
//                    varId++;
//                }
//                XElement input = new XElement("Input");
//                input.Add(new XAttribute("Name", n.sockets[i.name].Text));
//                input.Add(new XAttribute("SigID", i.sigId));
//                input.Add(new XAttribute("Optional", i.optional));
//                input.Value = id.ToString();
//                cnd.Add(input);
//            }
//            //outputs
//            foreach (Output o in ((SerializedCondition)n.Data).outputs)
//            {
//                int id;
//                //if the socket has something attached
//                if (n.sockets[o.name].ConnectedSockets.Count > 0)
//                {
//                    //if the socket is an existing variable node
//                    if (n.sockets[o.name].ConnectedSockets[0].OwnerNode is TriggerscripterNode_Variable)
//                    {
//                        //try and add the var
//                        TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[o.name].ConnectedSockets[0].OwnerNode;
//                        AddVar(
//                        v.Id,
//                        v.Type,
//                        v.Name,
//                        false,
//                        v.Name,
//                        triggerName + "::" + n.Name,
//                        varX);
//                        id = v.Id;
//                    }
//                    //if the socket is something else
//                    else
//                    {
//                        //check if there is already an invisible linked var for this socket
//                        if (linkedVars.ContainsKey((TriggerscripterSocket_Output)n.sockets[o.name]))
//                        {
//                            id = linkedVars[(TriggerscripterSocket_Output)n.sockets[o.name]];
//                        }
//                        //try and add a linked var
//                        else
//                        {
//                            AddVar(
//                                varId,
//                                o.valueType,
//                                "linked" + o.valueType,
//                                false,
//                                null,
//                                triggerName + "::" + n.Name,
//                                varX);
//                            id = varId;
//                            linkedVars.Add((TriggerscripterSocket_Output)n.sockets[o.name], varId);
//                            varId++;

//                        }
//                    }
//                }
//                //socket is not attached to anything. add a null variable
//                else
//                {
//                    AddVar(
//                        varId,
//                        n.sockets[o.name].ValueType,
//                        "null" + n.sockets[o.name].ValueType,
//                        true,
//                        null,
//                        triggerName + "::" + n.Name,
//                        varX);
//                    id = varId;
//                    varId++;
//                }
//                XElement output = new XElement("Output");
//                output.Add(new XAttribute("Name", n.sockets[o.name].Text));
//                output.Add(new XAttribute("SigID", o.sigId));
//                output.Add(new XAttribute("Optional", o.optional));
//                output.Value = id.ToString();
//                cnd.Add(output);
//            }

//            triggerCnd.Add(cnd);
//        }
//        int AddTrigger(TriggerscripterNode n, XElement triggers, XElement triggerVars)
//        {
//            if (triggerVarLinks.ContainsKey(n.Id))
//            {
//                //trigger has already been processed
//                return triggerVarLinks[n.Id];
//            }
//            else
//            {
//                //root node
//                XElement trigger = new XElement("Trigger");
//                trigger.Add(new XAttribute("ID", ((TriggerscripterNode_Trigger)n).Id));
//                trigger.Add(new XAttribute("Name", ((TriggerscripterNode_Trigger)n).Name));
//                trigger.Add(new XAttribute("Active", ((TriggerscripterNode_Trigger)n).Name));
//                trigger.Add(new XAttribute("EvaluateFrequency", 0));
//                trigger.Add(new XAttribute("EvalLimit", 0));
//                trigger.Add(new XAttribute("CommentOut", false));
//                trigger.Add(new XAttribute("ConditionalTrigger", ((TriggerscripterNode_Trigger)n).Conditional));
//                trigger.Add(new XAttribute("X", n.PosX));
//                trigger.Add(new XAttribute("Y", n.PosY));
//                trigger.Add(new XAttribute("GroupID", -1));
//                trigger.Add(new XAttribute("TemplateID", -1));
//                XElement triggerCnd = new XElement("TriggerConditions");
//                XElement triggerT = new XElement("TriggerEffectsOnTrue");
//                XElement triggerF = new XElement("TriggerEffectsOnFalse");
//                trigger.Add(triggerCnd);
//                trigger.Add(triggerT);
//                trigger.Add(triggerF);

//                TriggerscripterNode_Trigger t = (TriggerscripterNode_Trigger)n;

//                //wip commenting
//                //triggerVars.Add(new XComment("==Trigger: " + ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text + "=="));

//                //add triger and create reference variable for it
//                triggers.Add(trigger);
//                AddVar(varId, "Trigger", n.Name, false, n.Id.ToString(), n.Name, triggerVars);
//                triggerVarLinks.Add(n.Id, varId);
//                varId++;

//                //write condition type
//                XElement cndElement;
//                if (t.Conditional == false)
//                    cndElement = new XElement("And");
//                else
//                    cndElement = new XElement("Or");
//                triggerCnd.Add(cndElement);

//                //add all conditions
//                if (t.sockets["Conditions"].ConnectedSockets.Count > 0)
//                {
//                    foreach (TriggerscripterSocket s in t.sockets["Conditions"].ConnectedSockets)
//                    {
//                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + s.node.nodeTitle + " (Condition)"));
//                        AddCondition(s.OwnerNode, t.Name, cndElement, triggerVars);
//                    }
//                }

//                //add all effects on true
//                if (t.sockets["Call On True"].ConnectedSockets.Count > 0)
//                {
//                    TriggerscripterNode last = t.sockets["Call On True"].ConnectedSockets[0].OwnerNode;
//                    while (last != null)
//                    {
//                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
//                        if (!last.Name.Contains("Trigger"))
//                        {
//                            AddEffect(last, t.Name, triggerT, triggerVars, 0);
//                        }
//                        else
//                        {
//                            if (last.sockets["Trigger"].ConnectedSockets.Count > 0)
//                            {
//                                AddTrigger(last.sockets["Trigger"].ConnectedSockets[0].OwnerNode, triggers, triggerVars);
//                                AddEffect(last, t.Name, triggerT, triggerVars, triggerVarLinks[last.sockets["Trigger"].ConnectedSockets[0].OwnerNode.Id]);
//                            }
//                        }

//                        if (last.sockets["Call"].ConnectedSockets.Count > 0) last = last.sockets["Call"].ConnectedSockets[0].OwnerNode;
//                        else last = null;
//                    }
//                }

//                //add all effects on false
//                if (t.sockets["Call On False"].ConnectedSockets.Count > 0)
//                {
//                    TriggerscripterNode last = t.sockets["Call On False"].ConnectedSockets[0].OwnerNode;
//                    while (last != null)
//                    {
//                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
//                        //triggerVars.Add(new XComment("-" + t.nodeTitle + "::" + last.nodeTitle + " (Effect)"));
//                        if (!last.Name.Contains("Trigger"))
//                        {
//                            AddEffect(last, t.Name, triggerF, triggerVars, 0);
//                        }
//                        else
//                        {
//                            if (last.sockets["Trigger"].ConnectedSockets.Count > 0)
//                            {
//                                AddTrigger(last.sockets["Trigger"].ConnectedSockets[0].OwnerNode, triggers, triggerVars);
//                                AddEffect(last, t.Name, triggerF, triggerVars, triggerVarLinks[last.sockets["Trigger"].ConnectedSockets[0].OwnerNode.Id]);
//                            }
//                        }

//                        if (last.sockets["Call"].ConnectedSockets.Count > 0) last = last.sockets["Call"].ConnectedSockets[0].OwnerNode;
//                        else last = null;
//                    }
//                }
                
//                return triggerVarLinks[n.Id];
//            }
//        }

//        public void Compile(List<TriggerscripterNode> nodes, int lastVarId, string outPath)
//        {
//            varId = lastVarId;
//            XDocument x = new XDocument();

//            //root node
//            XElement triggerSystem = new XElement("TriggerSystem");
//            triggerSystem.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
//            triggerSystem.Add(new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));
//            triggerSystem.Add(new XAttribute("Name", Path.GetFileName(outPath)));
//            triggerSystem.Add(new XAttribute("Type", "TriggerScript"));
//            triggerSystem.Add(new XAttribute("NextTriggerVarID", lastVarId));
//            triggerSystem.Add(new XAttribute("NextTriggerID", 9999));
//            triggerSystem.Add(new XAttribute("NextConditionID", 9999));
//            triggerSystem.Add(new XAttribute("NextEffectID", 9999));
//            x.Add(triggerSystem);

//            //fixed child nodes
//            XElement triggerGroups = new XElement("TriggerGroups");
//            triggerSystem.Add(triggerGroups);
//            XElement triggerVars = new XElement("TriggerVars");
//            triggerSystem.Add(triggerVars);
//            XElement triggers = new XElement("Triggers");
//            triggerSystem.Add(triggers);


//            //compile from entry points (active triggers)
//            foreach(TriggerscripterNode n in nodes)
//            {
//                if(n.HandleAs == "Trigger")
//                {
//                    if(((TriggerscripterNode_Trigger)n).Active)
//                    {
//                        AddTrigger(n, triggers, triggerVars);
//                    }
//                }
//            }
            
//            triggerVars.ReplaceNodes(
//                triggerVars.Elements().OrderBy(y => int.Parse(y.Attribute("ID").Value)));

//            x.Save(outPath);
//        }
//    }
//}
