using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ROCServer
{
    class Logger
    {
        string logsPath = Environment.CurrentDirectory + "\\logs\\";
        string logsFile = "roc_server_logs.txt";

        StreamWriter writer;

        public Logger()
        {
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            writer = new StreamWriter(logsPath + logsFile);
        }

        public void WriteLine(string message)
        {
            var dateTime = DateTime.Now;

            writer.WriteLine(String.Format("[{0}]: {1}", dateTime.ToString("yyyyMMddTHH:mm:ssZ"), message));
            writer.Flush();
        }
    }
}
