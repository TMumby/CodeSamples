using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// Holds data for one road/route of junction
    /// Comprise of 2 incoming sections to the junction
    /// In the case of a route only having 1  entry to the juction( T junction)
    /// then one incoming section can be ignored but must still be in the config file
    /// </summary>
    /// 
    public class Route
    {     
        private List<RouteSection> _incomingSection;

        /// <summary>
        /// The collective pollution for the route
        /// </summary>
        public double Pollution { get{ return CalculatePollution(); } }
        
        /// <summary>
        /// Initialises the route by adding the incoming sections
        /// </summary>
        /// <param name="routeNo"></param>
        public Route(int routeNo)
        {
            try
            {
                _incomingSection = new List<RouteSection>
            {
                new RouteSection(routeNo, 0),
                new RouteSection(routeNo, 1)
            };
            }
            catch (Exception ex)
            {
                throw new JunctionException("Intialising route", ex);
            }
        }
        
        /// <summary>
        /// Calculates the pollution for the route by summming 
        /// incomming section pollution
        /// </summary>
        /// <returns>pollution for the route</returns>
        private double CalculatePollution()
        {
            try
            {
                double pollution = 0;
                foreach (RouteSection section in _incomingSection)
                {
                    pollution += section.Pollution;
                }
                return pollution;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error calculating pollution for route", ex);
            }

        }

        /// <summary>
        /// Amends the routes pollution due to an incoming or outgoing vehicle
        /// </summary>
        /// <param name="vehicleData">data of vehicle passing through sensor</param>
        public void AmendPollution(VehicleData vehicleData)
        {
            try
            {
                _incomingSection[vehicleData.SectionNo].AdjustPollution(vehicleData.Pollution);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error Amending pollution for route", ex);
            }

        }

        
        /// <summary>
        /// Adjusts the pollution factors for the route 
        /// from an incoming config change request
        /// </summary>
        /// <param name="config">dictionary of config items</param>
        public void AdjustPollutionFactors(Dictionary<string, string> config)
        {
            try
            {
                foreach (RouteSection section in _incomingSection)
                {
                    section.AdjustPollutionFactor(config);
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting pollution factors for route", ex);
            }
        }
    }
}
