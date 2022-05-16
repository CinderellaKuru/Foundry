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
using SMHEditor.Project;
using static SMHEditor.Project.ModProject;

namespace SMHEditor.DockingModules.ProjectExplorer
{
    //
    // [STMP] - Used to explore the project. 
    //          This class has the power to make edits to the project's files.
    //
    public partial class ProjectExplorerControl : UserControl
    {
        // TreeViews use image lists and indexes for sprites, and "lucky" for me it uses and ImageList, which cant be initialized.
        // InitImageList() is called from Program.cs before anything else.
        private static ImageList ImageList = new ImageList();
        public static void InitImageList()
        {
            ImageList.Images.Add(Properties.Resources.box);
            ImageList.Images.Add(Properties.Resources.folder);
            ImageList.Images.Add(Properties.Resources.cog);
            ImageList.Images.Add(Properties.Resources.page_white);
        }
        public static Dictionary<string, int> ImageIndex = new Dictionary<string, int>()
        {
            {"", 3}, //fallback
            {"\\data", 0},
            {"\\art", 0},
            {"folder", 1},
            {"object", 3},
            {"scenario", 2},
        };

        string rootDir;
        string pageName;
        public ProjectExplorerControl(string dir, string name)
        {
            InitializeComponent();

            rootDir = dir;
            pageName = name;

            treeView.ImageList = ImageList;

            //context menu events
            ctx_addfolder.Click += new EventHandler(ctx_addfolder_Click);
            ctx_rename.Click += new EventHandler(ctx_rename_Click);

            treeView.TreeView.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(node_doubleclicked);
            treeView.TreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(node_clicked);
            treeView.TreeView.AfterLabelEdit += new NodeLabelEditEventHandler(node_renamed);
            treeView.TreeView.AfterExpand += new TreeViewEventHandler(node_expanded);
            treeView.TreeView.AfterCollapse += new TreeViewEventHandler(node_collapsed);
        }

        // expand/colapse
        void node_expanded(object o, TreeViewEventArgs e)
        {
            try
            {
                MainWindow.project.projectData.folderData[e.Node.Name].folded = false;
            }
            catch { }
        }
        void node_collapsed(object o, TreeViewEventArgs e)
        {
            try
            {
                MainWindow.project.projectData.folderData[e.Node.Name].folded = true;
            }
            catch { }
        }

        // addfolder
        KryptonContextMenuItem ctx_addfolder = new KryptonContextMenuItem("New Folder");
        void ctx_addfolder_Click(object o, EventArgs e)
        {
            if(treeView.SelectedNode != null)
            {
                TreeNode selected = treeView.SelectedNode;
                selected.Expand();

                KryptonTreeNode newNode = new KryptonTreeNode("");
                newNode.Name = selected.Name + "\\New Folder";
                newNode.Text = "New Folder";
                newNode.Tag = FileType.FOLDER;

                selected.Nodes.Add(newNode);

                newNode.BeginEdit();
            }
        }
        // rename
        KryptonContextMenuItem ctx_rename = new KryptonContextMenuItem("Rename");
        void ctx_rename_Click(object o, EventArgs e)
        {
            if(treeView.SelectedNode != null)
            {
                treeView.SelectedNode.BeginEdit();
            }
        }

        // addfolder and rename
        void node_renamed(object o, NodeLabelEditEventArgs e)
        {
            if (e.Label == null) return;
            if (e.Node.Text == e.Label) return;

            MainWindow.project.RenameNode((KryptonTreeNode)e.Node, e.Label);
        }

        void node_clicked(object o, TreeNodeMouseClickEventArgs a)
        {
            if (a.Button == MouseButtons.Right)
            {
                KryptonContextMenu kcm = new KryptonContextMenu();
                if ((FileType)a.Node.Tag == FileType.FOLDER) //is not a file, show "add" context menu.
                {
                    KryptonContextMenuItem add = new KryptonContextMenuItem("Add");
                    add.Items.Add(ctx_addfolder);

                    kcm.Items.Add(add);
                    kcm.Items.Add(ctx_rename);

                    kcm.Show(o, ((Control)o).PointToScreen(a.Location));
                }
            }
        }
        void node_doubleclicked(object o, TreeNodeMouseClickEventArgs a)
        {
                    if(a.Node.Tag != null &&
            ((FileType)a.Node.Tag) != FileType.FOLDER)
                    MainWindow.project.OpenFile(a.Node.Name);
        }
    }
}
