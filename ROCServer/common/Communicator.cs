using OBSWebsocketDotNet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace ROCServer
{
    class Communicator
    {
        MsgHandler msgHandler;
        Logger logger;
        TcpClient client;
        TcpListener server;
        OBSWebsocket ows;

        string ipAddr;
        int port;
        int rcDelay;
        bool isRunning;

        /// <summary>
        /// Basic constructor
        /// </summary>
        public Communicator(OBSWebsocket _ows, Logger _logger, string _ipAddr, int _port, int _rcDelay)
        {
            msgHandler = new MsgHandler(_ows);
            logger = _logger;
            ows = _ows;

            ipAddr = _ipAddr;
            port = _port;

            rcDelay = _rcDelay;

            isRunning = true;
        }

        public void Start()
        {
            server = null;
            try
            {
                //Create connection information for TcpListener
                IPAddress localAddr = IPAddress.Parse(ipAddr);

                //Initialize TcpListener
                server = new TcpListener(localAddr, port); //Eventually 'localAddr' will be the address TP exists at

                //Start listening for client requests
                server.Start();
                logger.WriteLine(String.Format("Server started @ -> {0}:{1}",ipAddr,port));

                //Buffer read data
                Byte[] bytes = new Byte[256];
                string data = null;

                //Enter listening loop
                while (isRunning)
                {
                    logger.WriteLine("Waiting for a client connection...");

                    //Wait for client request to come in and accept it
                    client = server.AcceptTcpClient();
                    logger.WriteLine("Connected!");

                    data = null;

                    //Create stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    while((i = stream.Read(bytes, 0 , bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        logger.WriteLine(String.Format("Received: {0}", data));

                        while (!ows.IsConnected)
                        {
                            ows.Disconnect();
                            try
                            {
                                ows.Connect(String.Format("ws://{0}:{1}", Program.ipAddr, Program.port), Program.wsPw);
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLine("Failed to connect to local OBS instance.");
                                logger.WriteLine(ex.ToString());
                            }
                            if (!ows.IsConnected)
                            {
                                logger.WriteLine(String.Format("Waiting {0} ms before attempting reconnect", rcDelay.ToString()));
                                Thread.Sleep(rcDelay);
                            }
                        }

                        try
                        {
                            data = msgHandler.ReadMessage(data);
                        }
                        catch
                        {
                            data = "1111:REQUEST_FAILED";
                        }
                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        //Send back a response
                        stream.Write(msg, 0, msg.Length);
                        logger.WriteLine(String.Format("Send: {0}", data));
                    }

                    client.Close();
                }
            }
            catch(Exception ex)
            {
                logger.WriteLine(String.Format("Exception: {0}", ex.ToString()));
            }

            logger.WriteLine("Listener finished.");
        }

        /// <summary>
        /// Close up any communications 
        /// </summary>
        public void Close()
        {
            isRunning = false;
            server.Stop();
            client.Close();
        }
    }
}
