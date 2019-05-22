using System;
using System.Collections.Generic;
using System.Linq;


namespace SumoController.Traci.Command
{
    /// <summary>
    /// Class for constructing Traci Request message
    /// </summary>
    public class TraciRequestMessage 
    {
        private  Dictionary<TraciConstants.Command, List<byte>> _commandQueue = new Dictionary<TraciConstants.Command, List<byte>>();
        
        /// <summary>
        /// Adds command to command queue
        /// </summary>
        /// <param name="commandIdentifier">command name</param>
        /// <param name="commandBody">command body in bytes</param>
        public void AddCommand(TraciConstants.Command commandIdentifier, List<byte> commandBody )
        {
            try
            {
                _commandQueue.Add(commandIdentifier, commandBody);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error adding command to command queue", ex);
            }                        
        }

        /// <summary>
        /// Creates the Traci message from the command queue
        /// </summary>
        /// <returns>Traci Message</returns>
        public byte[] GetMessage()
        {
            try
            {
                List<byte> messageBodyBytes = BuildMessageBody();

                int messageSize = messageBodyBytes.Count + 4;
                List<byte> header = TraciDataHelper.CreateTraciIntBytes(messageSize);

                return header.Concat(messageBodyBytes).ToArray();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error creating Traci Message", ex);
            }
        }

        /// <summary>
        /// Creates the Traci message body
        /// </summary>
        /// <returns>Traci message body in bytes</returns>
        private List<byte> BuildMessageBody()
        {
            try
            {
                List<byte> sectionBytes = new List<byte>();
                foreach (KeyValuePair<TraciConstants.Command, List<byte>> command in _commandQueue)
                {
                    sectionBytes = TraciDataHelper.AddToByteList(sectionBytes, BuildCommandBytes(command));
                }

                return sectionBytes;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error buiding Traci Message body", ex);
            }
        }

        /// <summary>
        /// Builds the command in bytes, includes header defining size
        /// </summary>
        /// <param name="command">key value pair consisting of command identifier and command body</param>
        /// <returns>command in bytes</returns>
        private List<byte> BuildCommandBytes(KeyValuePair<TraciConstants.Command, List<byte>> command)
        {
            try
            {
                List<byte> commandBytes = BuildCommandHeader(command);
                commandBytes = AddCommandBody(commandBytes, command.Value);
                return commandBytes;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error buiding Traci Command", ex);
            }
        }

        /// <summary>
        /// Builds the command header (size of message + command identifier) in bytes
        /// </summary>
        /// <param name="command">key value pair consisting of command identifier and command body</param>
        /// <returns>the command header in bytes</returns>
        private List<byte> BuildCommandHeader(KeyValuePair<TraciConstants.Command, List<byte>> command)
        {
            try
            {
                List<byte> headerBytes = new List<byte>();
                int commandTotalLength = 2 + GetCommandBodyLength(command.Value);
                headerBytes.Add((byte)commandTotalLength);
                headerBytes.Add((byte)command.Key);

                return headerBytes;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Building Command header", ex);
            }            
        }

        /// <summary>
        /// calculates command body length, returns 0 for null
        /// </summary>
        /// <param name="commandBody">command body in bytes</param>
        /// <returns>command body length</returns>
        private int GetCommandBodyLength(List<byte> commandBody)
        {
            try
            {
                int commandBodyLength = 0;
                if (commandBody != null)
                {
                    commandBodyLength = commandBody.Count;
                }

                return commandBodyLength;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error calculating command body length", ex);
            }
        }

        /// <summary>
        /// Adds the commands body in bytes
        /// </summary>
        /// <param name="commandBytes">command byte list with header but no body</param>
        /// <param name="command">the command to be added in bytes</param>
        /// <returns>command including command body in bytes</returns>
        private List<byte> AddCommandBody(List<byte> commandBytes, List<byte> command)
        {
            try
            {
                if (command != null)
                {
                    commandBytes = commandBytes.Concat(command).ToList();
                }

                return commandBytes;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error adding command body to command");
            }
        }
    }
}
