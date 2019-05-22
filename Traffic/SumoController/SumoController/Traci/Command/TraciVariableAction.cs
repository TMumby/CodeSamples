using System;
using System.Collections.Generic;
using System.Linq;


namespace SumoController.Traci.Command
{
    /// <summary>
    /// For sending variable command (command with sub command) to Sumo using Traci
    /// </summary>
    public class TraciVariableAction : TraciAction

    {
        private TraciConstants.Variable _variableName;
        private string _variableValue;

        public double DoubleResponse { get; protected set; }
        public List<string> StringListResponse { get; protected set; }

        public string StringResponse { get; protected set; }

        public TraciVariableAction(Connection connection, 
                                   TraciConstants.Command command, 
                                   TraciConstants.Variable variableName,
                                   string variableValue)  : base(connection)
        {
            try
            {
                _command = command;
                _variableName = variableName;
                _variableValue = variableValue;
                SendMessage();
                ProcessResponse();
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error whilst initialising Traci Variable Action class", ex);
            }            
        }

        /// <summary>
        /// Sets Body for variable action Traci message
        /// </summary>
        protected override void SetBody()
        {
            try
            {
                _commandBody = new List<byte>();
                _commandBody.Add((byte)_variableName);
                _commandBody = TraciDataHelper.AddToByteList(_commandBody, TraciDataHelper.CreateTraciString(_variableValue));
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting body of Traci variable message", ex);
            }
        }

        private void ProcessResponse()
        {
            try
            {
                if (!IsEmptyResponse())
                {
                    GetData();
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error proecessing Traci variable message response", ex);
            }
        }
        
        /// <summary>
        /// Checks to see if message body is empty
        /// </summary>
        /// <returns>returns true if the message body is not empty</returns>
        private bool IsEmptyResponse()
        {
            try
            {
                if (_traciResponse.MessageBody.Count() == 0)
                {
                    DoubleResponse = 0;
                    StringListResponse = new List<string>();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error checking if response was empty", ex);
            }
        }

        /// <summary>
        /// Retrievess data from message body. populates property dependant on data type
        /// </summary>
        private void GetData()
        {
            try
            {
                int dataTypePosition = TraciDataHelper.GetTraciInteger(_traciResponse.MessageBody, _traciResponse.MessageStart + 1) + _traciResponse.MessageStart + 5;
                switch (TraciDataHelper.GetTraciDataType(_traciResponse.MessageBody, dataTypePosition))
                {

                    case TraciConstants.DataType.TYPE_DOUBLE:
                        DoubleResponse = TraciDataHelper.GetTraciDouble(_traciResponse.MessageBody, dataTypePosition + 1);
                        break;

                    case TraciConstants.DataType.TYPE_STRINGLIST:
                        StringListResponse = TraciDataHelper.GetTracStringList(_traciResponse.MessageBody, dataTypePosition + 1);
                        break;

                    case TraciConstants.DataType.TYPE_STRING:
                        StringResponse = TraciDataHelper.GetTraciString(_traciResponse.MessageBody, dataTypePosition + 1);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving data from Variable Traci data response", ex);
            }
        }
    }
}
