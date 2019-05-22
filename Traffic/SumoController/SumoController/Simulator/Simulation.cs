using System;
using System.Collections.Generic;
using SumoController.Traci;
using SumoController.Traci.Command;

namespace SumoController
{
    public class Simulation
    {
        private Connection _connection;

        private List<TrafficDetector> _trafficDetectorList = new List<TrafficDetector>();
        private List<TrafficLight> _trafficLightList = new List<TrafficLight>();

        /// <summary>
        /// Class initialiser. Gets all data for simulation from Sumo and config
        /// Starts listening for change light requests
        /// </summary>
        /// <param name="sumoConnect"></param>
        public Simulation(Connection sumoConnect)
        {
            Console.WriteLine("Starting Simulation");
            _connection = sumoConnect;
            SetScenario();            
            SignalChangeListener signalChangeLister = new SignalChangeListener(_trafficLightList);
        }

        /// <summary>
        /// Gets all data for simulation from Sumo and config
        /// </summary>
        private void SetScenario()
        {
            try
            {
                SetLights();
                GetTrafficDetectors();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting scenario", ex);
            }
        }

        /// <summary>
        /// Gets Traffic light data from simulation then sets all lights to phase 0
        /// </summary>
        private void SetLights()
        {
            try
            {
                List<string> lights = GetTrafficLightData();
                AddLights(lights);                
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error setting retriving and setting traffic lights", ex);
            }
        }
        /// <summary>
        /// Gets List of Traffic lights from Sumo from Sumo
        /// </summary>
        /// <returns>List of traffic lights</returns>
        private List<string> GetTrafficLightData()
        {
            try
            {
                TraciGetTrafficLights trafficLightRequest = new TraciGetTrafficLights(_connection);
                return trafficLightRequest.IDList;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error acquiring traffic lights from Sumo", ex);
            }

        }

        /// <summary>
        /// Adds Lights to the simulation
        /// </summary>
        /// <param name="trafficLightList">list of traffic lights</param>
        private void AddLights(List<string> trafficLightList)
        {
            try
            {
                foreach (string trafficLight in trafficLightList)
                {
                    _trafficLightList.Add(new TrafficLight(_connection, trafficLight));
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error adding lights to the scenario", ex);
            }
        }


        /// <summary>
        /// Gets the traffic detectors from sumo and add them to simulation
        /// </summary>
        private void GetTrafficDetectors()
        {
            try
            {
                List<string> detectors = GetTrafficDetectorData();
                AddDetectors(detectors);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving detector data", ex);
            }
        }

        /// <summary>
        /// Gets the traffic detectors from sumo
        /// </summary>
        /// <returns>a list of traffic detectors</returns>
        private List<string> GetTrafficDetectorData()
        {
            try
            {
                TraciInductionLoop inductionLoopRequest = new TraciInductionLoop(_connection, "Loop0"); ;
                return inductionLoopRequest.IDList;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error retrieving detector data from sumo", ex);
            }
        }

        /// <summary>
        /// Adds detectors to simulation
        /// </summary>
        /// <param name="trafficDetectors">list of traffic detectors</param>
        private void AddDetectors(List<string> trafficDetectors)
        {
            try
            {
                foreach (string detector in trafficDetectors)
                {
                    _trafficDetectorList.Add(new TrafficDetector(_connection, detector));
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error adding detectors to simuilation scenario", ex);
            }
        }

        /// <summary>
        /// Starts the simultion
        /// </summary>
        public void Simulate()
        {
            try
            {
                int simSteps = ConfigReader.GetIntAppSetting("SimulationSteps");
                for (int i = 0; i <= simSteps; i++)
                {
                    SimulationCycle();
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error occurred during simulation", ex);
            }
        }

        /// <summary>
        /// One simulation cycle
        /// Moves the simulation on 1 step and detects if any cars have been detected by sensors
        /// </summary>
        private void SimulationCycle()
        {
            try
            {
                _connection.SimulationStep();
                DetectCars();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error in simulation cycle", ex);
            }
        }

        /// <summary>
        /// Checks to see if any cars have been detected in all dectors
        /// </summary>
        private void DetectCars()
        {
            try
            {
                foreach (TrafficDetector trafficDetector in _trafficDetectorList)
                {
                    CheckDetectorForVehicle(trafficDetector);
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error detecting cars", ex);
            }
        }

        /// <summary>
        /// See if a naw vehicle has entered a detector
        /// </summary>
        /// <param name="trafficDetector">traffic detector to check</param>
        private void CheckDetectorForVehicle(TrafficDetector trafficDetector)
        {
            try
            {
                if (trafficDetector.NewVehicle())
                {
                    VehiclePost vehiclePost = new VehiclePost(trafficDetector);
                }
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error checking individual detector for vehicle", ex);
            }
        }
    }
}
