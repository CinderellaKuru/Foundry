using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.DockingModules.ProjectExplorer
{
    class ProjectExplorerPage : KryptonPage
    {
        public ProjectExplorerPage()
        {
            ProjectExplorerControl pec = new ProjectExplorerControl();

            //Control options
            pec.Dock = System.Windows.Forms.DockStyle.Fill;

            //Page options
            Name = "Explorer";
            TextTitle = "Explorer";

            Controls.Add(pec);

            //pec.treeView.Nodes.Add(MainWindow.project.vfs.Root().GetTreeView());
        }
    }
}
