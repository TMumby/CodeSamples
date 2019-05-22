using System;
using System.Collections.Generic;

namespace SumoController.Traci.Command
{
    /// <summary>
    /// Returns the controlled lanes of a junction
    /// </summary>
    public class TraciGetTraficLightLanes
    {
        public List<string> LaneList { get; protected set; }

        /// <summary>
        /// Intialises class and retrieves list of traffic light's lanes from Sumo
        /// </summary>
        /// <param name="connection">the Sumo/Traci connection</param>
        public TraciGetTraficLightLanes(Connection connection, string lightName)
        {
            try
            {
                TraciVariableAction loopResponse = new TraciVariableAction(connection,
                                                                           TraciConstants.Command.CMD_GET_TL_VARIABLE,
                                                                           TraciConstants.Variable.TL_CONTROLLED_LANES, 
                                                                           lightName);
                LaneList = loopResponse.StringListResponse;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving list of traffic light lanes", ex);
            }
        }
    }
}
