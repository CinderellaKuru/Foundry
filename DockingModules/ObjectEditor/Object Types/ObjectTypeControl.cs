using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Object_Types
{
    public partial class ObjectTypeControl : UserControl
    {
        private readonly ObjectTypesControl parent;
        private readonly string ot;
        private readonly ObjectFile obj;
        public ObjectTypeControl(ObjectFile o, string objectType, ObjectTypesControl owner)
        {
            objectType = ot;
            InitializeComponent();
            parent = owner;
            obj = o;
            Dock = DockStyle.Right;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            parent.flowLayoutPanel.Controls.Remove(this);
            obj.ObjectType.Remove(ot);
        }
    }
}
