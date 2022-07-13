using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.PropertyEditor
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
