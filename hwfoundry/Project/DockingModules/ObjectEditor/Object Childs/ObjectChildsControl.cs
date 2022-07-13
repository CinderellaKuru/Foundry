using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Object_Childs
{
    public partial class ObjectChildsControl : UserControl
    {
        private readonly ObjectFile obj;
        public ObjectChildsControl(ObjectFile o)
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
            flowLayoutPanel.Controls.Add(new ObjectChildControl(obj, flag, this));
        }
    }
}
