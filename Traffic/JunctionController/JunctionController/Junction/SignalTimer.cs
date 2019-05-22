using System;
using System.Timers;
using System.Diagnostics;

namespace JunctionController
{
    /// <summary>
    /// For timing phases.
    /// </summary>
    public static class SignalTimer
    {
        private static Timer _timer; //for signal timing
        
        private static Stopwatch stopWatch = new Stopwatch(); //for retrieving time elapsed

        public static long TimeElapsed { get; private set; }

        /// <summary>
        /// Starts the timer.
        /// Timer runs for specified time then triggers an event to show 
        /// signal phase is complete
        /// </summary>
        /// <param name="interval">how long for the timer to run</param>
        public static void Start(double interval)
        {
            try
            {
                _timer = new Timer(interval);
                _timer.Elapsed += new ElapsedEventHandler(PhaseComplete);
                _timer.Start();
                stopWatch.Start();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error starting signal timer", ex);
            }
        }

        /// <summary>
        /// Triggered when timer complete (phase complete)
        /// </summary>
        /// <param name="source">object source of event</param>
        /// <param name="e">data from event</param>
        private static void PhaseComplete(Object source, ElapsedEventArgs e)
        {
            try
            {
                Junction.PhaseComplete();
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error triggering phase complete by timer", ex);
            }            
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        public static void PauseTimer()
        {
            try
            {
                _timer.Stop();
                stopWatch.Stop();
                TimeElapsed = stopWatch.ElapsedMilliseconds;
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error pausing timer", ex);
            }

        }

        /// <summary>
        /// Restarts the timer
        /// </summary>
        /// <param name="interval">new interval time for timer</param>
        public static void RestartTimer(double interval)
        {
            try
            {
                _timer.Stop();
                _timer.Interval = interval;
                _timer.Enabled = true;
                stopWatch.Restart();
            }
            catch(Exception ex)
            {
                throw new JunctionException("Error restarting the timer", ex);
            }      
        }
    }
}
