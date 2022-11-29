using System;
using System.Windows.Forms;
using Foundry.DockingModules.PropertyEditor;
using Foundry.DockingModules.ProjectExplorer;
using Foundry.Project;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace Foundry
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
        }

        
        public void OpenProject(string file)
        {
            project = ModProject.Open(file);
        }
        public void CloseProject()
        {
            //project.Close();
            project = null;
        }

        #region GetsSets
        public DockPanel Workspace()
        {
            return workspace;
        }
        #endregion

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenProject(ofd.FileName);
            }
        }
    }
}
