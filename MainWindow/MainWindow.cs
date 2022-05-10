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
using SMHEditor.DockingModules.MapEditor;
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
#if HALFD
            project = new ModProject("C:/Users/halfd/Downloads/testproj/testproj/project.hwproj");
#endif
#if STUMPY
            project = new ModProject("C:/Users/jaken/Desktop/testproj/project.hwproj");
#endif
            project.Save();
        }

        //Project stuff
        public static ModProject project;
        public static ViewportPage vp;

        
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

            // static map editor page.
            vp = new ViewportPage();
            dockingManager.AddToWorkspace(WORKSPACE_NAME, new KryptonPage[] { vp });

            dockingManager.AddDockspace(DOCKINGCONTROLLER_NAME, DockingEdge.Left, new KryptonPage[] {
                new ProjectExplorerPage("\\data", "Data"),
                new ProjectExplorerPage("\\art",  "Art") });


            TerrainFile t = TerrainFile.Create(TerrainFile.TerrainSize.Medium1024);
            //YAXLib.YAXSerializer s = new YAXLib.YAXSerializer(typeof(TerrainFile));
            //s.SerializeToFile(t, "C:\\Users\\jaken\\Desktop\\testproj\\data\\MyMap\\map.trn");
            MapEditorScene scn = new MapEditorScene();
            scn.LoadFile(t);
            vp.SetScene(scn);
            //scn.ExportXTD("C:\\Users\\jaken\\Desktop\\out.xtd");
        }
    }
}
