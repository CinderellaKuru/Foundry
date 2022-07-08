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
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.DockingModules.MapEditor;
using SMHEditor.DockingModules.PropertyEditor;
using SMHEditor.DockingModules.ProjectExplorer;
using SMHEditor.DockingModules.Triggerscripter;
using SMHEditor.Project;
using SMHEditor.Project.FileTypes;
using System.Reflection;
using DarkUI.Win32;
using WeifenLuo.WinFormsUI.Docking;

namespace SMHEditor
{
    public partial class Foundry : KryptonForm
    {
        public Foundry()
        {
            InitializeComponent();
            versionReadout.Text = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
        }

        //Project stuff
        public static ModProject project;
        public static ViewportPage vp;
        public static TriggerscripterPage ts;
        public static PropertyEditorPage propertyEditor;
        public static ProjectExplorer projectExplorer;

        //Docking stuff
        //Krypton name strings, global.
        public static string WORKSPACE_NAME = "Workspace";
        public static string DOCKINGCONTROLLER_NAME = "Control";
        public static string FLOATING_NAME = "Floating";

        //Code to execute directly after the main window is loaded.
        private void MainWindow_Load(object sender, EventArgs e)
        {
            newProjectTMI.Click                 += new EventHandler(NewProjectPressed);
            openProjectTMI.Click                += new EventHandler(OpenProjectPressed);
            saveTMI.Click                       += new EventHandler(SavePressed);
            saveAsTMI.Click                     += new EventHandler(SaveAsPressed);
            
#if DEBUG
            InitProject("C:\\users\\jake\\desktop\\testproj2\\project.hwfp");
#endif
        }


        private void InitProject(string file)
        {
            project = new ModProject(file);
        }

        private void NewProjectPressed(object o, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Foundry Project|*.hwfp";

            if(sfd.ShowDialog() == DialogResult.OK)
            {
                InitProject(sfd.FileName);
            }

        }
        private void OpenProjectPressed(object o, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Foundry Project|*.hwfp";
            ofd.Multiselect = false;

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                InitProject(ofd.FileName);
            }
        }
        private void SavePressed(object o, EventArgs e)
        {
            project.SaveActiveFile();
        }
        private void SaveAsPressed(object o, EventArgs e)
        {

        }


        #region GetSet
        public DockPanel Workspace()
        {
            return workspace;
        }
        #endregion
    }
}
