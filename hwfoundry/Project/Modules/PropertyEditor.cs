using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;

namespace Foundry.DockingModules.PropertyEditor
{
    public class PropertyEditor : DockContent
    {
        PropertyGrid grid;

        public PropertyEditor()
        {
            grid = new PropertyGrid();
            grid.Dock = DockStyle.Fill;
            Controls.Add(grid);
        }

        public void SetSelectedObject(object o)
        {
            grid.SelectedObject = o;
        }
    }
}
