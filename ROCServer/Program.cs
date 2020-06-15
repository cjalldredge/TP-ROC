using OBSWebsocketDotNet;
using ROCServer.common;
using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading;

namespace ROCServer
{
    class Program
    {
        public static string ipAddr;
        public static int port;
        public static string wsPw;
        public static int rcDelay;
        public static string wsAddr;
        public static string wsPort;

        static OBSWebsocket ows;
        static Communicator comm;
        static Logger logger;
        static BackgroundWorker worker;
        static ROCService rocService;



        static void Main(string[] args)
        {
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
#if DEBUG
            Start();
#else
            rocService = new ROCService();
            ServiceBase.Run(rocService);
#endif
        }

        public static void Start()
        {
            // onstart code here
            logger = new Logger();

            //Inform user the server is starting
            logger.WriteLine("******Starting ROC Server******");

            //Get ipAddress and port from config file
            Config config = new Config(logger);
            ipAddr = config.GetAddr();
            port = config.GetPort();
            rcDelay = config.GetRcDelay();
            wsAddr = config.GetOwsAddr();
            wsPort = config.GetOwsPort();
            wsPw = config.GetOwsPw();

            worker.RunWorkerAsync();
#if DEBUG
            while (worker.IsBusy)
            {
                //logger.WriteLine("Worker is busy.");
                Thread.Sleep(1000);
            }
#endif
        }

        public static void Stop()
        {
            worker.CancelAsync();
            Thread.Sleep(1000);
            
            //Inform the user the server is closing
            logger.WriteLine("******Closing ROC Server******");
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.WriteLine("Background worker started...");
            try
            {
                ows = new OBSWebsocket();
                comm = new Communicator(ows, logger, ipAddr, port, rcDelay);

                //Initialize OBSWebSocket Connection
                logger.WriteLine("Starting communications to local OBS instance.");
                obsConnect(ows, wsAddr, wsPort, wsPw, rcDelay, logger);

                //Initialize and start comms
                logger.WriteLine("Opening TCP communications for clients.");
                comm.Start();
            }
            catch
            {
                logger.WriteLine("Background worker started but ran in to an issue.");
            }
        }

        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logger.WriteLine("Background worker completed...");
        }

        static void obsConnect(OBSWebsocket ows, string wsAddr, string wsPort, string wsPw, int rcDelay, Logger logger)
        {
            bool connected = false;

            Thread connect = new Thread(() =>
            {
                while (!connected)
                {
                    try
                    {
                        ows.Connect(String.Format("ws://{0}:{1}", wsAddr, wsPort), wsPw);
                        connected = ows.IsConnected;
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLine(String.Format("Error while connecting to OBS websocket {0}", ex.ToString()));
                    }

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
