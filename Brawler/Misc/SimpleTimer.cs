using System;
using System.Diagnostics.PerformanceData;
using System.Timers;

namespace Brawler
{
    /// <summary>
    /// Simple one time timer
    /// </summary>
    public class SimpleTimer
    {
        public SimpleTimer(float seconds, Action func)
        {
            if (seconds <= 0)
                seconds = 0.000001f;

            Timer tim = new Timer();
            tim.Interval = TimeSpan.FromSeconds(seconds).TotalMilliseconds;
            tim.Enabled = true;
            tim.AutoReset = false;
            tim.Elapsed += delegate { func(); };
        }
    }   
}
