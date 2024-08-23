
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerLauncher
{
    static class Program
    {
        public static bool AutoStart { get; private set; } = false;
        /// <summary>
        /// Main entry point of the application.
        /// </ summary>
        [STAThread]
        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                if (arg.ToLower().Equals("\\autostart"))
                {
                    AutoStart = true;
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
