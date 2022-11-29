namespace hwFoundry
{
    internal static class Program
    {
        public static GUI.MainWindow? mainWindow;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainWindow = new();
            Application.Run(mainWindow);
        }
    }
}