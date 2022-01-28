using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.DockingModules.ProjectExplorer;
using SMHEditor.Project;

namespace SMHEditor
{
    public partial class MainWindow : KryptonForm
    {
        public MainWindow()
        {
            InitializeComponent();
            project = Project.Project.OpenProject("C:/users/jaken/desktop/test.hwp", false);

        }


        //Project stuff
        public static Project.Project project;



        //Docking stuff
        //Krypton name strings, global.
        static string WORKSPACE_NAME = "Workspace";
        static string DOCKINGCONTROLLER_NAME = "Control";
        static string FLOATING_NAME = "Floating";

        //Code to execute directly after the main window is loaded.
        private void MainWindow_Load(object sender, EventArgs e)
        {
            KryptonDockingWorkspace w = dockingManager.ManageWorkspace(WORKSPACE_NAME, dockableWorkspace);
            dockingManager.ManageControl(DOCKINGCONTROLLER_NAME, kryptonPanel, w);
            dockingManager.ManageFloating(FLOATING_NAME, this);

            dockingManager.AddDockspace(DOCKINGCONTROLLER_NAME, DockingEdge.Left, new KryptonPage[] { new ProjectExplorerPage() });
        }
    }
}
