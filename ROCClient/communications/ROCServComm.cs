using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ROCClient
{
    class ROCServComm
    {
        Logger logger;
        public string ipAddr;
        int port;
        int rcDelay;
        bool wasConnected = false; // Flag to inicate if at one point we were connected
        public bool isConnected = false; //Flag to indicate if we are currently connected
        bool connectAttempt = false; //Flag to indicate we are initiating a connect request
        bool firstAttempt = true; // Flag to indicate that connect attempt is the first attempt
        bool allowReconnect = false; // Flag to indicate whether or not a reconnect is allowable
        
        /// <summary>
        /// Basic constructor
        /// </summary>
        public ROCServComm(Logger _logger, string _ipAddr, int _port, int _rcDelay)
        {
            logger = _logger;
            ipAddr = _ipAddr;
            port = _port;
            rcDelay = _rcDelay;
        }

        public void ConnectRequest()
        {
            connectAttempt = true;
            //logger.Write(String.Format("Attempting to reach {0}...", ipAddr));
            string response = Send("1000:ConnectRequest");
            if (!response.Equals("[ERROR]"))
            {
                isConnected = true;
                wasConnected = true;
                logger.Write(String.Format("[{0}]Connect Request Response: {1}", ipAddr, response));
                logger.Write(String.Format("[{0}]ROC Serv Connected: {1}", ipAddr, isConnected.ToString()));
            }
            connectAttempt = false;

            if (!isConnected && firstAttempt) // Stop trying, nobody is listening there...
            {
                firstAttempt = false;
            }
            else if (!isConnected && !firstAttempt)
            {
                if (allowReconnect)
                {
                    logger.Write(String.Format("Waiting {0} ms before next attempt...", rcDelay));
                    Thread.Sleep(rcDelay);
                }
            }

            //Thread thread = new Thread(() =>
            //{
            //    while (!isConnected && Program.isRunning && !exitConnectRequest)
            //    {
            //        connectAttempt = true;
            //        //logger.Write(String.Format("Attempting to reach {0}...", ipAddr));
            //        string response = Send("1000:ConnectRequest");
            //        if (!response.Equals("[ERROR]"))
            //        {
            //            isConnected = true;
            //            wasConnected = true;
            //            logger.Write(String.Format("[{0}]Connect Request Response: {1}", ipAddr, response));
            //            logger.Write(String.Format("[{0}]ROC Serv Connected: {1}", ipAddr, isConnected.ToString()));
            //        }
            //        connectAttempt = false;

            //        if (!isConnected && firstAttempt) // Stop trying, nobody is listening there...
            //        {

            //            firstAttempt = false;
            //            exitConnectRequest = true;
            //            break;
            //        }
            //        else if (!isConnected && !firstAttempt)
            //        {
            //            if (allowReconnect)
            //            {
            //                logger.Write(String.Format("Waiting {0} ms before next attempt...", rcDelay));
            //                Thread.Sleep(rcDelay);
            //            }
            //        }
            //    }
            //});

            //if (firstAttempt)
            //{
            //    thread.Start();
            //    firstAttempt = false;
            //}
        }

        public string Send(string message)
        {
            if(isConnected || connectAttempt)
            {
                try
                {
                    //Create connection information to server
                    //Int32 port = 6567;
                    string server = ipAddr; //Temp server address for testing
                    TcpClient client = new TcpClient(server, port);

                    // Set the timeout on TCP Client communications
                    client.ReceiveTimeout = 500;

                    //Translate message to bytes
                    Byte[] data = Encoding.ASCII.GetBytes(message);

                    //Get a client stream for reading and writing
                    NetworkStream stream = client.GetStream();

                    //Send message to TcpServer
                    stream.Write(data, 0, data.Length);

                    //Inform user the data has been sent
                    logger.Write(String.Format("Sent: {0}", message));

                    //Be ready to receive a response from the server
                    data = new Byte[256];

                    //String to store the data received
                    string responseData = string.Empty;

                    //Read the first batch of data
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    logger.Write(String.Format("Received: {0}", responseData));

                    stream.Close();
                    client.Close();

                    return responseData;
                }
                catch (SocketException ex)
                {
                    if(ex.SocketErrorCode == SocketError.ConnectionRefused && !connectAttempt && !firstAttempt)
                    {
                        logger.Write(String.Format("{0}: ROCServ host refused connection.", ipAddr));
                        if (allowReconnect)
                        {
                            logger.Write("Attempting to reconnect...");
                            isConnected = false;
                            ConnectRequest();
                        }
                    }
                    else if(!connectAttempt && !firstAttempt)
                    {
                        logger.Write(String.Format("{0}: Unable to reach ROCServ.", ipAddr));
                        if (allowReconnect)
                        {
                            logger.Write(ex.ToString());
                            logger.Write("Attempting to reconnect...");
                            ConnectRequest();
                        }
                    }
                }
            }
            return "[ERROR]";
        }
    }
}
