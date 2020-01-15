using System;
using System.Windows.Forms;

namespace AwakeningItemTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ItemNameUtility.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AwakeningItemToolForm());
        }
    }
}
