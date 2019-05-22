using System;
using System.Collections.Generic;

namespace SumoController.Traci.Command
{
    /// <summary>
    /// Returns the current phase/state of traffic light
    /// </summary>
    public class TraciGetTrafficLightState
    {

        public string State { get; protected set; }

        /// <summary>
        /// Intialises class and retrieves list of traffic light's state from Sumo
        /// </summary>
        /// <param name="connection">the Sumo/Traci connection</param>
        public TraciGetTrafficLightState(Connection connection, string lightName)
        {
            try
            {
                TraciVariableAction loopResponse = new TraciVariableAction(connection,
                                                                           TraciConstants.Command.CMD_GET_TL_VARIABLE,
                                                                           TraciConstants.Variable.TL_RED_YELLOW_GREEN_STATE,
                                                                           lightName);
                State = loopResponse.StringResponse;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving traffic light state", ex);
            }
        }
    }
}
