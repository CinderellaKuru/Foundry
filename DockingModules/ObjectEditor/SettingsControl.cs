using SMHEditor.Project.FileTypes;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class SettingsControl : UserControl
    {
        private readonly ObjectFile obj;
        public SettingsControl(ObjectFile o)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            obj = o;
        }
    }
}
