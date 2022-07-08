using SMHEditor.Project.FileTypes;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class UIControl : UserControl
    {
        private readonly ObjectFile obj;
        public UIControl(ObjectFile o)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            obj = o;
        }
    }
}
