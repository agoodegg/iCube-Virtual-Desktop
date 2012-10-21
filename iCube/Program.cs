using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace iCube
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "MyApplicationName", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    string[] arguments = new string[1];
                    arguments[0] = "--auto";

                    Form1 f = new Form1(args);
                    f.FormClosed += delegate { Application.Exit(); };
                    f.ShowWindow();

                    Application.Run();
                }
                else
                {
                    MessageBox.Show("iCube已经运行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
