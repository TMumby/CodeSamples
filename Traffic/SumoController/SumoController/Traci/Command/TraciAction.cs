using System;
using System.Collections.Generic;


namespace SumoController.Traci.Command
{
    /// <summary>
    /// Base class for all interaction with Sumo via Traci
    /// </summary>
    public abstract class TraciAction
    {
        protected Connection _connection;

        protected TraciRequestMessage _traciRequest = new TraciRequestMessage();

        protected TraciResponseMessage _traciResponse;

        protected TraciConstants.Command _command;

        protected List<byte> _commandBody;

        /// <summary>
        /// Initialise class
        /// </summary>
        /// <param name="sumoConnection">the Sumo/Traci connection</param>
        public TraciAction(Connection sumoConnection)
        {
            _connection = sumoConnection;            
        }

        /// <summary>
        /// Sends message to traci, places response in a TraciResponseMessage
        /// </summary>
        protected virtual void SendMessage()
        {
            try
            {
                SetBody();
                _traciRequest.AddCommand(_command, _commandBody);
                _traciResponse = _connection.SendMessage(_traciRequest);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Sending Traci Message", ex);
            }
        }
     
        protected abstract void SetBody();




    }
}
