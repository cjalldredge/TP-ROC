using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using RemoteOBSController;
using ROCClient.common.messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ROCClient
{
    class TPComm
    {
        Logger logger;
        MsgHandler msgHandler;
        List<ROCServer> rocServs;
        Socket sender;

        bool isPaired = false;

        /// <summary>
        /// Basic Constructor
        /// </summary>
        public TPComm(Logger _logger, List<ROCServer> _rocServs)
        {
            logger = _logger;
            rocServs = _rocServs;
            msgHandler = new MsgHandler(rocServs, logger);
        }

        public void RequestPair()
        {
            //TP Socet Information
            string ipAddress = "127.0.0.1";
            Int32 port = 12136;

            try
            {
                IPAddress ipAddr = IPAddress.Parse(ipAddress);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

                sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    //Attempt connection to TouchPortal
                    sender.Connect(localEndPoint);
                    logger.Write(String.Format("Socket connected to -> {0}", sender.RemoteEndPoint.ToString()));

                    //Create and serialize pair request
                    PairRequest pr = new PairRequest("pair", "roc");
                    JsonSerializer serializer = new JsonSerializer();
                    string pairRequest = JsonConvert.SerializeObject(pr);

                    //Send pair request to TP
                    byte[] message = Encoding.ASCII.GetBytes(pairRequest + "\n");
                    sender.Send(message);
                    isPaired = true;
                    Thread listenThread = new Thread(() =>
                    {
                        Listen();
                    });
                    listenThread.Start();
                }
                catch(Exception ex)
                {
                    logger.Write(ex.ToString());
                }
            }
            catch(Exception ex)
            {
                logger.Write(ex.ToString());
            }
        }

        public void Listen()
        {
            while (isPaired)
            {
                logger.Write("Listening for TP...");
                try
                {
                    //Listen for the new message
                    byte[] messageReceived = new byte[1024];
                    sender.Receive(messageReceived);

                    //Write the message we recieved
                    //logger.Write(Encoding.ASCII.GetString(messageReceived)); //This should be commented out before release

                    //Pass the decoded message to the message handler
                    msgHandler.ReadMessage(Encoding.ASCII.GetString(messageReceived));
                }
                catch (SocketException ex)
                {
                    if(ex.ErrorCode == 10054)
                    {
                        logger.Write("Connection with TouchPortal forcibly closed.");
                        logger.Write("Closing ROC Client...");
                        isPaired = false;
                        Program.isRunning = false;
                    }
                }
                catch (Exception ex)
                {
                    logger.Write(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Accept a list of scenes and pass it to Touch Portal
        /// </summary>
        public void UpdateScenesList(List<string> scenes)
        {
            ChangeList scenesMessage = new ChangeList();

            scenesMessage.type = "choiceUpdate";
            scenesMessage.id = "roc_0001";
            scenesMessage.options = scenes;

            logger.Write("Sending scenes list to TP");
            sender.Send(Encoding.ASCII.GetBytes(FormatListUpdateString(scenesMessage)));
        }

        /// <summary>
        /// Sends a message to Touch Portal with a list active servers
        /// </summary>
        /// <param name="sources">Servers listening for commands</param>
        public void UpdateServerList(List<string> sources)
        {
            ChangeList srvMessage = new ChangeList();

            srvMessage.type = "choiceUpdate";
            srvMessage.id = "roc_1111";
            srvMessage.options = sources;

            logger.Write("Sending server list to TP");
            sender.Send(Encoding.ASCII.GetBytes(FormatListUpdateString(srvMessage)));
        }

        /// <summary>
        /// Formats a message which provides a list of scene transitions to Touch Portal
        /// </summary>
        /// <param name="transitions">Transitions available in OBS</param>
        public void UpdateTransitionsList(List<string> transitions)
        {
            ChangeList srvMessage = new ChangeList();

            srvMessage.type = "choiceUpdate";
            srvMessage.id = "roc_0002";
            srvMessage.options = transitions;

            //logger.Write(String.Format("UpdateTransitionsList: {0}", FormatListUpdateString(srvMessage)));
            sender.Send(Encoding.ASCII.GetBytes(FormatListUpdateString(srvMessage)));
        }

        private string FormatListUpdateString(ChangeList changeMessage)
        {
            string message = "";

            message = "{" + "\"type\":\"" + changeMessage.type + "\",";
            message = message + "\"id\":\"" + changeMessage.id + "\",";
            message = message + "\"value\":[";
            foreach (string option in changeMessage.options)
                message += option + ",";
            message = message.Remove(message.Length - 1);
            message += "]}\n";

            //logger.Write(String.Format("Message Formated: {0}", message));
            return message;
        }
    }
}
