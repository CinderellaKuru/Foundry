using Aga.Controls.Tree.NodeControls;
using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;
using static Foundry.FoundryInstance;

namespace Foundry.Project.Modules
{
    class ExplorerNode : Node
    {
        public ExplorerNode(string text, string fullPath, Image image)
        {
            Text = text;
            FullPath = fullPath;
            Image = image;
        }

        public string FullPath { get; set; }
    }
    public class ProjectExplorer : FoundryPage
    {
        private ToolStrip toolStrip;
        private ToolStripButton buttonRefresh;
        private NodeIcon nodeImage;
        private NodeTextBox nodeText;
        private TreeViewAdv treeView;
        public  TreeModel treeModel;

        public ProjectExplorer(FoundryInstance i) : base(i)
        {
            Init();
        }
        private void Init()
        {
            this.treeModel = new TreeModel();
            this.nodeImage = new NodeIcon();
            this.nodeText = new NodeTextBox();
            this.toolStrip = new ToolStrip();
            this.buttonRefresh = new ToolStripButton();
            this.treeView = new TreeViewAdv();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // nodeImage
            // 
            this.nodeImage.LeftMargin = 1;
            this.nodeImage.ParentColumn = null;
            this.nodeImage.DataPropertyName = "Image";
            this.nodeImage.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeText
            // 
            this.nodeText.IncrementalSearchEnabled = true;
            this.nodeText.LeftMargin = 3;
            this.nodeText.DataPropertyName = "Text";
            this.nodeText.ParentColumn = null;
            // 
            // toolstrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonRefresh});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolstrip";
            this.toolStrip.Size = new System.Drawing.Size(284, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRefresh.Image = global::Foundry.Properties.Resources.arrow_refresh_small;
            this.buttonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonRefresh.Size = new System.Drawing.Size(23, 22);
            this.buttonRefresh.Click += new System.EventHandler(this.ButtonRefresh_Clicked);
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.SystemColors.Window;
            this.treeView.DefaultToolTipProvider = null;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Model = treeModel;
            this.treeView.Name = "treeView";
            this.treeView.NodeControls.Add(this.nodeImage);
            this.treeView.NodeControls.Add(this.nodeText);
            this.treeView.SelectedNode = null;
            this.treeView.Size = new System.Drawing.Size(284, 236);
            this.treeView.TabIndex = 2;
            this.treeView.Text = "treeview";
            this.treeView.NodeMouseClick += new EventHandler<TreeNodeAdvMouseEventArgs>(TreeViewNode_Clicked);
            this.treeView.NodeMouseDoubleClick += new EventHandler<TreeNodeAdvMouseEventArgs>(TreeViewNode_DoubleClicked);
            // 
            // ProjectExplorer
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.toolStrip);
            this.Name = "ProjectExplorer";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // nodes
        private void CreateExplorerNodeRecursive(DiskEntryNode diskNode, ExplorerNode explorerNode)
        {
            foreach(DiskEntryNode child in diskNode._children)
            {
                ExplorerNode newNode = new ExplorerNode(child._name, child._path, child._icon);
                CreateExplorerNodeRecursive(child, newNode);
                explorerNode.Nodes.Add(newNode);
            }
        }
        public void UpdateNodes(DiskEntryNode rootDiskEntryNode)
        {
            ExplorerNode rootExplorerNode = new ExplorerNode(rootDiskEntryNode._name, rootDiskEntryNode._path, rootDiskEntryNode._icon);
            CreateExplorerNodeRecursive(rootDiskEntryNode, rootExplorerNode);
            
            treeView.BeginUpdate();
            treeModel.Nodes.Clear();
            treeModel.Nodes.Add(rootExplorerNode);
            treeView.EndUpdate();

            treeView.ExpandAll(); //replace with cached fold info.
            treeView.FullUpdate();
        }
        public void RefreshNodes()
        {
            FoundryInstance owner = Instance();
            owner.UpdateAllProjectExplorers(owner.UpdateContent());
        }
        public void ClearNodes()
        {
            treeView.BeginUpdate();
            treeModel.Nodes.Clear();
            treeView.EndUpdate();
            treeView.FullUpdate();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // ui callbacks
        private void TreeViewNode_Clicked(object o, TreeNodeAdvMouseEventArgs e)
        {
            
        }
        private void TreeViewNode_DoubleClicked(object o, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Node.Tag is ExplorerNode)
            {
                Instance().ContentFileOpen(((ExplorerNode)e.Node.Tag).FullPath);
            }
        }
        private void ButtonRefresh_Clicked(object o, EventArgs e)
        {
            RefreshNodes();
        }
    }
}
