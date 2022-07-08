using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkUI.Docking;
using static SMHEditor.Project.ModProject;
using DarkUI.Controls;
using WeifenLuo.WinFormsUI.Docking;
using BrightIdeasSoftware;
using Aga.Controls.Tree.NodeControls;
using Aga.Controls.Tree;
using SMHEditor.Project;

namespace SMHEditor.DockingModules.ProjectExplorer
{
    public partial class ProjectExplorer : DockContent
    {
        // InitImageList() is called from Program.cs before anything else.
        private static ImageList ImageList = new ImageList();
        public static void InitImageList()
        {
        }

        ModProject proj;
        ContextMenu contextMenu;
        TreeModel model;
        NodeTextBox textBox;
        NodeStateIcon stateIcon;
        public ProjectExplorer(ModProject proj)
        {
            this.proj = proj;

            contextMenu = new ContextMenu();
            ContextMenu = contextMenu;
            contextMenu.Popup += OnPopup;
            InitializeComponent();

            model = new TreeModel();

            stateIcon = new NodeStateIcon();
            stateIcon.DataPropertyName = "Image";

            textBox = new NodeTextBox();
            textBox.DataPropertyName = "Text";
            textBox.EditEnabled = false;
            textBox.IncrementalSearchEnabled = true;
            textBox.LeftMargin = 3;

            treeViewAdv.Model = model;
            treeViewAdv.NodeControls.Add(stateIcon);
            treeViewAdv.NodeControls.Add(textBox);
            treeViewAdv.NodeMouseDoubleClick += OnDoubleClicked;
        }

        private void OnPopup(object o, EventArgs e)
        {
            contextMenu.MenuItems.Clear();


        }
        private void OnDoubleClicked(object o, TreeNodeAdvMouseEventArgs e)
        {
            EntryNodeData end = (EntryNodeData)e.Node.Tag;
            proj.DirOpenFile(end.FullPath, end.SubName);
        }

        public void UpdateHierarchy(IEnumerable<EntryNodeData> rootNodes)
        {
            model.Nodes.Clear();

            treeViewAdv.BeginUpdate();

            foreach (Node n in rootNodes)
                model.Nodes.Add(n);

            treeViewAdv.EndUpdate();
            treeViewAdv.ExpandAll(); //replace with cached fold info.
            treeViewAdv.FullUpdate();
        }
    }
}
