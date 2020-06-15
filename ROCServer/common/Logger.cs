using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ROCServer
{
    class Logger
    {
        string logsPath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\";
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

#if DEBUG
            Debug.WriteLine(message);
#else
            writer.WriteLine(String.Format("[{0}]: {1}", dateTime.ToString("yyyyMMddTHH:mm:ssZ"), message));
            writer.Flush();            
#endif
        }
    }
}
