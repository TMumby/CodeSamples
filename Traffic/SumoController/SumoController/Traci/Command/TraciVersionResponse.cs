using System;

namespace SumoController.Traci.Command
{
    /// <summary>
    /// For retrieving sumo version number using Traci
    /// </summary>
    public class TraciVersionResponse : TraciAction
    {
        public int ApiVersion;

        public string Software;

        /// <summary>
        /// Initialises class, sends message to retrieve sumo version no
        /// </summary>
        /// <param name="connection"></param>
        public TraciVersionResponse(Connection connection) : base(connection)
        {
            _command = TraciConstants.Command.CMD_GETVERSION;
            SendMessage();

            ApiVersion = TraciDataHelper.GetTraciInteger(_traciResponse.MessageBody, _traciResponse.MessageStart);
            Software = TraciDataHelper.GetTraciString(_traciResponse.MessageBody, _traciResponse.MessageStart + 8);
        }

        /// <summary>
        /// Sets the body. Null as no body needed for this Traci command
        /// </summary>
        protected override void SetBody()
        {
            try
            {
                _commandBody = null;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting command body for Version Request", ex);
            }            
        }
    }
}
