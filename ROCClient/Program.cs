using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ROCClient
{
    class Program
    {
        static Logger logger;

        public static TPComm tpComm;
        static ROCServComm rocServ;

        public static bool isRunning = true;
        public static bool debug = false;

        public static List<string> connections = new List<string>(); // List of ROC endpoints that responded
        public static List<ROCServer> rocServs = new List<ROCServer>(); // List of ROC Servers to communicate to

        static void Main(string[] args)
        {
            logger = new Logger();
            logger.Write("******Remote OBS Controller Started******");

            //Read config for server ip and port values
            Config config = new Config(logger);

            // Get connection information from config file
            string ipAddr = config.GetAddr();
            int port = config.GetPort();
            int rcDelay = config.GetRcDelay();

            //Start communications with ROC servers
            for (int i = 1; i <= 255; i++)
            {
                string tmpAddr = "";
                List<string> ipParts = ipAddr.Split('.').ToList();
                
                ipParts[ipParts.Count - 1] = i.ToString();
                tmpAddr = String.Format("{0}.{1}.{2}.{3}", ipParts[0], ipParts[1], ipParts[2], ipParts[3]);

                rocServs.Add(new ROCServer(tmpAddr, port, logger));
            }

            //Start the TouchPortal comms
            tpComm = new TPComm(logger, rocServs);

            //Begin StartupSequence
            StartupSequence();

            logger.Write("******Remote OBS Controller Closed******");
        }

        /// <summary>
        /// Initializes connection to ROCServ and Touch Portal. 
        /// Then it pulls a list of the available OBS scenes from the remote PC
        /// </summary>
        private static void StartupSequence()
        {
            //Send request to potential ROC Servs to connect
            foreach(ROCServer rocServer in rocServs)
            {
                ThreadPool.QueueUserWorkItem((i) =>
                {
                    if (rocServer.Connect())
                        connections.Add(rocServer.ipAddress);
                });
            }
            //for (int i = 0; i < rocServs.Count - 1; i++)
            //{
            //    ThreadPool.QueueUserWorkItem((x) =>
            //    {
            //        if (rocServs[i].Connect())
            //            connections.Add(rocServs[i].ipAddress);
            //    });
            //    Thread serverConnect = new Thread(() =>
            //    {

            //    });
            //    serverConnect.Start();
            //}

            Thread.Sleep(2000);

            tpComm.RequestPair();

            Thread.Sleep(500);

            List<string> scenes = new List<string>();

            //Get a list of scenes and pass to Touch Portal
            foreach(ROCServer server in rocServs)
                if (server.comm.isConnected)
                    foreach (string scene in server.GetScenes())
                        scenes.Add(scene);

            tpComm.UpdateScenesList(scenes);
            tpComm.UpdateServerList(connections);
        }

    }
}
