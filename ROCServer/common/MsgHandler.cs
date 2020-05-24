using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

namespace ROCServer
{
    class MsgHandler
    {
        OBSWebsocket obsSocket;

        public MsgHandler(OBSWebsocket _obsSocket)
        {
            obsSocket = _obsSocket;
        }

        public string ReadMessage(string message)
        {
            string returnString = "";
            string messageCode = message.Split(":")[0];
            string messageValue = message.Split(":")[1];

            if (messageCode.Equals("1000")) //Request for connection
            {
                returnString = "2000:ConnectionRequestAck";
            }
            else if (messageCode.Equals("1100")) //Request for Scene change
            {
                obsSocket.SetCurrentScene(messageValue);
                returnString = "2100:SceneChangeAck";
            }
            else if (messageCode.Equals("1200")) //Request for Transistion change
            {
                obsSocket.SetCurrentTransition(messageValue);
                returnString = "2200:TransitionChangeAck";
            }
            else if (messageCode.Equals("1300")) //Request for Stream Start
            {
                obsSocket.StartStreaming();
                returnString = "2300:StreamStartAck";
            } 
            else if (messageCode.Equals("1400")) //Request for Stream Stop
            {
                obsSocket.StopStreaming();
                returnString = "2400:StreamStopAck";
            }
            else if (messageCode.Equals("1500")) //Request for Record Start
            {
                obsSocket.StartRecording();
                returnString = "2500:RecordStartAck";
            }
            else if (messageCode.Equals("1600")) //Request for Record Stop
            {
                obsSocket.StopRecording();
                returnString = "2600:RecordStopAck";
            }
            else if (messageCode.Equals("1700"))
            {
                List<OBSScene> _scenes = obsSocket.ListScenes();
                List<string> scenes = new List<string>();
                foreach(var scene in _scenes)
                {
                    scenes.Add(scene.Name);
                }
                string scenesResponse = JsonSerializer.Serialize(scenes);
                returnString = "2700:" + scenesResponse;
            }
            else if (messageCode.Equals("1800"))
            {
                GetTransitionListInfo transitions =  obsSocket.GetTransitionList();
                List<string> transList = new List<string>();
                foreach(var trans in transitions.Transitions)
                {
                    transList.Add(trans.Name);
                }
                string transResponse = JsonSerializer.Serialize(transList);
                returnString = "2800:" + transResponse;
            }
            else
            {
                returnString = "0000:UNKNOWN_MESSAGE";
            }

            return returnString;
        }
    }
}
