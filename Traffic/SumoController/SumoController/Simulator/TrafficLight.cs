using System;
using SumoController.Traci;
using SumoController.Traci.Command;

namespace SumoController
{
    /// <summary>
    /// Class for communicating with a single traffic light
    /// </summary>
    public class TrafficLight
    {
        private Connection _connection;
        public string ID { get; protected set; }

       
        /// <summary>
        /// Initialises traffic light
        /// </summary>
        /// <param name="sumoConnection">the Sumo/Traci connection</param>
        /// <param name="trafficLightName">traffic light name</param>
        public TrafficLight (Connection sumoConnection, string trafficLightName)
        {
            try
            {
                ID = trafficLightName;
                _connection = sumoConnection;
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error initialising traffic light", ex);
            }
        }

        /// <summary>
        /// Sets the phase of Traffic light by sending Traci message to Sumo
        /// </summary>
        /// <param name="phaseDefinition">phase definition for light to change state to</param>
        /// <param name="tJunctionFlag">T Junction flag</param>
        public void SetPhase(string phaseDefinition)
        {
            try
            {                
                TraciChangeState phaseChange = new TraciChangeState(_connection,
                                                                    TraciConstants.Command.CMD_SET_TL_VARIABLE,
                                                                    TraciConstants.Variable.TL_RED_YELLOW_GREEN_STATE,
                                                                    ID,
                                                                    TraciConstants.DataType.TYPE_STRING,
                                                                    phaseDefinition);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting traffic light phase", ex);
            }           
        }
    }
}
