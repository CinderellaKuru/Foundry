using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwFoundry.Modules.TriggerScripter.Nodes
{
    public class TriggerNode : BaseNode
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetAndInvalidate(ref name, value);
        }
        private bool conditionalOr;
        public bool ConditionalOr
        {
            get => conditionalOr;
            set => SetAndInvalidate(ref conditionalOr, value);
        }
        private bool active;
        public bool Active
        {
            get => active;
            set => SetAndInvalidate(ref active, value);
        }
        private bool conditional;
        public bool Conditional
        {
            get => conditional;
            set => SetAndInvalidate(ref conditional, value);
        }

        public TriggerNode(SerializedTrigger trg, int id, int x, int y) : base(x, y, id)
        {
            // Serialized Data
            data = trg;
            Active = trg.active;
            ConditionalOr = trg.cndIsOr;
            Name = trg.name;

            // Header data
            TitleColor = Color.FromArgb(255, 64, 117, 130);
            typeTitle = "Trigger";
            handleAs = "Trigger";

            // Inputs
            InputOptions.Add("Caller", typeof(TriggerNode), false);
            InputOptions.Add("Conditions", typeof(ConditionNode), false);
            
            // Outputs
            OutputOptions.Add("Call On True",  typeof(EffectNode), true);
            OutputOptions.Add("Call On False", typeof(EffectNode), true);
        }

        #region Overrides
        protected override void OnDrawTitle(DrawingTools dt)
        {
            base.OnDrawTitle(dt);

            // Controls the size of all ellipses
            int ellipseSize = 12;

            // Outline brush and coordinates
            Pen outline = new(new SolidBrush(Color.Black), 1.5f);
            int headerOffsetX = TitleRectangle.X + Width - 5;
            int headerOffsetY = TitleRectangle.Y + 3;

            // Draw "Active" Indicator
            SolidBrush activeBrush = new(active ? Color.Green : Color.Red);
            dt.Graphics.FillEllipse(activeBrush, headerOffsetX - ellipseSize, headerOffsetY, ellipseSize, ellipseSize);
            dt.Graphics.DrawEllipse(outline,     headerOffsetX - ellipseSize, headerOffsetY, ellipseSize, ellipseSize);

            // Draw "Conditional" Indicator
            SolidBrush conditionalBrush = new(!conditional ? BackColor : Color.FromArgb(205, 170, 0));
            dt.Graphics.FillEllipse(conditionalBrush, headerOffsetX - (2 * ellipseSize) - 5, headerOffsetY, ellipseSize, ellipseSize);
            dt.Graphics.DrawEllipse(outline,          headerOffsetX - (2 * ellipseSize) - 5, headerOffsetY, ellipseSize, ellipseSize);
        }
        #endregion
    }
}
