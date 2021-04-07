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
    /// <Schedule Frequency="Interval" StartTime="Apr 6, 2021 9:40 AM">
    ///    <RunTimes>
    ///       <RunTime Value="00:15:05"></RunTime>
    ///    </RunTimes>
    /// </Schedule>
    /// </summary>
    public class IntervalScheduleProvider : BaseScheduleProvider
    {
        private readonly TimeSpan timeBetweenRunTimes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startDate">The first instance to start from</param>
        /// <param name="evalDate">The Now Date -- added so can mock up tests</param>
        /// <param name="timeBetweenTrigger">The interval between run times</param>
        public IntervalScheduleProvider(DateTime startDate, IClock evalDate, TimeSpan timeBetweenTrigger) : base(startDate, evalDate)
        {
            IntervalType = FrequencyInterval.Interval;
            timeBetweenRunTimes = timeBetweenTrigger;
        }
        /// <summary>
        /// Calculates the time tho the interval occures
        /// </summary>
        /// <remarks>
        /// Remember that EvalDate is just DateTime.Now but is 
        /// used for allowing Test Driven Development (TDD)
        /// </remarks>
        public override DateTime NextRunTime
        {
            get
            {
                if (StartDate > EvalDate) return StartDate;
                var timeDiff = EvalDate - StartDate;
                /**************************************************************************************
                 * PROGRAMMER'S NOTE: You cannot divide TimeSpan objects to get the number of times
                 * an interval occured. You have to convert everyting to ticks
                 * and then back to a TimeSpan object. From the TimeSpan object
                 * we can then add it from a '0' DateTime object to return a DateTime result
                 * ************************************************************************************/
                var numOfTimesIntervalWasAlreadyDone = (int)(timeDiff.Ticks / timeBetweenRunTimes.Ticks);
                return new DateTime() + TimeSpan.FromTicks(StartDate.Ticks + (numOfTimesIntervalWasAlreadyDone + 1) * timeBetweenRunTimes.Ticks);
            }
        }
    }
}
