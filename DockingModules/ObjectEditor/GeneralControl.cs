using SMHEditor.Project.FileTypes;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor
{
    public partial class GeneralControl : UserControl
    {
        private readonly ObjectFile obj;
        public GeneralControl(ObjectFile o)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            obj = o;
        }
    }
}
