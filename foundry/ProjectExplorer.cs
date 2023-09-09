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
using System.IO;
using static foundry.FoundryInstance;
using BrightIdeasSoftware;

namespace foundry
{
    public class ProjectExplorer : DockContent
    {
        private TreeListView treelist;
        private ToolStrip toolstrip;
        private ToolStripButton toolstrip_refreshbutton;
        private System.ComponentModel.IContainer components;
        private FoundryInstance instance;

        public ProjectExplorer(FoundryInstance i)
        {
            instance = i;
            InitializeComponent();
            treelist.CanExpandGetter = delegate (object o)
            {
                DiskEntryNode den = (DiskEntryNode)o;
                return den.IsFolder;
            };
            treelist.ChildrenGetter = delegate (object o)
            {
                DiskEntryNode den = (DiskEntryNode)o;
                return den.Children;
            };
            treelist.AllColumns.Add(new OLVColumn("Name", "Name"));
            treelist.RebuildColumns();
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.treelist = new BrightIdeasSoftware.TreeListView();
            this.toolstrip = new System.Windows.Forms.ToolStrip();
            this.toolstrip_refreshbutton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.treelist)).BeginInit();
            this.toolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeListView1
            // 
            this.treelist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treelist.Location = new System.Drawing.Point(0, 28);
            this.treelist.Name = "treeListView1";
            this.treelist.ShowGroups = false;
            this.treelist.Size = new System.Drawing.Size(284, 233);
            this.treelist.TabIndex = 0;
            this.treelist.View = System.Windows.Forms.View.Details;
            this.treelist.ItemActivate += new EventHandler(TreeList_Node_ItemActivate);
            // 
            // toolStrip1
            // 
            this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstrip_refreshbutton});
            this.toolstrip.Location = new System.Drawing.Point(0, 0);
            this.toolstrip.Name = "toolStrip1";
            this.toolstrip.Size = new System.Drawing.Size(284, 25);
            this.toolstrip.TabIndex = 1;
            this.toolstrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolstrip_refreshbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstrip_refreshbutton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolstrip_refreshbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_refreshbutton.Name = "toolStripButton1";
            this.toolstrip_refreshbutton.Size = new System.Drawing.Size(23, 22);
            this.toolstrip_refreshbutton.Text = "toolStripButton1";
            this.toolstrip_refreshbutton.Click += new System.EventHandler(this.ButtonRefresh_Clicked);
            // 
            // ProjectExplorer
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.toolstrip);
            this.Controls.Add(this.treelist);
            this.Name = "ProjectExplorer";
            ((System.ComponentModel.ISupportInitialize)(this.treelist)).EndInit();
            this.toolstrip.ResumeLayout(false);
            this.toolstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        class ExplorerNode : Node
        {
            public ExplorerNode(string text, string fullPath)
            {
                Text = text;
                FullPath = fullPath;
                Image = null;
            }

            public string FullPath { get; set; }
            public ref Image Icon 
            { 
                get 
                {
                    return ref defaultImage;

                    //string ext = Path.GetExtension(FullPath);

                    //if(nodeImages.ContainsKey(ext))
                    //{
                    //    return ref NodeIcons[0];
                    //}
                    //else
                    //{
                    //    return ref defaultImage;
                    //}
                }
            }
		}
		/// <summary>
		/// Only files ending with these extensions are allowed.
		/// Also (not listed here) any folder starting with a . is excluded too.
		/// </summary>
		private static List<string> allowedExtensions = new List<string>()
		{

		};
        private static Image defaultImage = Properties.Resources.page_white;
		/// <summary>
		/// The file extension of each node is compared to this map of icons.
		/// </summary>
		private static Dictionary<string, Image> nodeImages = new Dictionary<string, Image>()
        {
            //{ ExtSerializeTriggerscript,   Properties.Resources.page_white },
            //{ ExtSerializeScenario,        Properties.Resources.page_white },
        };

        private static List<Image> NodeIcons = new List<Image>()
        {

        };


        private void UpdateNodes_CreateExplorerNodeRecursive(DiskEntryNode diskNode, ExplorerNode explorerNode)
        {
			foreach (DiskEntryNode child in diskNode.Children.Where(x =>
			{
				//filter folder names starting with a dot.
				if (x.IsFolder)
					if (x.Name[0] == '.')
						return false;
				////filter out file extensions.
				//if (!x.isFolder)
				//	if (!allowedExtensions.Contains(Path.GetExtension(x.name)))
				//		return false;
				return true;
			}))
            {
                Image img = Properties.Resources.page_white;
                //if (child.isFolder)
                //    img = Properties.Resources.folder;
                //else
                //    img = nodeImages.ContainsKey(Path.GetExtension(child.path)) ? nodeImages[Path.GetExtension(child.path)] : Properties.Resources.page_white;

                ExplorerNode newNode = new ExplorerNode(child.Name, child.Path);
                UpdateNodes_CreateExplorerNodeRecursive(child, newNode);
                explorerNode.Nodes.Add(newNode);
            }
        }
        public void UpdateNodes(DiskEntryNode root)
        {
            treelist.ClearObjects();
            treelist.AddObject(instance.OpenedWorkspaceRoot);
            treelist.RebuildAll(true);
        }
        public void RefreshNodes()
        {
            instance.UpdateDirectory();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TreeList_Node_ItemActivate(object o, EventArgs e)
        {
            var item = treelist.GetItem(treelist.SelectedIndex);
        }
        private void ButtonRefresh_Clicked(object o, EventArgs e)
        {
            RefreshNodes();
        }
    }
}
