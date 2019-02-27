using System;
using System.Windows.Forms;

namespace LogoExt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Global.Instance.mainForm);     //form will be initialized as globally
          //  Application.Run(Form1.Instance.form1);          //for singleton form calling
        }
    }
}
