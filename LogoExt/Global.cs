using System.IO;
using System.Reflection;

namespace LogoExt
{
    class Global
    {
        private static Global GlobalVariableInstance = null;

        public Global()
        {
        }

        public static Global Instance
        {
            get
            {
                if (GlobalVariableInstance == null)
                {
                    GlobalVariableInstance = new Global();
                }
                return GlobalVariableInstance;
            }
        }

        public static string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string firmCode = "017";

        private static string strDBUser = "tolga";                     // SQL Server kullanıcı adı
        private static string strDBPass = "1234";                      // SQL Server kullanıcısının şifresi
        private static string strDBServer = "MHSB";                    // SQL Server
        private static string strDBName = "TGER";                      // Veritabanı adı 
        public static string sqlConnection = "Data Source=localhost; Persist Security Info = False; User ID = " + strDBUser + "; Password = " + strDBPass + "; Initial Catalog = " + strDBName + "; Data Source = " + strDBServer;

        public MainForm mainForm = new MainForm();
        public Query query = new Query();


        public void ErrorNotification(string errorBody) {
            mainForm.LabelWarningBody.Text = errorBody;
            mainForm.TimerSlideIn.Enabled = true;
        }
    }
}
