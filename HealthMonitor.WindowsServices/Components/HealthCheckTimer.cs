using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HealthMonitor.Services.Interfaces;

namespace HealthMonitor.WindowsServices.Components
{
    public class HealthCheckTimer:Timer
    {
        private readonly IScheduleProvider scheduleProvider;
        /// <summary>
        /// Constructor to the HealthCheck Timer
        /// </summary>
        /// <param name="scheduleProvider">
        /// an implementation of an <see cref="IScheduleProvider"/> object
        /// </param>
        public HealthCheckTimer(IScheduleProvider scheduleProvider)
        {
            this.scheduleProvider = scheduleProvider;
            Interval = scheduleProvider.GetInterval;
        }
        /// <summary>
        /// Stop and Recalculate
        /// </summary>
        public void Reset()
        {
            Stop();
            scheduleProvider.Reset();
            Interval = scheduleProvider.GetInterval;
            Enabled = true;
            Start();
        }
        /// <summary>
        /// Next time the elapsed event will occur
        /// </summary>
        /// <returns>DateTime of next elapsed time</returns>
        public DateTime NextRunTime => scheduleProvider.NextRunTime;
        

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }

    }
}
