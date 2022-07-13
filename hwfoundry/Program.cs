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
        public static Foundry window;

        [STAThread]
        static void Main()
        {
            ProjectExplorer.InitImageList();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new Foundry();
            Application.Run(window);
        }
    }
}
