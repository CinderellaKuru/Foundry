using SMHEditor.DockingModules.ProjectExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor
{
    internal static class Program
    {
        public static MainWindow window;

        [STAThread]
        static void Main()
        {
            ProjectExplorerControl.InitImageList();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new MainWindow();
            Application.Run(window);
        }
    }
}
