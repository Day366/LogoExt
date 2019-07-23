using System;
using System.IO;

namespace LogoExt
{
    public class LogWriter
    {
        private static readonly LogWriter instance = new LogWriter();

        static LogWriter()
        {
        }

        private LogWriter()
        {
        }

        public static LogWriter Instance
        {
            get {return instance;}
        }

        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }

        public void LogWrite(string logMessage)
        {
            try
            {
                using (StreamWriter w = File.AppendText(Global.exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
