using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class HardpointControl : UserControl
    {
        public static int HEIGHT = 35;
        private readonly Hardpoint hardpoint;
        private readonly HardpointsControl parent;
        private readonly ObjectFile obj;
        public HardpointControl(ObjectFile o, Hardpoint hp, HardpointsControl owner)
        {
            hardpoint = hp;
            InitializeComponent();
            parent = owner;
            obj = o;
            Dock = DockStyle.Right;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            parent.flowLayoutPanel.Controls.Remove(this);
            obj.Hardpoint.Remove(hardpoint);
        }
    }
}
