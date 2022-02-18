using SMHEditor.Project.FileTypes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Commands
{
    public partial class CommandControl : UserControl
    {
        private readonly Command command;
        private readonly CommandsControl parent;
        private readonly ObjectFile obj;
        public CommandControl(ObjectFile o, Command cm, CommandsControl owner)
        {
            command = cm;
            InitializeComponent();
            parent = owner;
            obj = o;
            Dock = DockStyle.Right;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            parent.flowLayoutPanel.Controls.Remove(this);
            obj.Command.Remove(command);
        }

        private void position_ValueChanged(object sender, EventArgs e)
        {
            Image img;
            switch (position.Value)
            {
                case 0:
                    img = Properties.Resources.Placement0;
                    placementBox.Image = img;
                    break;
                case 1:
                    img = Properties.Resources.Placement1;
                    placementBox.Image = img;
                    break;
                case 2:
                    img = Properties.Resources.Placement2;
                    placementBox.Image = img;
                    break;
                case 3:
                    img = Properties.Resources.Placement3;
                    placementBox.Image = img;
                    break;
                case 4:
                    img = Properties.Resources.Placement4;
                    placementBox.Image = img;
                    break;
                case 5:
                    img = Properties.Resources.Placement5;
                    placementBox.Image = img;
                    break;
                case 6:
                    img = Properties.Resources.Placement6;
                    placementBox.Image = img;
                    break;
                case 7:
                    img = Properties.Resources.Placement7;
                    placementBox.Image = img;
                    break;
            }

        }
    }
}
