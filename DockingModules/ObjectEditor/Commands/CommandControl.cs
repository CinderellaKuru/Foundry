using SMHEditor.Project.FileTypes;
using System;
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
    }
}
