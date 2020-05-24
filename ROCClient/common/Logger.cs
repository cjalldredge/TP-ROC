using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ROCClient
{
    class Logger
    {
        StreamWriter file;
        FileInfo info;

        string logsPath;
        string logsFile = "roc_logs.txt";

        public Logger()
        {
            // Update the location of the logs folder based on if we are in debug mode
            if (Program.debug)
                logsPath = Environment.CurrentDirectory + "\\logs\\";
            else
                logsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                                "\\TouchPortal\\plugins\\roc\\logs\\";

            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);

            file = new StreamWriter(logsPath + logsFile);
            info = new FileInfo(logsPath + logsFile);
        }

        public void Write(string message)
        {
            if (info.Length > 40000)
            {
                file.Dispose();
                File.Delete(logsPath + logsFile);
                file = new StreamWriter(logsPath + logsFile);
            }

            var dateTime = DateTime.Now;

            file.WriteLine(String.Format("[{0}]: {1}",dateTime.ToString("yyyyMMddTHH:mm:ssZ"), message));
            file.Flush();
        }
    }
}
