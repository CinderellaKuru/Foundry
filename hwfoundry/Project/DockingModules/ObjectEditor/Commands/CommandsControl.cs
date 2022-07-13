using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Commands
{
    public partial class CommandsControl : UserControl
    {
        private readonly ObjectFile obj;
        public CommandsControl(ObjectFile o)
        {
            InitializeComponent();
            add.MouseClick += new MouseEventHandler(Add);
            Dock = DockStyle.Fill;
            obj = o;
        }

        private void Add(object o, EventArgs e)
        {
            Command cm = new Command();
            obj.Command.Add(cm);
            flowLayoutPanel.Controls.Add(new CommandControl(obj, cm, this));
        }
    }
}
