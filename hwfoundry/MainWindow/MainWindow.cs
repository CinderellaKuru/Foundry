using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMHEditor.DockingModules.MapEditor;
using SMHEditor.DockingModules.PropertyEditor;
using SMHEditor.DockingModules.ProjectExplorer;
using SMHEditor.DockingModules.Triggerscripter;
using SMHEditor.Project;
using SMHEditor.Project.FileTypes;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace SMHEditor
{
    public partial class Foundry : Form
    {
        public Foundry()
        {
            InitializeComponent();
            versionReadout.Text = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
        }
        
        public ModProject project;
        public PropertyEditor propertyEditor;
        public ProjectExplorer projectExplorer;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            workspace.Theme = new VS2015LightTheme();

            projectExplorer = new ProjectExplorer();
            projectExplorer.Show(workspace, DockState.DockLeft);

            propertyEditor = new PropertyEditor();
            propertyEditor.Show(workspace, DockState.DockRight);

#if DEBUG
            InitProject("C:\\users\\jake\\desktop\\testproj2\\project.hwfp");
#endif
        }
        
        private void InitProject(string file)
        {
            project = new ModProject(file);
        }

        #region GetsSets
        public DockPanel Workspace()
        {
            return workspace;
        }
        #endregion
    }
}
