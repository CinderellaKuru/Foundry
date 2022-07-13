using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class HardpointsControl : UserControl
    {
        private readonly ObjectFile obj;
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
