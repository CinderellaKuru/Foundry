using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using hwFoundry.Project;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace hwFoundry.GUI
{
    public partial class ProjectExplorer : DockContent
    {

        TreeModel treeModel;
        NodeStateIcon nodeStateIcon;
        NodeTextBox nodeTextBox;

        public ProjectExplorer()
        {
            InitializeComponent();

            // Set Theme
            //treeViewAdv.BackColor = Color.FromArgb(45, 45, 48);
            //treeViewAdv.ForeColor = Color.White;

            treeModel = new();

            nodeStateIcon = new()
            {
                DataPropertyName = "Image"
            };

            nodeTextBox = new()
            {
                DataPropertyName = "Text",
                EditEnabled = false,
                IncrementalSearchEnabled = true,
                LeftMargin = 3
            };

            treeViewAdv.Model = treeModel;
            treeViewAdv.NodeControls.Add(nodeStateIcon);
            treeViewAdv.NodeControls.Add(nodeTextBox);
            treeViewAdv.NodeMouseDoubleClick += OnDoubleClicked;
            treeViewAdv.NodeMouseClick += OnClicked;
        }

        #region Events
        private void OnClicked(object? sender, TreeNodeAdvMouseEventArgs e)
        {
            EntryNode selectedNode = (EntryNode)e.Node.Tag;
            Program.mainWindow.modProject.DirSelectFile(selectedNode.FullPath);
        }

        private void OnDoubleClicked(object? sender, TreeNodeAdvMouseEventArgs e)
        {
            EntryNode selectedNode = (EntryNode)e.Node.Tag;
            Program.mainWindow.modProject.DirOpenFile(selectedNode.FullPath, selectedNode.SubName);
        }
        #endregion

        public void UpdateProjectHierarchy(IEnumerable<EntryNode> rootNodes)
        {
            treeModel.Nodes.Clear();

            treeViewAdv.BeginUpdate();
            foreach (Node n in rootNodes)
                treeModel.Nodes.Add(n);

            treeViewAdv.EndUpdate();
            treeViewAdv.ExpandAll(); // replace with cached fold info.
            treeViewAdv.FullUpdate();
        }

        private void AddNewTriggerScriptProject(object sender, EventArgs e)
        {

        }
    }
}
