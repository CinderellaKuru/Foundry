using SharpDX.Direct2D1;
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

namespace foundry.unit
{
    public partial class UnitPickerPage : BaseToolPage
    {
        private TreeView treeview;

        public UnitModule Module { get; private set; }
        public UnitPickerPage(UnitModule module)
        {
            Module = module;
            Module.ModuleUpdated += (sender, e) =>
            {
                UpdateView();
            };


            UnitContextMenu = new ContextMenuStrip();
            foreach (ToolStripMenuItem item in Module.Operators_UnitRightClicked.GetRootMenuItems())
            {
                UnitContextMenu.Items.Add(item);
            }


            treeview = new TreeView();
            treeview.Dock = DockStyle.Fill;
            treeview.NodeMouseClick += (sender, e) =>
            {
                treeview.SelectedNode = e.Node;

                string selected = (string)treeview.SelectedNode.Tag;
                if(selected != null)
                {
                    Module.SetSelectedUnit(selected);

                    if (e.Button == MouseButtons.Right)
                    {
                        UnitContextMenu.Show(this, e.Location);
                    }

                }
            };

            Text = "Unit Picker";
            Controls.Add(treeview);
        }
        ~UnitPickerPage()
        {
            Module.UnitPickers.Remove(this);
        }


        private ContextMenuStrip UnitContextMenu;
        private Dictionary<string, TreeNode> UnitNodes = new Dictionary<string, TreeNode>();
        public void UpdateView()
        {
            treeview.Nodes.Clear();
            UnitNodes.Clear();

            TreeNode root = new TreeNode("Units");
            treeview.Nodes.Add(root);

            foreach (UnitModule.Unit unit in Module.Units.Values)
            {
                TreeNode last = root;
                foreach (string group in unit.EditorData.Group)
                {
                    if (last.Nodes.ContainsKey(group))
                    {
                        last = last.Nodes[group];
                    }
                    else
                    {
                        TreeNode node = new TreeNode();
                        node.Name = group; //key
                        node.Text = group; //text
                        last.Nodes.Add(node);
                        last = node;
                    }
                }
                TreeNode unitNode = new TreeNode(unit.Name);

                unitNode.Tag = unit.Name;
                last.Nodes.Add(unitNode);
                UnitNodes.Add(unit.Name, unitNode);
            }

            treeview.SelectedNode = root;
            root.Expand();
        }
    }
}
