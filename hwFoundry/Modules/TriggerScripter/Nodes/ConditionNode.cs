using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwFoundry.Modules.TriggerScripter.Nodes
{
    public class ConditionNode : BaseNode
    {
        private bool _inverted;
        public bool Inverted
        {
            get => _inverted;
            set => SetAndInvalidate(ref _inverted, value);
        }

        public ConditionNode(SerializedCondition cnd, int id, int x, int y) : base(x, y, id)
        {
            // Serialized data
            data = cnd;

            // Appearance
            TitleColor = Color.FromArgb(255, 130, 64, 64);
            nodeTitle = $"{cnd.name} v{cnd.version}";
            handleAs = "Condition";
            typeTitle = "Condition";
            ItemHeight *= 2;

            // Outputs
            OutputOptions.Add("Result", typeof(ConditionNode), true);

            foreach (Input i in cnd.inputs)
            {
                STNodeOption inn = new($"{i.name}\n[{i.valueType}]", typeof(VariableNode), i.valueType, true);
                inn.DotColor = i.optional ? OptionalVarColor : RequiredVarColor;
                inn.sigId = i.sigId;
                inn.optional = i.optional;
                InputOptions.Add(inn);
            }

            foreach (Output ou in cnd.outputs)
            {
                STNodeOption oun = new($"{ou.name}\n[{ou.valueType}]", typeof(VariableNode), ou.valueType, true);
                oun.DotColor = ou.optional ? OptionalVarColor : RequiredVarColor;
                OutputOptions.Add(oun);
            }
        }

        protected override void OnDrawTitle(DrawingTools dt)
        {
            base.OnDrawTitle(dt);

            // Controls the size of all ellipses
            int ellipseSize = 12;

            // Outline brush and coordinates
            Pen outline = new(new SolidBrush(Color.Black), 1.5f);
            int headerOffsetX = TitleRectangle.X + Width - 5;
            int headerOffsetY = TitleRectangle.Y + 3;

            // Draw "Inverted" Indicator
            SolidBrush invertedBrush = new(!_inverted ? BackColor : Color.FromArgb(205, 170, 0));
            dt.Graphics.FillEllipse(invertedBrush, headerOffsetX - ellipseSize, headerOffsetY, ellipseSize, ellipseSize);
            dt.Graphics.DrawEllipse(outline,       headerOffsetX - ellipseSize, headerOffsetY, ellipseSize, ellipseSize);
        }
    }
}
