using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class TriggerscripterCompiler
    {
        static int varID = 0;
        static void AddVar(XElement triggerVars, Variable v)
        {
            XElement var = new XElement("TriggerVar");
            var.Add(new XAttribute("ID", v.id));
            var.Add(new XAttribute("Type", v.type));
            var.Add(new XAttribute("Name", v.name));
            var.Add(new XAttribute("IsNull", false));
            triggerVars.Add(var);
        }
        public static void Compile(List<TriggerscripterNode> nodes, string outPath)
        {
            Dictionary<TriggerscripterNode, Variable> vars = new Dictionary<TriggerscripterNode, Variable>();

            XDocument x = new XDocument();
            XElement triggerSystem = new XElement("TriggerSystem");
            x.Add(triggerSystem);
            XElement triggerGroups = new XElement("TriggerGroups");
            XElement triggerVars = new XElement("TriggerVars");
            XElement triggers = new XElement("Triggers");
            triggerSystem.Add(triggerGroups);
            triggerSystem.Add(triggerVars);
            triggerSystem.Add(triggers);



            x.Save(outPath);
        }
    }
}
