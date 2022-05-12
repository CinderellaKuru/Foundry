using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class TriggerscripterPage : KryptonPage
    {
        TriggerscripterControl c;

        public TriggerscripterPage()
        {
            c = new TriggerscripterControl();
            c.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            c.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(c);
        }
    }
}
