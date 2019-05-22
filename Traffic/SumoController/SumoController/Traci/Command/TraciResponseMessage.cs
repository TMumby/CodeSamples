using System;
using System.Linq;

namespace SumoController.Traci.Command
{
    /// <summary>
    /// Holds Traci response message. Retrieves status header and main body of message
    /// </summary>
    public class TraciResponseMessage 
    {     
       
        public int MessageLength { get; private set; }
        public TraciConstants.Command Command { get; private set; }
        public bool Success { get; private set; }
        public int StatusHeaderLength { get; private set; }
        public byte[] MessageBody { get; private set; }
        public int MessageBodyLength { get; private set; }
        public int MessageStart { get; private set; }


        /// <summary>
        /// Initialises class, retrieves header and main body from message
        /// </summary>
        /// <param name="response">Traci response message</param>
        public TraciResponseMessage(byte[] response)
        {
            MessageLength = TraciDataHelper.GetTraciInteger(response, 0);
            Command = TraciDataHelper.GetTraciCommand(response, 5);
            ReadStatusHeader(response);
            SetMessageBody(response);
            SetMessageStart();
        }

        /// <summary>
        /// Retrieves the Status header from Traci response message
        /// </summary>
        /// <param name="response">the traci reponse in bytes<</param>
        private void ReadStatusHeader(byte[] response)
        {
            try
            {
                StatusHeaderLength = (int)response[4];
                SetStatus(response);
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error retriving status header from traci response", ex);
            }

        }

        /// <summary>
        /// Retrieves the status from the Traci response
        /// </summary>
        /// <param name="response">the traci reponse in bytes</param>
        private void SetStatus(byte[] response)
        {
            try
            {
                byte status = response[6];

                if (status == 0)
                {
                    Success = true;
                }
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error retrieving message status", ex);
            }
        }

        /// <summary>
        /// Gets the main message body from the traci response
        /// </summary>
        /// <param name="response">the traci reponse in bytes<</param>
        private void SetMessageBody(byte[] response)
        {   try
            {                
                MessageBodyLength = MessageLength - StatusHeaderLength - 4;
                MessageBody = response.Skip(4 + StatusHeaderLength).Take(MessageBodyLength).ToArray();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving message body", ex);
            }   
        }   
        
        /// <summary>
        /// Set the position that the message starts within the data.
        /// Its dependant on data size.
        /// </summary>
        private void SetMessageStart()
        {
            try
            {
                MessageStart = 2;
                if (MessageBodyLength > 255)
                {
                    MessageStart = 6;
                }
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error establishing message start position", ex);            }

        }     
   }
}
