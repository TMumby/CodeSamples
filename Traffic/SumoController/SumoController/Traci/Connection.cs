using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using SumoController.Traci.Command;
using System;

namespace SumoController.Traci
{
    /// <summary>
    /// Class for processsing network TCP network connection with Sumo
    /// </summary>
    public class Connection
    {
        private TcpClient _sumoClient = new TcpClient();
        private NetworkStream _stream;

        private string _hostname;
        private int _port;
        private int _receiveBufferSize;
        private int _sendBufferSize;

        /// <summary>
        /// Constructor, gets settings and uses them to connect to Sumo
        /// </summary>
        public Connection()
        {
            GetSettings();
            Connect();
        }

        /// <summary>
        /// Gets the connection settings from config
        /// </summary>
        private void GetSettings()
        {
            try
            {
                _hostname = ConfigReader.GetConnectionString("Sumo");
                _port = ConfigReader.GetIntAppSetting("SummoConnectionPort");
                _sendBufferSize = ConfigReader.GetIntAppSetting("SendBufferSize");
                _receiveBufferSize = ConfigReader.GetIntAppSetting("ReceiveBufferSize");
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error reading Sumo connection settings from config", ex);
            }
        }
        
        /// <summary>
        /// Connects to sumo 
        /// </summary>
        private void Connect ()
        {   
            try
            {
                InitiateConnection();
                _stream = _sumoClient.GetStream();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error connecting to Sumo", ex);
            }
        }

        /// <summary>
        /// Initialises connection to Sumo
        /// </summary>
        private void InitiateConnection()
        {
            try
            {
                _sumoClient.SendBufferSize = _sendBufferSize;
                _sumoClient.ReceiveBufferSize = _receiveBufferSize;
                _sumoClient.Connect(_hostname, _port);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error initialising connection to Sumo", ex);
            }
        }
        
        /// <summary>
        /// Sends message to Sumo
        /// </summary>
        /// <param name="message">message to send</param>
        /// <returns>response from Sumo</returns>                    
        public TraciResponseMessage SendMessage(TraciRequestMessage message)
        {
            try
            {
                _sumoClient.Client.Send(message.GetMessage());
                return GetResponse();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Sending Message to Sumo", ex);
            }

        }

        /// <summary>
        /// Gets and processes Sumo response
        /// </summary>
        /// <returns>Response Message</returns>
        private TraciResponseMessage GetResponse()
        {
            try
            {
                byte[] response = GetByteResponse();
                TraciResponseMessage traciResponseMessage = new TraciResponseMessage(response);
                return traciResponseMessage;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error getting and processing resposne from Sumo", ex);
            }
        }

        /// <summary>
        /// Get the bytes of the response fromn the data stream
        /// </summary>
        /// <returns>byte array of the response</returns>
        private byte[] GetByteResponse()
        {
            try
            {
                byte[] receiveBuffer = new byte[_receiveBufferSize];
                int bytesRead = _stream.Read(receiveBuffer, 0, _receiveBufferSize);
                byte[] response = receiveBuffer.Take(bytesRead).ToArray();
                return response;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving Bytes from Sumo response", ex);
            }

        }

        /// <summary>
        /// Sends a message to Sumo to process one simulation step
        /// </summary>
        public void SimulationStep()
        {
            try
            {
                SendMessage(CreateSimStepMessage());
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error sending simulation step", ex);
            }            
        }

        /// <summary>
        /// Create simulation step message
        /// </summary>
        /// <returns>simulation step message</returns>
        private TraciRequestMessage CreateSimStepMessage()
        {
            try
            {
                TraciRequestMessage traciMessage = new TraciRequestMessage();
                List<byte> body = TraciDataHelper.CreateTraciIntBytes(0);
                traciMessage.AddCommand(TraciConstants.Command.CMD_SIMSTEP, body);
                return traciMessage;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error createing simulation step message", ex);
            }
        }
    }

    

}
