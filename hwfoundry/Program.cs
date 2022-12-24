using Foundry.Project.Modules.ScenarioEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry
{
    internal static class Program
    {
        public static FoundryInstance window;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new FoundryInstance();
            Application.Run(window);
        }
    }
}
