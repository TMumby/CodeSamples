using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// Defines the current phase state
    /// </summary>
    public class CurrentState
    {
        public double IntervalTime { get; private set; }
        public Phase Phase { get; private set; }

        public long TimeElapsed { get; set; }

        private List<Phase> _phaseList;

        /// <summary>
        /// Initialises class, sets initial interval time
        /// </summary>
        /// <param name="phaseList">list of phases</param>
        public CurrentState( List<Phase> phaseList)
        {
            try
            {
                _phaseList = phaseList;
                Phase = _phaseList[0];
                TimeElapsed = 0;
                SetCurrentIntervalTime(phaseList[0].Time);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error initialising Current State", ex);
            }
        } 
        
        /// <summary>
        /// Set the interval time
        /// </summary>
        /// <param name="intervalTime">interval time to update to</param>
        public  void SetCurrentIntervalTime(double intervalTime)
        {
            try
            {
                IntervalTime = intervalTime;                
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error setting interval time", ex);
            }            
        }

        /// <summary>
        /// change the current phase to the next phase,
        /// and sets the interval time
        /// </summary>        
        public void ChangeCurrentPhase()
        {
            try
            {
                ChangePhase();
                SetCurrentIntervalTime(Phase.Time);
                TimeElapsed = 0;
            }
            catch (Exception ex)
            {
                throw new JunctionException("error changing phase and setting interval time", ex);
            }
        }

        /// <summary>
        /// change the current phase to the next phase
        /// </summary>
        private void ChangePhase()
        {
            try
            {
                int newPhase = Phase.PhaseNo + 1;
                if (newPhase == 4)
                {
                    newPhase = 0;
                }
                Phase = _phaseList[newPhase];
            }
            catch (Exception ex)
            {
                throw new JunctionException("error changing phase to next phase", ex);
            }
        }
    }
}
