using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hwFoundry.Project;
using WeifenLuo.WinFormsUI.Docking;

namespace hwFoundry.GUI
{
    public partial class MainWindow : Form
    {
        // Module Handles
        public ModProject? modProject;
        public PropertyEditor? propertyEditor;
        public ProjectExplorer? projectExplorer;

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            label_version.Text = $"Version: {System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}";
        }

        #region Events
        private void MainWindow_Load(object sender, EventArgs e)
        {
            workspace.Theme = new VS2015DarkTheme();

            propertyEditor = new();
            propertyEditor.Show(workspace, DockState.DockRight);

            projectExplorer = new();
            projectExplorer.Show(workspace, DockState.DockLeft);
        }

        private void OpenProject_Click(object sender, EventArgs e)
            => OpenProject();

        private void CreateNewProject_Clicked(object sender, EventArgs e)
            => CreateNewProject();

        private void OpenPropertyEditor_Click(object sender, EventArgs e)
        {
            if (propertyEditor.IsDisposed)
            {
                propertyEditor = new();
                propertyEditor.Show(workspace, DockState.DockRight);
            }
        }

        private void OpenProjectExplorer_Click(object sender, EventArgs e)
        {
            if (projectExplorer.IsDisposed)
            {
                projectExplorer = new();
                projectExplorer.Show(workspace, DockState.DockLeft);
            }
        }

        private void SaveProject_Clicked(object sender, EventArgs e)
            => SaveProject();

        private void SaveProjectAs_Clicked(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void CompileProject_Clicked(object sender, EventArgs e)
            => CompileProject();

        #endregion

        #region Methods

        private void CreateNewProject()
        {
            throw new NotImplementedException();
        }

        public void OpenProject()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = $"Foundry Project|*{ModProject.PROJ_EXT}",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            modProject = ModProject.OpenProject(openFileDialog.FileName);
        }

        private void SaveProject()
        {
            throw new NotImplementedException();
        }

        private void CompileProject()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Getters/Setters
        public DockPanel GetWorkspace()
            => workspace;
        #endregion
    }
}
