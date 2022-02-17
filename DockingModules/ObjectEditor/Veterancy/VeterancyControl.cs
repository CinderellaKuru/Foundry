using SMHEditor.Project.FileTypes;
using System;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Veterancy
{
    public partial class VeterancyControl : UserControl
    {
        private readonly Project.FileTypes.Veterancy veterancy;
        private readonly VeterancysControl parent;
        private readonly ObjectFile obj;
        public VeterancyControl(ObjectFile o, Project.FileTypes.Veterancy vt, VeterancysControl owner)
        {
            veterancy = vt;
            InitializeComponent();
            parent = owner;
            obj = o;
            Dock = DockStyle.Right;

            if (obj.Veterancy.Count == 1)
            {
                deleteButton.Visible = false;
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            parent.flowLayoutPanel.Controls.Remove(this);
            obj.Veterancy.Remove(veterancy);
        }
    }
}
