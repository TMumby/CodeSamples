using System;
using SumoController.Traci.Command;
using SumoController.Traci;


namespace SumoController
{
    /// <summary>
    /// Gets details of Detector and interacts with Sumo Induction Loop
    /// </summary>
    public class TrafficDetector
    {
        private Connection _sumoConnection;
        public string DetectorID { get; private set; }

        public string RouteNo { get; private set; }

        public string SectionNo { get; private set; }

        public string Direction { get; private set; }

        public string Url { get; private set; }
             
        public TraciInductionLoop TraciInductionLoop { get; private set; }

        public string LastVehicle { get; private set; }

        /// <summary>
        /// Initialises Traffic Detector, gets addititional details from config
        /// </summary>
        /// <param name="sumoConnection">the Sumo/Traci connection</param>
        /// <param name="loopName">Traffic Detector/loop name</param>
        public TrafficDetector(Connection sumoConnection, string loopName)
        {
            try
            {
                _sumoConnection = sumoConnection;
                TraciInductionLoop = new TraciInductionLoop(_sumoConnection, loopName);
                DetectorID = loopName;
                GetDetectorDetails();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error initialising Traffic Detector", ex);
            }
        }

        /// <summary>
        /// Retrieves detector details from config
        /// </summary>
        private void GetDetectorDetails()
        {
            try
            {
                Url = ConfigReader.GetAppSettingString(DetectorID + "Url");
                SectionNo = ConfigReader.GetAppSettingString(DetectorID + "SectionNo");
                RouteNo = ConfigReader.GetAppSettingString(DetectorID + "RouteNo");
                Direction = ConfigReader.GetAppSettingString(DetectorID + "Direction");
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error aquiring detector details from config", ex);
            }
        }

        /// <summary>
        /// Checks to see if new vehicle passed has entered the detectors induction loop
        /// </summary>
        /// <returns>true if new vehicle entered induction loop</returns>
        public bool NewVehicle()
        {
            try
            {
                if (TraciInductionLoop.LastStepVehicleIds.Count != 0
                    && TraciInductionLoop.LastStepVehicleIds[0] != LastVehicle)
                {
                    LastVehicle = TraciInductionLoop.LastStepVehicleIds[0];
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                throw new SumoControllerException("Error checking if new vehicle has been detected", ex);
            }
        }

    }
}
