using System;
using System.Reflection;
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
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                Assembly thisAssembly = Assembly.GetEntryAssembly();
                String resourceName = string.Format("{0}.{1}.dll", thisAssembly.EntryPoint.DeclaringType.Namespace, new AssemblyName(args.Name).Name);
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Global.Instance.mainForm);     //form will be initialized as globally
          //  Application.Run(GtipForm.Instance.gtipForm);          //for singleton form calling
        }
    }
}
