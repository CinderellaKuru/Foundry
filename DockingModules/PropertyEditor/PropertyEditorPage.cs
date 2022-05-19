using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.DockingModules.PropertyEditor
{
    public class PropertyEditorPage : KryptonPage
    {
        public PropertyEditorControl control = new PropertyEditorControl();
        public PropertyEditorPage()
        {
            control.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(control);
        }
    }
}
