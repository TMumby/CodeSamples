using System;
using System.Collections.Generic;

namespace SumoController.Traci.Command
{
    /// <summary>
    /// Class for changing state of Sumo objects using Traci
    /// </summary>
    public class TraciChangeState : TraciAction
    {
        private TraciConstants.Variable _variableName;
        private string _variableValue;
        private TraciConstants.DataType _dataType;
        private object _newData;

        /// <summary>
        /// Class for initialising a Change State message
        /// </summary>
        /// <param name="connection">the Sumo/Traci connection</param>
        /// <param name="command">Traci Command</param>
        /// <param name="variableName">Traci variable command (sub command)</param>
        /// <param name="variableValue">Valueassociated with variable, normally identifier of Sumo object</param>
        /// <param name="dataType">traci data type</param>
        /// <param name="newData">Value to set</param>
        public TraciChangeState(Connection connection,
                                   TraciConstants.Command command,
                                   TraciConstants.Variable variableName,
                                   string variableValue,
                                   TraciConstants.DataType dataType,
                                   object newData) : base(connection)
        {
            try
            {
                _command = command;
                _variableName = variableName;
                _variableValue = variableValue;
                _dataType = dataType;
                _newData = newData;
                SendMessage();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Initialising Traci Change State class", ex);
            }
        }

        /// <summary>
        /// Sets Body for change state Traci message
        /// </summary>
        protected override void SetBody()
        {
            try
            {
                _commandBody = new List<byte>();
                _commandBody.Add((byte)_variableName);
                _commandBody = TraciDataHelper.AddToByteList(_commandBody, TraciDataHelper.CreateTraciString(_variableValue));
                _commandBody = TraciDataHelper.AddToByteList(_commandBody, TraciDataHelper.CreateTraciData(_dataType, _newData));
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting Body for change state traci message", ex);
            }            
        }
    }
}
