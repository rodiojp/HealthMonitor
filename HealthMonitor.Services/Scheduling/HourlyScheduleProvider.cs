using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Interfaces;

namespace HealthMonitor.Services.Scheduling
{
    public class HourlyScheduleProvider : BaseScheduleProvider
    {
        private readonly IList<TimeSpan> runTimes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startDate">The first instance to start from</param>
        /// <param name="evalDate">The Now Date -- added so can mock up tests</param>
        /// <param name="times">The <see cref="TimeSpan"/> list of times to trigger a next run</param>
        public HourlyScheduleProvider(DateTime startDate, IClock evalDate, IEnumerable<TimeSpan> times) : base(startDate, evalDate)
        {
            IntervalType = FrequencyInterval.Hourly;
            runTimes = times.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Calculates the Hourly Next Run Time
        /// </summary>
        /// <remarks>
        /// Remember that EvalDate is just DateTime.Now but is 
        /// used for allowing Test Driven Development (TDD)
        /// </remarks>
        public override DateTime NextRunTime
        {
            get
            {
                if (runTimes == null || runTimes.Count == 0)
                    throw new InvalidOperationException("no hours defined");

                DateTime startTime = StartDate > EvalDate
                    ? StartDate
                    : EvalDate;
                DateTime current = startTime.Date;
                while (true)
                {
                    foreach (var time in runTimes)
                    {
                        DateTime result = current.Add(time);
                        if (result >= startTime)
                            return result;
                    }
                    current = current.AddDays(1);
                }
            }
        }
    }
}
