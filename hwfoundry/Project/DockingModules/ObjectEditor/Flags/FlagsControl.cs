using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Flags
{
    public partial class FlagsControl : UserControl
    {
        private readonly ObjectFile obj;
        public FlagsControl(ObjectFile o)
        {
            InitializeComponent();
            add.MouseClick += new MouseEventHandler(Add);
            Dock = DockStyle.Fill;
            obj = o;
        }

        private void Add(object o, EventArgs e)
        {
            string flag = "";
            obj.Flag.Add(flag);
            flowLayoutPanel.Controls.Add(new FlagControl(obj, flag, this));
        }
    }
}
