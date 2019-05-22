using System;
using System.Collections.Generic;


namespace SumoController.Traci.Command
{
    /// <summary>
    /// For communicating with Sumo for interacting with one induction loop/detector
    /// </summary>
    public class TraciInductionLoop 
    {
        private Connection _connection;
        private List<string> _lastStepVehicleIds = new List<string>();
        public double Position { get; protected set; }
        public List<string> IDList { get; protected set; }
        public string InductionLoopID { get; protected set; }

        
        public List<string> LastStepVehicleIds
        {
            get
            {
                SetLastStepVehicleIds();
                return _lastStepVehicleIds;
            }
            private set
            { _lastStepVehicleIds = value; }
        }

        /// <summary>
        /// Initialises class, retrieves details of detector/loop from Sumo
        /// </summary>
        /// <param name="connection">the Sumo/Traci connection</param>
        /// <param name="inductionLoopID">Traffic Detector/loop name</param>
        public TraciInductionLoop(Connection connection, string inductionLoopID) 
        {
            _connection = connection;
            InductionLoopID = inductionLoopID;
            SetProperties();            
        }

        /// <summary>
        /// Set the properties of the class
        /// </summary>
        private void SetProperties()
        {
            try
            {
                SetPosition();
                SetIdList();
                SetLastStepVehicleIds();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting Induction Loop properties", ex);
            }
        }

        /// <summary>
        /// sets the posion property by retrieving value from Sumo
        /// </summary>
        private void SetPosition()
        {
            try
            {
                TraciVariableAction loopResponse = new TraciVariableAction(_connection, 
                                                                            TraciConstants.Command.CMD_GET_INDUCTIONLOOP_VARIABLE, 
                                                                            TraciConstants.Variable.VAR_POSITION, InductionLoopID);
                Position = loopResponse.DoubleResponse;
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error retrieving Induction loop position", ex);
            }

        } 
           
        /// <summary>
        /// Sets property by retrieving list of induction loops from Sumo
        /// </summary>
        private void SetIdList()
        {    
            try
            {
                TraciVariableAction loopResponse = new TraciVariableAction(_connection,
                                                           TraciConstants.Command.CMD_GET_INDUCTIONLOOP_VARIABLE,
                                                           TraciConstants.Variable.ID_LIST,
                                                           InductionLoopID);
                IDList = loopResponse.StringListResponse;

            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving list of induction loops", ex);
            }
        }

        /// <summary>
        /// Sets the vehicle IDs that have passed into detector by retrieving from sumo
        /// </summary>
        private void SetLastStepVehicleIds()
        {
            try
            {
                TraciVariableAction loopResponse = new TraciVariableAction(_connection,
                                                            TraciConstants.Command.CMD_GET_INDUCTIONLOOP_VARIABLE,
                                                            TraciConstants.Variable.LAST_STEP_VEHICLE_ID_LIST,
                                                            InductionLoopID);
                _lastStepVehicleIds = loopResponse.StringListResponse;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving vehicle IDS that have passed into detector", ex);
            }
        }
    }
}
