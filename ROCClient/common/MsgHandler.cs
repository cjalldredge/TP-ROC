using Newtonsoft.Json;
using ROCClient.common.messages;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ROCClient
{
    class MsgHandler
    {
        List<ROCServer> rocComms;
        ROCServer rocComm;
        Logger logger;

        public MsgHandler(List<ROCServer> _rocComms, Logger _logger)
        {
            rocComms = _rocComms;
            logger = _logger;
        }

        public void ReadMessage(string message)
        {
            
            string destinationIP = "";  // Destination that this command should be sent to
            string actionId = "";       // Message code identifier
            string actionValue = "";    // Command parameter
            string listId = "";
            string listValue = "";

            TPAction action = new TPAction();
            ListChange listChange = new ListChange();

            try
            {
                if (message.Contains("\"type\":\"listChange\""))
                {
                    listChange = JsonConvert.DeserializeObject<ListChange>(message);
                    listId = listChange.listId;
                    listValue = listChange.value;

                    //logger.Write(message);
                    logger.Write(String.Format("ListChange received from TouchPortal {0}:{1} ", listId, listValue));
                }
                else
                {
                    action = JsonConvert.DeserializeObject<TPAction>(message);
                    foreach (Dictionary<string, string> command in action.data)
                    {
                        if (command["id"] == "roc_1111")
                            destinationIP = command["value"];
                        else
                        {
                            actionId = command["id"];
                            actionValue = command["value"];
                        }
                    }
                    logger.Write(String.Format("Command {0}:{1} destined for {2}", actionId, actionValue, destinationIP));
                }

                Thread.Sleep(500); // Pause while full message comes in??? I don't actually remember why I put this pause here...


                // Find the ROC server connection matching ip address 
                // parameter designation of received command
                foreach(ROCServer rocServ in rocComms)
                    if (rocServ.ipAddress == destinationIP)
                        rocComm = rocServ;



                //actionId = action.data[0]["id"];
                //actionValue = action.data[0]["value"];

                if (actionId == "roc_0001") //Set the current scene in Remote OBS
                {
                    string request = "1100:" + actionValue;
                    rocComm.ChangeScene(request);
                }
                else if (actionId == "roc_0002") //Set the current transition in Remote OBS
                {
                    string request = "1200:" + actionValue;
                    logger.Write("Received request for transition change from TouchPortal.");
                    logger.Write(String.Format("Sending {0}", request));
                    rocComm.ChangeTransition(request);
                }
                else if (actionId == "roc_0003") //Start OBS streaming
                {
                    string request = "1300:" + actionValue;
                    logger.Write("Received request for Stream Start from TouchPortal.");
                    logger.Write(String.Format("Sending {0}", request));
                    rocComm.StartStream(request);
                }
                else if (actionId == "roc_0004") //Stop OBS Streaming
                {
                    string request = "1400:" + actionValue;
                    logger.Write("Received request for Stream Stop from TouchPortal.");
                    logger.Write(String.Format("Sending {0}", request));
                    rocComm.StopStream(request);
                }
                else if (actionId == "roc_0005") //Start OBS Recording
                {
                    string request = "1500:" + actionValue;
                    logger.Write("Received request for Record Start from TouchPortal.");
                    logger.Write(String.Format("Sending {0}", request));
                    rocComm.StartRecording(request);
                }
                else if (actionId == "roc_0006") //Stop OBS Recording
                {
                    string request = "1600:" + actionValue;
                    logger.Write("Received request for Record Stop from TouchPortal.");
                    logger.Write(String.Format("Sending {0}", request));
                    rocComm.StopRecording(request);
                } 
                else if (listId == "roc_1111" && listChange.actionId == "roc_scene_selector") // ROC Server selection changed
                {
                    foreach(ROCServer server in rocComms) // Loop through all our ROC Servers
                        if(server.ipAddress == listValue) // Find the server the user selected
                            Program.tpComm.UpdateScenesList(server.GetScenes()); // Update scenes list with that servers scenes
                }
                else if (listId == "roc_1111" && listChange.actionId == "roc_trans_setter")
                {
                    foreach (ROCServer server in rocComms)  // Loop through all our ROC Servers
                        if (server.ipAddress == listValue)  // Find the server the user selected
                            Program.tpComm.UpdateTransitionsList(server.GetTransitions());
                }
                else
                {
                    logger.Write(String.Format("Unknown message {0}:{1} from TouchPortal detected.", actionId, actionValue));
                    //logger.Write(message);
                    logger.Write("Disposing message...");
                }
            }
            catch(Exception ex)
            {
                //logger.Write("[ERROR]: Error while processing message: \n" + message);
                //logger.Write(ex.ToString());
            }
        }
    }
}
