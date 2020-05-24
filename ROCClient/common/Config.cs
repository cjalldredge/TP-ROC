using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ROCClient
{
    class Config
    {
        string configPath;
        public bool configExists;

        Logger logger;
        XmlDocument doc;

        /// <summary>
        /// Basic constructor
        /// </summary>
        public Config(Logger _logger)
        {
            if (Program.debug)
            {
                configPath = Environment.CurrentDirectory + "\\config.xml";
            }
            else
            {
                configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                            "\\TouchPortal\\plugins\\roc\\config.xml";
            }

            logger = _logger;
            if (File.Exists(configPath))
            {
                logger.Write("Reading config file...");
                try
                {
                    doc = new XmlDocument();
                    doc.Load(configPath);
                    logger.Write("Config file loaded.");
                    configExists = true;
                }
                catch
                {
                    logger.Write("Unable to find config file.");
                    configExists = false;
                }
            }
        }

        //Get the address of the server
        public string GetAddr()
        {
            string ipAddr = "0.0.0.0";

            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("ipAddress");
                ipAddr = node.Attributes["value"].Value;
                logger.Write(String.Format("Applying {0} as server address.", ipAddr));
            }
            catch
            {
                logger.Write("Applying default server address " + ipAddr);
            }

            return ipAddr;
        }

        //Get the port the server is listening on
        public int GetPort()
        {
            int port = 6567;

            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("port");
                port = Convert.ToInt32(node.Attributes["value"].Value);
                logger.Write(String.Format("Applying {0} as server port", port));
            }
            catch
            {
                logger.Write("Applying default server port of " + port.ToString());
            }

            return port;
        }

        public int GetRcDelay()
        {
            int rcDelay = 1000;

            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("rcDelay");
                rcDelay = Convert.ToInt32(node.Attributes["value"].Value);
                logger.Write(String.Format("Applying {0} as the server reconnect delay.", rcDelay));
            }
            catch
            {
                logger.Write("Applying default server reconnect delay of " + rcDelay.ToString());
            }

            return rcDelay;
        }
    }
}
