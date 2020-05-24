using OBSWebsocketDotNet;
using ROCServer.common;
using System;
using System.Threading;

namespace ROCServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger();

            //Inform user the server is starting
            logger.WriteLine("******Starting ROC Server******");

            //Get ipAddress and port from config file
            Config config = new Config(logger);
            string ipAddr = config.GetAddr();
            int port = config.GetPort();
            int rcDelay = config.GetRcDelay();
            string wsAddr = config.GetOwsAddr();
            string wsPort = config.GetOwsPort();
            string wsPw = config.GetOwsPw();

            //Initialize OBSWebSocket Connection
            OBSWebsocket ows = new OBSWebsocket();
            obsConnect(ows, wsAddr, wsPort, wsPw, rcDelay, logger);

            //Initialize and start comms
            Communicator comm = new Communicator(ows, logger, ipAddr, port, rcDelay);
            comm.Start();

            // Add a flag to determine if the application is closing.
            // Once done, we will uncomment out this code to finalize closing the app.
            //if(ows.IsConnected)
            //    ows.Disconnect();
            
            //Inform the user the server is closing
            logger.WriteLine("******Closing ROC Server******");
        }

        static void obsConnect(OBSWebsocket ows, string wsAddr, string wsPort, string wsPw, int rcDelay, Logger logger)
        {
            bool connected = false;

            Thread connect = new Thread(() =>
            {
                while (!connected)
                {
                    ows.Connect(String.Format("ws://{0}:{1}", wsAddr, wsPort), wsPw);
                    connected = ows.IsConnected;

                    if (!connected)
                    {
                        logger.WriteLine(String.Format("Unable to connect to OBS. \nAttempting again in {0} ms...", rcDelay));
                        Thread.Sleep(rcDelay);
                    }
                    else
                    {
                        logger.WriteLine(String.Format("Connection established to OBS"));
                    }
                }
            });

            connect.Start();
        }
    }
}
