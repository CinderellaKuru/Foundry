using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace SMHEditor.DockingModules.ProjectExplorer
{
    //
    // [STMP] - This class has the power to make edits to the project's files.
    //
    public partial class ProjectExplorerControl : UserControl
    {
        // TreeViews use image lists and indexes for sprites, and "lucky" for me it uses and ImageList, which cant be initialized.
        // InitImageList() is called from Program.cs before anything else.
        private static ImageList ImageList = new ImageList();
        public static void InitImageList()
        {
            ImageList.Images.Add(Properties.Resources.folder);
            ImageList.Images.Add(Properties.Resources.folder_page_white);
        }
        public static Dictionary<string, int> ImageIndex = new Dictionary<string, int>()
        {
            {"folder", 0},
            {"folder_page", 1},
        };

        // Context menu items and their handlers. Returns root items.
        public KryptonContextMenuItems BuildFolderContextMenu()
        {
            root_add = new KryptonContextMenuItem("Add");
            add_folder = new KryptonContextMenuItem("New Folder");
            add_folder.Click += new EventHandler(add_folder_Click);


            root_add.Items.Add(new KryptonContextMenuItems(new KryptonContextMenuItem[] { add_folder }));

            return new KryptonContextMenuItems(
                new KryptonContextMenuItem[] {
                    root_add
                });
        }

        // Folder/Root context menu
        // Shown when right clicking on the explorer itself, or a folder.
        // Always add new context menu items here when adding a new project content type.
        KryptonContextMenu folderMenu;
        KryptonContextMenuItem root_add;
        KryptonContextMenuItem add_folder;
        void add_folder_Click(object o, EventArgs e)
        {

        }

        public ProjectExplorerControl()
        {
            InitializeComponent();

            //Set up folder menu and add to root view.
            folderMenu = new KryptonContextMenu();


            folderMenu.Items.Add(BuildFolderContextMenu());
            treeView.KryptonContextMenu = folderMenu;

            //Temp root tree node. Replace later with project name.
            //treeView.Nodes.Add("Project");
            treeView.ImageList = ProjectExplorerControl.ImageList;

            treeView.Click += new EventHandler(Test);
        }

        void Test(object o, EventArgs e)
        {
            treeView.SelectedNode = null;
            treeView.Focus();
            Console.WriteLine("A");
        }
    }
}
