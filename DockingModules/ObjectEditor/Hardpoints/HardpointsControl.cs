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
using ComponentFactory.Krypton.Toolkit;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class HardpointsControl : UserControl
    {
        ObjectFile obj;
        public HardpointsControl(ObjectFile o)
        {
            InitializeComponent();
            add.MouseClick += new MouseEventHandler(Add);
            Dock = DockStyle.Fill;
            obj = o;
        }

        private void Add(object o, EventArgs e)
        {
            Hardpoint hp = new Hardpoint();
            obj.Hardpoint.Add(hp);
            flowLayoutPanel.Controls.Add(new HardpointControl(obj, hp, this));
        }
    }
}
