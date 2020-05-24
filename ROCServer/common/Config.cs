using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ROCServer.common
{
    class Config
    {
        string configPath = Environment.CurrentDirectory + "\\config.xml";

        Logger logger;
        XmlDocument doc;

        public Config(Logger _logger)
        {
            logger = _logger;
            try
            {
                doc = new XmlDocument();
                doc.Load(configPath);

                logger.WriteLine("Config file loaded.");
            }
            catch
            {
                logger.WriteLine("Unable to find the config file.");
            }

        }

        public string GetAddr()
        {
            string ipAddr = "0.0.0.0"; //Default ipaddress

            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("ipAddress");
                ipAddr = node.Attributes["value"].Value;

                logger.WriteLine(String.Format("Applying {0} as the OBS IPAddress.", ipAddr));
            }
            catch
            {
                logger.WriteLine(String.Format("Applying default value {0} as OBS IPAddress", ipAddr));
            }

            return ipAddr;
        }

        public int GetPort()
        {
            int port = 6567;

            try
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("port");
                port = Convert.ToInt32(node.Attributes["value"].Value);

                logger.WriteLine(String.Format("Applying {0} as the OBS Port", port));
            }
            catch
            {
                logger.WriteLine(String.Format("Applying default of {0} as OBS Port", port));
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

                logger.WriteLine(String.Format("Applying {0} as OBS reconnect delay.", rcDelay));
            }
            catch
            {
                logger.WriteLine(String.Format("Applying default of {0} as OBS reconnect delay.", rcDelay));
            }

            return rcDelay;
        }

        public string GetOwsAddr()
        {
            string owsAddr = "0.0.0.0";

            try
            {
                owsAddr = GetAttrStringValue("obsWsAddress");
                logger.WriteLine("Setting OBS WebSocket address to " + owsAddr);
            }
            catch
            {
                logger.WriteLine("Defaulting OBS WebSocket address to " + owsAddr);
            }

            return owsAddr;
        }

        public string GetOwsPort()
        {
            string owsPort = "4444";

            try
            {
                owsPort = GetAttrStringValue("obsWsPort");
                logger.WriteLine("Setting OBS WebSocket port to " + owsPort);
            }
            catch
            {
                logger.WriteLine("Defaulting OBS WebSocket port to " + owsPort);
            }

            return owsPort;
        }

        public string GetOwsPw()
        {
            string owsPw = "";

            try
            {
                owsPw = GetAttrStringValue("obsWsPort");
                logger.WriteLine("Setting OBS WebSocket password to configured value.");
            }
            catch
            {
                logger.WriteLine("Defaulting OBS WebSocket password.");
            }

            return owsPw;
        }

        private string GetAttrStringValue(string nodeName)
        {
            XmlNode node = doc.DocumentElement.SelectSingleNode(nodeName);
            return node.Attributes["value"].Value;
        }
    }
}
