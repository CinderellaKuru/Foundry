using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwFoundry.Modules.TriggerScripter.Nodes
{
    public class EffectNode : BaseNode
    {
        public EffectNode(SerializedEffect eff, int id, int x, int y): base(x, y, id)
        {
            // Serialized Data
            data = eff;

            // Header data
            nodeTitle = $"{eff.name} v{eff.version}";
            typeTitle = "Effect";
            handleAs = "Effect";
            TitleColor = Color.FromArgb(255, 130, 64, 106);
            ItemHeight *= 2;

            // Inputs
            InputOptions.Add("Caller", typeof(EffectNode), true);
            
            // Outputs
            OutputOptions.Add("Call", typeof(EffectNode), true);

            if (!eff.name.Contains("Trigger"))
            {
                foreach (Input i in eff.inputs)
                {
                    //Color color = i.optional ? optionalVarColor : requiredVarColor;
                    STNodeOption inn = new($"{i.name}\n[{i.valueType}]", typeof(VariableNode), i.valueType, true);
                    inn.DotColor = i.optional ? OptionalVarColor : RequiredVarColor;
                    InputOptions.Add(inn);
                }
                foreach (Output ou in eff.outputs)
                {
                    //Color color = ou.optional ? optionalVarColor : requiredVarColor;
                    STNodeOption oun = new($"{ou.name}\n[{ou.valueType}]", typeof(VariableNode), ou.valueType, true);
                    oun.DotColor = ou.optional ? OptionalVarColor : RequiredVarColor;
                    OutputOptions.Add(oun);
                }
            }
            else
            {
                OutputOptions.Add("Trigger", typeof(TriggerNode), true);
            }
        }
    }
}
