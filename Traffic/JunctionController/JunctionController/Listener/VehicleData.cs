using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// class for holding data for vehicle picked up by sensor
    /// </summary>
    public class VehicleData

    {
        public double Pollution { get; protected set; }
        public int SectionNo { get; protected set; }   
        public int RouteNo { get; protected set; }

        public string NumberPlate { get; protected set; }

        public string Direction { get; protected set; }

        public int VehicleNo { get; protected set; }


        /// <summary>
        /// Intialises vehicle data. 
        /// Sets fields and calculates the pollution as a vector
        /// </summary>
        /// <param name="vehicleDetails">details of vehicle passing a sensor</param>
        public VehicleData(Dictionary<string, string> vehicleDetails)
        {
            try
            {
                SectionNo = Convert.ToInt32(vehicleDetails["SectionNo"]);
                RouteNo = Convert.ToInt32(vehicleDetails["RouteNo"]);
                NumberPlate = vehicleDetails["NoPlate"];
                Direction = vehicleDetails["Direction"];
                CalculatePollution(NumberPlate, Direction);
                LogJourney();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error initialising vehicle data", ex);
            }
        }

        /// <summary>
        /// Sets fields and calculates the pollution as a vector.
        /// Fetches the pollution value for the vehicle from the database,
        /// then uses its direction to see whether its positive or negative
        /// </summary>
        /// <param name="numberPlate"></param>
        /// <param name="direction"></param>
        private void CalculatePollution(string numberPlate, string direction)
        {
            try
            {
                double absolutePollution = GetVehicleDetailsPollution(numberPlate);
                Pollution = DirectionFactor(direction) * absolutePollution;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error calculating pollution", ex);
            }
        }

        /// <summary>
        /// Gets the vehicles pollution from database
        /// </summary>
        /// <param name="numberPlate">number plate of the vehicle</param>
        /// <returns></returns>
        private double GetVehicleDetailsPollution(string numberPlate)
        {
            try
            {
                IDataConnect vehicleDB = DependencyFactory.ResolveDataConnect(); ; 
                Dictionary<string, object> vehicleParameters = new Dictionary<string, object>()
                {
                    {"Registration", numberPlate}
                };
                List<Dictionary<string, object>> vehicleDetails
                                = vehicleDB.SendDataRequest("usp_GetVehicleData", vehicleParameters);
                VehicleNo = (int)vehicleDetails[0]["VehicleID"];
                return (double)vehicleDetails[0]["Pollution"];
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error getting vehicle pollution data from data source", ex);
            }
        }

        /// <summary>
        /// Logs vehicle details to database
        /// </summary>
        private void LogJourney()
        {
            try
            {
                if (LogjourneySetting())
                {
                    IDataConnect vehicleDB = DependencyFactory.ResolveDataConnect();
                    Dictionary<string, object> vehicleParameters = new Dictionary<string, object>()
                    {
                        { "ID", VehicleNo },
                        { "RouteNo", RouteNo },
                        { "SectionNo", SectionNo },
                        { "Direction", Direction },
                        { "Pollution", Pollution }
                    };
                    List<Dictionary<string, object>> vehicleDetails
                               = vehicleDB.SendDataRequest("usp_InsertJourneyLog", vehicleParameters);
                }
                 
            }
            catch (JunctionException ex)
            {
                throw new JunctionException("Error Logging Journey to database", ex);
            }
        }
        /// <summary>
        /// Gets Log Journey flag from config
        /// </summary>
        /// <returns>Log Journay flag</returns>
        private bool LogjourneySetting()
        {
            try
            {
                return ConfigReader.GetAppSettingBool("LogJourney");
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error getting log journey flag from confif", ex);
            }
        }
        /// <summary>
        /// transforms the direction item  to +1 or -1 depending on direction
        /// </summary>
        /// <param name="direction">direction, whether vehicle entered or exited section</param>
        /// <returns>+1 or -1 depending on direction</returns>
        private double DirectionFactor(string direction)
        {
            try
            {
                double directionFactor = 1;
                if (direction == "exit")
                {
                    directionFactor = -1;
                }
                return directionFactor;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error determining direction", ex);
            }
        }
    }
}
