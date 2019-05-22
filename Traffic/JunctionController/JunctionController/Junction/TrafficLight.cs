using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// Represents and controls traffic light signal
    /// </summary>
    public class TrafficLight
    {
        private List<Phase> _phaseList;

        private double _cycleTime;

        private CurrentState _currentState;

        /// <summary>
        /// initialises traffic light.
        /// Sets the cycletime and phase data
        /// </summary>
        public TrafficLight()
        {
            try
            {
                ReadCycleTime();
                ReadPhaseData();
                _currentState = new CurrentState(_phaseList);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error initialising traffic light", ex);
            }
        }

        /// <summary>
        /// Reads the cycle time from the config file
        /// </summary>
        private void ReadCycleTime()
        {
            try
            {
                _cycleTime = ConfigReader.GetDoubleAppSetting("CyCleTime");
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error reading cycle time from config", ex);
            }            
        }

        /// <summary>
        /// Reads phase data from config file for each phase
        /// </summary>
        private void ReadPhaseData()
        {
            try
            {
                _phaseList = new List<Phase>();
                for (int i = 0; i <= 3; i++)
                {
                    _phaseList.Add(new Phase(_cycleTime, i));
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error reading phase data from App.config", ex);
            }
        }

        /// <summary>
        /// Starts the timer 
        /// </summary>
        public void Start()
        {
            try
            {                
                SignalTimer.Start(_currentState.IntervalTime);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error starting timer", ex);
            }            
        }

        /// <summary>
        /// Pauses timer
        /// </summary>
        public void Pause()
        {
            try
            {
                SignalTimer.PauseTimer();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error pausing timer", ex);
            }            
        }

        /// <summary>
        /// Restarts timer
        /// </summary>
        public void Restart()
        {
            try
            {
                SignalTimer.RestartTimer(_currentState.IntervalTime);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error restarting the timer", ex);
            }            
        }

        /// <summary>
        /// To be called when a phase (interval time reached) is complete
        /// </summary>
        public void PhaseComplete()
        {
            try
            {
                Pause();
                _currentState.ChangeCurrentPhase();
                ChangeLight();
                Restart();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error when phase complete", ex);
            }
        }

        /// <summary>
        /// Changes the lights by sending post to infastructure
        /// </summary>
        private void ChangeLight()
        {
            try
            {
                Console.WriteLine("Change Lights to State: " + _currentState.Phase.ToString() + "Interval Time: " + _currentState.IntervalTime.ToString());
                PhasePost changeLights = new PhasePost(_currentState);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error when changing the light", ex);
            }            
        }

        /// <summary>
        /// Adjusts the light timing depending on vehicle entering or leaving section
        /// </summary>
        /// <param name="vehicleData">data of vehicle entering sensor</param>
        /// <param name="routes">routes of junction</param>
        public void AdjustTiming(VehicleData vehicleData, List<Route> routes)
        {
            try
            {
                _currentState.TimeElapsed += SignalTimer.TimeElapsed;
                SetSignalTiming(routes);
                Console.WriteLine("Current Interval time: " + _currentState.IntervalTime.ToString());
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting light timing", ex);
            }
        }

        /// <summary>
        /// Sets lights time
        /// </summary>
        /// <param name="routes">routes of junction</param>
        private void SetSignalTiming(List<Route> routes)
        {

            try
            {
                CalculatePhaseTimes(routes);
                _currentState.SetCurrentIntervalTime(_currentState.Phase.Time);
                
                if (_currentState.TimeElapsed < _currentState.IntervalTime)
                {
                    _currentState.SetCurrentIntervalTime(_currentState.IntervalTime - _currentState.TimeElapsed);
                }
                else
                {
                    _currentState.ChangeCurrentPhase();
                    ChangeLight();
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("error setting light time", ex);
            }            
        }

        /// <summary>
        /// Calculates phase times
        /// </summary>
        /// <param name="routes">routes of junction</param>
        private void CalculatePhaseTimes(List<Route> routes)
        {
            try
            {
                if (routes[0].Pollution == 0 && routes[1].Pollution == 0)
                {
                    SetDefaultPhaseTime();
                }
                else if (routes[0].Pollution == 0)
                {
                    SetRoute1FullPriority();
                }
                else if (routes[1].Pollution == 0)
                {
                    SetRoute0FullPriority();
                }
                else
                {
                    SetWeightedSignalTime(routes);
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error calculating and setting phase times", ex);
            }            
        }

        /// <summary>
        /// Sets the time for each phase to its default
        /// </summary>
        private void SetDefaultPhaseTime()
        {
            try
            {
                foreach (Phase phase in _phaseList)
                {
                    phase.SetTimeToDefault();
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting phases to default time", ex);
            }
        }

        /// <summary>
        /// Sets the timing of the light so Route 1 has full priority
        /// </summary>
        private void SetRoute1FullPriority()
        {
            try
            {
                _phaseList[0].SetTime(_phaseList[0].MinTimePercent);
                _phaseList[2].SetTime(_phaseList[2].MaxTimePercent);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error giving route1 full priority", ex);
            }
        }

        /// <summary>
        /// Sets the timing of the light so Route 0 has full priority
        /// </summary>
        private void SetRoute0FullPriority()
        {
            try
            {
                _phaseList[0].SetTime(_phaseList[0].MaxTimePercent);
                _phaseList[2].SetTime(_phaseList[2].MinTimePercent);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error giving route0 full priority", ex);
            }
        }

        /// <summary>
        /// Calates the signal times by setting the times 
        /// to be in proportion to the pollution on the sections.
        /// </summary>
        /// <param name="routes">routes of junction</param>
        private void SetWeightedSignalTime(List<Route> routes)
        {
            try
            {
                double maxgreenTimeSum = _cycleTime - _phaseList[1].Time - _phaseList[3].Time;
                double totalPollution = routes[0].Pollution + routes[1].Pollution;
                double route0TimePercent = CalculateRouteTimePercent(routes[0], maxgreenTimeSum, totalPollution);
                double route1TimePercent = CalculateRouteTimePercent(routes[1], maxgreenTimeSum, totalPollution);

                _phaseList[0].SetTime(route0TimePercent);
                _phaseList[2].SetTime(route1TimePercent);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting weighted light times", ex);
            }
        }

        /// <summary>
        /// Calculate the percentage for green time for a route
        /// </summary>
        /// <param name="route">routes of junction</param>
        /// <param name="maxgreenTimeSum">maximum available green time for the phase</param>
        /// <returns></returns>
        private double CalculateRouteTimePercent(Route route, double maxgreenTimeSum, double totalPollution)
        {
            try
            {               
                return (route.Pollution * maxgreenTimeSum) / (_cycleTime * totalPollution);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error calculating roue time percent", ex);
            }
        }

        /// <summary>
        /// Adjust the config for the lights
        /// </summary>
        /// <param name="config">configuration dictionary</param>
        public void AdjustConfig(Dictionary<string, string> config)
        {
            try
            {
                _cycleTime = Convert.ToDouble(config["CycleTime"]);
            }
            catch (Exception ex)
            {
                throw new JunctionException("", ex);
            }
        } 
    }
}
