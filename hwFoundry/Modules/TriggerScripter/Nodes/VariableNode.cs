using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwFoundry.Modules.TriggerScripter.Nodes
{
    public class ValueBox : STNodeControl
    {
        protected override void OnPaint(DrawingTools dt)
        {
            base.OnPaint(dt);
            Graphics g = dt.Graphics;
            g.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), this.ClientRectangle);
            m_sf.Alignment = StringAlignment.Near;
            g.DrawString(this.Text, this.Font, Brushes.White, this.ClientRectangle, m_sf);
        }
    }

    public class VariableNode : BaseNode
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetAndInvalidate(ref name, value);
        }
        private string valueString;
        public string Value
        {
            get => valueString;
            set
            {
                SetAndInvalidate(ref valueString, value);
                if (m_con_value != null)
                    m_con_value.Text = value;
            }
        }

        private ValueBox m_con_value;

        public VariableNode(SerializedVariable var, int id, int x, int y): base(x, y, id)
        {
            // Serialized Data
            data = var;
            Name = var.name;
            Value = var.value;

            // Header data
            TitleColor = RequiredVarColor;
            handleAs = "Variable";
            nodeTitle = var.name;
            typeTitle = var.type;

            // Inputs
            InputOptions.Add("Set", typeof(VariableNode), var.type, false);

            // Outputs
            OutputOptions.Add("Use", typeof(VariableNode), var.type, false);

            m_con_value = new()
            {
                Text = Value,
                DisplayRectangle = new Rectangle(3, ItemHeight * (InputOptionsCount > OutputOptionsCount ? InputOptionsCount : OutputOptionsCount), (Width * 2) - 6, ItemHeight)
            };
            Controls.Add(m_con_value);
        }

        protected override void OnOwnerChanged()
        {
            base.OnOwnerChanged();
            Height += 3;
        }
    }
}
