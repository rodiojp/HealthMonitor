using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Interfaces;

namespace HealthMonitor.Services.Scheduling
{
    /// <summary>
    /// Base Class for Provider that will find the next time to flag as next
    /// </summary>
    /// <remarks>
    /// Class: BaseScheduleProvider.cs
    /// Author: 
    /// </remarks>
    public class BaseScheduleProvider : IScheduleProvider
    {
        protected readonly FrequencyInterval FrequencyInterval;
        protected readonly DateTime StartDate;
        /// <summary>
        /// This date will allways be DateTime.Now.
        /// However, for testing purposes, we pass this in as IClock.Now
        /// so we can manipulate it what DateTime.Now is
        /// </summary>
        protected DateTime EvalDate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">interval type</param>
        /// <param name="startDate">The first scheduled date to start from</param>
        /// <param name="evalDate">Date of Now</param>
        public BaseScheduleProvider(FrequencyInterval interval, DateTime startDate, IClock evalDate)
        {
            FrequencyInterval = interval;
            StartDate = startDate;
            EvalDate = evalDate.Now;
        }

        /// <summary>
        /// What kind of frequency interval
        /// </summary>
        public FrequencyInterval IntervalType => FrequencyInterval;

        /// <summary>
        /// Next time to trigger event in Milliseconds
        /// </summary>
        public virtual double GetInterval => (NextRunTime - DateTime.Now).TotalMilliseconds;

        /// <summary>
        /// Next time to trigger event
        /// </summary>
        public virtual DateTime NextRunTime => DateTime.Today.AddDays(1);

        public void Reset()
        {
            EvalDate = DateTime.Now;
        }

    }
}
