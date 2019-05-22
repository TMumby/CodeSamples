using System;

namespace JunctionController
{
    /// <summary>
    /// Responsible for holding timing data for a phase
    /// </summary>
    public class Phase
    {
        private double _defaultTimePercent; //default percentage of cycle time for phase
        private double _phaseTimePercent; //current percentage of cycle time for phase
        private double _cycleTime; //time for all phases to complete

        public int PhaseNo { get; private set; }
        public string Definition { get; private set; }
        public double Time { get; private set; }        
        public double MaxTimePercent { get; private set; }
        public double MinTimePercent { get; private set; }



        /// <summary>
        /// Initialises Phase class, calclulates timings from timing percentages and sets timing
        /// </summary>
        /// <param name="cycleTime">time for all phases to complete</param>
        /// <param name="defaultTimePercent">default percentage of cycle time for phase</param>
        /// <param name="minTimePercent">minimum percentage of cycle time for phase</param>
        /// <param name="maxTimePercent">maximum percentage of cyle time for phase</param>
        public Phase(double cycleTime, int phaseNo)
        {
            try
            {                
                _cycleTime = cycleTime;
                SetProperties(phaseNo);
                SetTimeToDefault();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error initialising class", ex);
            }            
        }

        /// <summary>
        /// Sets properties by reading config file
        /// </summary>
        private void SetProperties(int phaseNo)
        {
            try
            {
                PhaseNo = phaseNo;
                _defaultTimePercent = ConfigReader.GetDoubleAppSetting("Phase" + phaseNo.ToString() + "DefaultTimePercent");
                MinTimePercent = ConfigReader.GetDoubleAppSetting("Phase" + phaseNo.ToString() + "MinTimePercent");
                MaxTimePercent = ConfigReader.GetDoubleAppSetting("Phase" + phaseNo.ToString() + "MaxTimePercent");
                Definition = ConfigReader.GetAppSettingString("Phase" + phaseNo.ToString() + "Definition");
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting phase properties", ex);
            }
        }
        /// <summary>
        /// Sets phase time to default time for phase
        /// </summary>
        public void SetTimeToDefault()
        {
            try
            {
                SetTime(_defaultTimePercent);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting phase time to default time for phase", ex);
            }            
        }

        /// <summary>
        /// Sets phase time according to a percentage of cycle time
        /// </summary>
        /// <param name="phaseTimePercent">percentage of cycle time to set phase time to</param>
        public void SetTime (double phaseTimePercent)
        {
            try
            {
                SetPhaseTimePercent(phaseTimePercent);
                Time = _cycleTime * _phaseTimePercent;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting phase time", ex);
            }            
        }

        /// <summary>
        /// Sets the phase time percent
        /// Checks the percent is within the 
        /// constraints of maximum and minum percent times
        /// and adjusts accordingly
        /// </summary>
        /// <param name="phaseTimePercent">percentage of cycle time to set phase time to</param>
        private void SetPhaseTimePercent(double phaseTimePercent)
        {
            try
            {
                if (phaseTimePercent > MaxTimePercent)
                {
                    _phaseTimePercent = MaxTimePercent;
                }
                else if (phaseTimePercent < MinTimePercent)
                {
                    _phaseTimePercent = MinTimePercent;
                }
                else
                {
                    _phaseTimePercent = phaseTimePercent;
                }
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting percentage of cycle time for phase", ex);
            }
        }            
    }
}
