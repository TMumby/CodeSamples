using System;
using System.Collections.Generic;


namespace SumoController.Traci.Command
{
    /// <summary>
    /// communicates with Sumo using Traci to get a list of all traffic lights
    /// </summary>
    public class TraciGetTrafficLights
    {
        public List<string> IDList { get; protected set; }

        /// <summary>
        /// Intialises class and retrieves list of traffic lights from Sumo
        /// </summary>
        /// <param name="connection">the Sumo/Traci connection</param>
        public TraciGetTrafficLights(Connection connection)
        {
            try
            {
                TraciVariableAction loopResponse = new TraciVariableAction(connection, 
                                                                           TraciConstants.Command.CMD_GET_TL_VARIABLE, 
                                                                           TraciConstants.Variable.ID_LIST, "GetTrafficLights");
                IDList = loopResponse.StringListResponse;
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error retrieving list of traffic lights", ex);
            }
        }
    }
}
