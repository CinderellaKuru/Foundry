using SMHEditor.Project.FileTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Object_Childs
{
    public partial class ObjectChildControl : UserControl
    {
        private readonly ObjectChildsControl parent;
        private readonly string oc;
        private readonly ObjectFile obj;
        public ObjectChildControl(ObjectFile o, string objectChild, ObjectChildsControl owner)
        {
            objectChild = oc;
            InitializeComponent();
            parent = owner;
            obj = o;
            Dock = DockStyle.Right;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            parent.flowLayoutPanel.Controls.Remove(this);
            obj.ObjectChild.Remove(oc);
        }
    }
}
