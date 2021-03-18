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
    /// Calculates the week day run
    /// </summary>
    /// <remarks>
    /// Class: WeeklyScheduleProvider.cs
    /// Author:
    /// How it works: Simply starts from the start date and checks if any of the days of 
    /// week is specified, if not increase by a day until we hit one. For example, if we 
    /// have specified Monday, Wednesday and Friday and the specified date to start from is 
    /// on Sunday, we iterate through the list of Monday, Wednesday and Friday, see that 
    /// non fit. Add a day which makes it Monday. We then iterate thruough specified day
    /// list and find that it does hit Monday, so return the date of whatever the Monday is.
    /// 
    /// Note: That the start date's time is important as that is when Next Run Time will
    /// be calculated from.
    /// </remarks>
    public class WeeklyScheduleProvider : BaseScheduleProvider
    {
        private readonly IList<DayOfWeek> daysOfWeek;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">The <see cref="FrequencyInterval"/> enumerated type</param>
        /// <param name="startDate">The first instance to start from</param>
        /// <param name="evalDate">The Now Date -- added so can mock up tests</param>
        /// <param name="days">The list of the <see cref="DayOfWeek"/> enumerated type of days we want to return 
        /// a next run time to calculate on</param>
        public WeeklyScheduleProvider(FrequencyInterval interval, DateTime startDate, IClock evalDate, IEnumerable<DayOfWeek> days) : base(interval, startDate, evalDate)
        {
            daysOfWeek = days.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Calculates the next date that should trigger an action
        /// </summary>
        /// <remarks>
        /// Remember that EvalDate is just DateTime.Now but is 
        /// used for allowing Test Driven Development (TDD)
        /// </remarks>
        public override DateTime NextRunTime
        {
            get
            {
                if (daysOfWeek == null || daysOfWeek.Count == 0)
                    throw new InvalidOperationException("no weekdays defined");

                DateTime current = StartDate > EvalDate
                    ? StartDate
                    : EvalDate.Date.Add(StartDate.TimeOfDay);

                while (true)
                {
                    if (daysOfWeek.Any(daysOfWeek => current.DayOfWeek == daysOfWeek && current.TimeOfDay <= EvalDate.TimeOfDay))
                    {
                        return current;
                    }
                    current = current.AddDays(1);
                }
            }
        }
    }
}
