using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ProjectExplorer
{
    class NodeComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            // both names are files.
            if ( tx.Name.Contains('.')  &&  ty.Name.Contains('.'))
            {
                return string.Compare(tx.Name, ty.Name);
            }
            // both names are folders.
            if (!tx.Name.Contains('.')  && !ty.Name.Contains('.'))
            {
                return string.Compare(tx.Name, ty.Name);
            }
            // left is file, right is folder. file loses.
            if ( tx.Name.Contains('.')  && !ty.Name.Contains('.')) return  1;
            // left is folder, right is file. folder wins.
            if (!tx.Name.Contains('.')  &&  ty.Name.Contains('.')) return -1;
            return 0;
        }
    }
    class ProjectExplorerPage : KryptonPage
    {
        public ProjectExplorerPage(string rootDir, string name)
        {
            ProjectExplorerControl pec = new ProjectExplorerControl(rootDir, name);

            //Control options
            pec.Dock = System.Windows.Forms.DockStyle.Fill;
            pec.treeView.LabelEdit = true;

            //Page options
            Name = "Explorer";
            TextTitle = "Explorer";

            Controls.Add(pec);

            pec.treeView.Nodes.Add(MainWindow.project.GetTreeView(ModProject.DATA_DIR, "Data"));
           // pec.treeView.TreeViewNodeSorter = new NodeComparer();
           // pec.treeView.Sort();
        }
    }
}
