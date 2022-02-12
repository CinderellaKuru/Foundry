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
using SMHEditor.Project.FileTypes;

namespace SMHEditor
{
    public partial class MainWindow : KryptonForm
    {
        public MainWindow()
        {
            InitializeComponent();
            project = new ModProject("C:/project.hwproj"); //change this to somewhere on your desktop
            project.Save();
        }


        //Project stuff
        public static ModProject project;


        //Docking stuff
        //Krypton name strings, global.
        public static string WORKSPACE_NAME = "Workspace";
        public static string DOCKINGCONTROLLER_NAME = "Control";
        public static string FLOATING_NAME = "Floating";

        //Code to execute directly after the main window is loaded.
        private void MainWindow_Load(object sender, EventArgs e)
        {
            KryptonDockingWorkspace w = dockingManager.ManageWorkspace(WORKSPACE_NAME, dockableWorkspace);
            dockingManager.ManageControl(DOCKINGCONTROLLER_NAME, kryptonPanel, w);
            dockingManager.ManageFloating(FLOATING_NAME, this);

            ObjectFile o = new ObjectFile();
            o.Ability = "Ability1";
            YAXLib.YAXSerializer s = new YAXLib.YAXSerializer(typeof(ObjectFile), YAXLib.Enums.YAXSerializationOptions.DontSerializeNullObjects);
            project.Create("c:/users/jaken/desktop/testproj/data/", "New", ModProject.FileType.FOLDER);
            //s.SerializeToFile(o, "c:/users/jaken/desktop/testproj/data/New/object2.obj");
            //s.SerializeToFile(o, "c:/users/jaken/desktop/testproj/data/New/object3.obj");

            dockingManager.AddDockspace(DOCKINGCONTROLLER_NAME, DockingEdge.Left, new KryptonPage[] { new ProjectExplorerPage("\\data", "Data") });
        }
    }
}
