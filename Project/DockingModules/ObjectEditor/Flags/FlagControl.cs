using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Flags
{
    public partial class FlagControl : UserControl
    {
        private readonly FlagsControl parent;
        private readonly string fg;
        private readonly ObjectFile obj;
        public FlagControl(ObjectFile o, string flag, FlagsControl owner)
        {
            flag = fg;
            InitializeComponent();
            parent = owner;
            obj = o;
            Dock = DockStyle.Right;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            parent.flowLayoutPanel.Controls.Remove(this);
            obj.Flag.Remove(fg);
        }
    }
}
