using System;
using System.Collections.Generic;
using System.Linq;

namespace ROCClient
{
    class ROCServer
    {
        #region Vars and Containers
        public string ipAddress;    // IP Address of ROC server connected to OBS
        public int port;            // Port that ROC server is listening on
        public bool isConnected;    // Connection status of this endpoint

        public List<string> scenes;         // Scenes available in OBS where ROC is connected
        public List<string> transitions;    // Transitions available in OBS where ROC is connected

        public ROCServComm comm;    // Object responsible for communications with ROC server
        public Logger logger;       // Object responsible for outputing log messages to logfile
        #endregion

        #region Constructors
        /// <summary>
        /// Basic constructor establishing minimum required parameter to 
        /// initialize ROCServer object
        /// </summary>
        /// <param name="_ipAddr">IPAddress of the ROC Server present on machine hosting remote OBS client</param>
        /// <param name="_port">Port that ROC Server remote machine is listening on</param>
        /// <param name="_logger">Logger for outputing log messages to logfile</param>
        public ROCServer(string _ipAddr, int _port, Logger _logger)
        {
            ipAddress = _ipAddr;
            port = _port;
            logger = _logger;

            scenes = new List<string>();
            transitions = new List<string>();

            comm = new ROCServComm(logger, ipAddress, port, 5000);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Initializes attempt to connect to ROC server
        /// </summary>
        /// <returns>Returns connect attempt status</returns>
        public bool Connect()
        {
            comm.ConnectRequest();
            isConnected = comm.isConnected;
            return comm.isConnected;
        }

        /// <summary>
        /// Sends a request to ROC server for a list of scenes then returns the result
        /// </summary>
        /// <returns>Returns list of strings</returns>
        public List<string> GetScenes()
        {
            string response = comm.Send("1700:ScenesList").Split(':')[1];
            response = response.Replace("[", "");
            response = response.Replace("]", "");
            return response.Split(',').ToList();
        }

        /// <summary>
        /// Sends a request to ROC server for a list of the available transitions
        /// </summary>
        /// <returns>List of available transitions</returns>
        public List<string> GetTransitions()
        {
            string response = comm.Send("1800:TransitionsList").Split(':')[1];
            response = response.Replace("[", "");
            response = response.Replace("]", "");
            return response.Split(',').ToList();
        }

        /// <summary>
        /// Sends a request to ROC server to update the currently selected scene
        /// </summary>
        /// <param name="request">Request to send to ROC server</param>
        public void ChangeScene(string request)
        {
            logger.Write("Received request for scene change from TouchPortal.");
            string response = comm.Send(request);
            logger.Write(response);
        }

        /// <summary>
        /// Sends a request to ROC server to update the currently selected transition
        /// </summary>
        /// <param name="request">Request to send to ROC server</param>
        public void ChangeTransition(string request)
        {
            logger.Write("Received request for transition change from TouchPortal.");
            string response = comm.Send(request);
            logger.Write(response);
        }

        /// <summary>
        /// Sends a request to ROC server to begin streaming
        /// </summary>
        /// <param name="request">Request to send to ROC server</param>
        public void StartStream(string request)
        {
            logger.Write("Received request for stream start from TouchPortal");
            string response = comm.Send(request);
            logger.Write(response);
        }

        /// <summary>
        /// Sends a request to ROC server to stop streaming
        /// </summary>
        /// <param name="request">Request to send to ROC server</param>
        public void StopStream(string request)
        {
            logger.Write("Received request for stream stop from TouchPortal");
            string response = comm.Send(request);
            logger.Write(response);
        }

        /// <summary>
        /// Sends a request to ROC server to start recording
        /// </summary>
        /// <param name="request">Request to send to ROC server</param>
        public void StartRecording(string request)
        {
            logger.Write("Received request for recording start from TouchPortal");
            string response = comm.Send(request);
            logger.Write(response);
        }

        /// <summary>
        /// Sends a request to ROC server to stop recording
        /// </summary>
        /// <param name="request">Request to send to ROC server</param>
        public void StopRecording(string request)
        {
            logger.Write("Received request for recording stop from TouchPortal");
            string response = comm.Send(request);
            logger.Write(response);
        }
        #endregion
    }
}
