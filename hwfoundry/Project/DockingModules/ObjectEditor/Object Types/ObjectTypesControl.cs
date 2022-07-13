using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Object_Types
{
    public partial class ObjectTypesControl : UserControl
    {
        private readonly ObjectFile obj;
        public ObjectTypesControl(ObjectFile o)
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
            flowLayoutPanel.Controls.Add(new ObjectTypeControl(obj, flag, this));
        }
    }
}
