using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMHEditor.Project.FileTypes;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class HardpointControl : UserControl
    {
        public static int HEIGHT = 35;
        Hardpoint hardpoint;
        HardpointsControl parent;
        ObjectFile obj;
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
