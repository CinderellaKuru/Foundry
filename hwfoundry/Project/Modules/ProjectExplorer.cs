using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Foundry.Project.ModProject;
using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree.NodeControls;
using Aga.Controls.Tree;
using Foundry.Project;

namespace Foundry.DockingModules.ProjectExplorer
{
    public partial class ProjectExplorer : DockContent
    {
        public static void InitImageList()
        {
        }
        
        ContextMenu contextMenu;
        TreeModel model;
        NodeTextBox textBox;
        NodeStateIcon stateIcon;
        public ProjectExplorer()
        {
            contextMenu = new ContextMenu();
            ContextMenu = contextMenu;
            contextMenu.Popup += OnContextMenuPopup;
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
            treeViewAdv.NodeMouseClick += OnClicked;
        }

        private void OnContextMenuPopup(object o, EventArgs e)
        {
            contextMenu.MenuItems.Clear();


        }
        private void OnClicked(object o, TreeNodeAdvMouseEventArgs e)
        {
            EntryNodeData end = (EntryNodeData)e.Node.Tag;
            Program.window.project.DirSelectFile(end.FullPath);
        }
        private void OnDoubleClicked(object o, TreeNodeAdvMouseEventArgs e)
        {
            EntryNodeData end = (EntryNodeData)e.Node.Tag;
            Program.window.project.DirOpenFile(end.FullPath, end.SubName);
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
