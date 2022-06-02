using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SMHEditor.DockingModules.Triggerscripter.TriggerscripterControl;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class TriggerscripterCompiler
    {
        List<int> varIds = new List<int>();
        int varId = -1;
        void AddVar(int id, string type, string name, bool isNull, string value, XElement varX)
        {
            if (varIds.Contains(id)) return;
            else
            {
                XElement v = new XElement("TriggerVar");
                v.Add(new XAttribute("ID", id));
                v.Add(new XAttribute("Type", type));
                v.Add(new XAttribute("Name", name));
                v.Add(new XAttribute("IsNull", isNull));
                v.SetValue(value);
            }
        }
        void AddEffect(TriggerscripterNode n, XElement triggerTF, XElement varX)
        {
            if (n.handleAs != "Effect") return;

            XElement eff = new XElement("Effect");
            eff.Add(new XAttribute("ID", n.id));
            eff.Add(new XAttribute("Type", n.nodeTitle));
            eff.Add(new XAttribute("DBID", ((Effect)n.data).dbid));
            eff.Add(new XAttribute("Version", ((Effect)n.data).version));
            eff.Add(new XAttribute("CommentOut", false));
            
            foreach(Input i in ((Effect)n.data).inputs)
            {
                int id;
                if(n.sockets[i.name].connectedSockets.Count > 0)
                {
                    TriggerscripterNode_Variable v = (TriggerscripterNode_Variable)n.sockets[i.name].connectedSockets[0].node;
                    AddVar(
                        v.id,
                        v.typeTitle,
                        v.nameProperty.tb.Text,
                        false,
                        v.valueProperty.tb.Text,
                        varX);
                    id = v.id;
                }
                else
                {
                    AddVar(
                        varId,
                        n.sockets[i.name].valueType,
                        "newNull" + n.sockets[i.name].valueType + "Var" + varId,
                        true,
                        "",
                        varX);
                    id = varId;
                    varId++;
                }
                XElement input = new XElement("Input");
                input.Add(new XAttribute("Name", n.sockets[i.name].text));
                input.Add(new XAttribute("SigID", i.sigId));
                input.Add(new XAttribute("Optional", i.optional));
                input.Value = id.ToString();
            }

            triggerTF.Add(eff);
        }

        public void Compile(List<TriggerscripterNode> nodes, int lastVarId, string outPath)
        {
            varId = lastVarId;

            XDocument x = new XDocument();
            XElement triggerSystem = new XElement("TriggerSystem");
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
                        XElement trigger = new XElement("Trigger");
                        trigger.Add(new XAttribute("ID", ((TriggerscripterNode_Trigger)n).id));
                        trigger.Add(new XAttribute("Name", ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text));
                        trigger.Add(new XAttribute("Active", ((TriggerscripterNode_Trigger)n).activeProperty.state));
                        trigger.Add(new XAttribute("EvaluateFrequency", 0));
                        trigger.Add(new XAttribute("EvalLimit", 0));
                        trigger.Add(new XAttribute("CommentOut", false));
                        trigger.Add(new XAttribute("ConditionalTrigger", false));
                        trigger.Add(new XAttribute("X", n.x));
                        trigger.Add(new XAttribute("Y", n.y));
                        XElement triggerT = new XElement("TriggerEffectsOnTrue");
                        XElement triggerF = new XElement("TriggerEffectsOnFalse");
                        trigger.Add(triggerT);
                        trigger.Add(triggerF);

                        TriggerscripterNode_Trigger t = (TriggerscripterNode_Trigger)n;
                        if(t.sockets["Call On True"].connectedSockets.Count > 0)
                        {
                            TriggerscripterNode last = t.sockets["Call On True"].connectedSockets[0].node;
                            while(last != null)
                            {
                                AddEffect(last, triggerT, triggerVars);
                                if (last.sockets["Call"].connectedSockets.Count > 0) last = last.sockets["Call"].connectedSockets[0].node;
                                else last = null;
                            }
                        }

                        triggers.Add(trigger);
                    }
                }
            }


            x.Save(outPath);
        }
    }
}
