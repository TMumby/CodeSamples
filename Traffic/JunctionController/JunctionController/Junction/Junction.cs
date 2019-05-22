using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// Controls Junction
    /// </summary>
    public static class Junction
    {

        private static List<Route> _routes;

        private static TrafficLight Light;
                

        /// <summary>        
        /// adds routes and traffic light to the juction,
        /// starts the junction
        /// </summary>
        public static void Start()
        {
            try
            {
                AddRoutes();
                Light = new TrafficLight();
                Light.Start();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error starting Junction", ex);
            }
        }

        /// <summary>
        /// Adds routes to the junction
        /// </summary>
        private static void AddRoutes()
        {
            try
            {
                _routes = new List<Route>()
                {
                    new Route(0),
                    new Route(1)
                };
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adding routes to the junction", ex);
            }
        }

        /// <summary>
        /// Adjusts the timing of the light phases according to vehicle data
        /// passing through sensor
        /// </summary>
        /// <param name="vehicleData">data of vehicle passing through sensor </param>
        public static void AdjustTiming(VehicleData vehicleData)
        {
            try
            {
                Light.Pause();
                AdjustPollution(vehicleData);
                Light.AdjustTiming(vehicleData, _routes);
                Light.Restart();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting timing for phases", ex);
            }
        }

        /// <summary>
        /// Adjusts recorded pollution in section due to pollution data
        /// </summary>
        /// <param name="vehicleData">data of vehicle passing through sensor</param>
        private static void AdjustPollution(VehicleData vehicleData)
        {
            try
            {
                _routes[vehicleData.RouteNo].AmendPollution(vehicleData);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting pollution", ex);
            }            
        }

        /// <summary>
        /// Adjusts configuration
        /// </summary>
        /// <param name="config">dictionary of configuration items</param>
        public static void AdjustConfig(Dictionary<string, string> config)
        {
            try
            {
                Light.Pause();
                Light.AdjustConfig(config);
                AdjustPollutionFactors(config);
                Light.Restart();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting configuration items", ex);
            }
        }

        /// <summary>
        /// Adjusts the pollution factors for sections from config
        /// </summary>
        /// <param name="config">dictionary of configuration items</param>
        private static void AdjustPollutionFactors(Dictionary<string, string> config)
        {
            try
            {
                foreach (Route route in _routes)
                {
                    route.AdjustPollutionFactors(config);
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting pollution factors for Routes", ex);
            }
        }

        /// <summary>
        /// Triggered when interval time for phase is reached by timer.
        /// Changes the phase of the lights
        /// </summary>
        public static void PhaseComplete()
        {
            try
            {
                Light.PhaseComplete();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error changing light phase", ex);
            }            
        }
    }
}
