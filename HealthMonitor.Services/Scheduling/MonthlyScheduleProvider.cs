using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Interfaces;

namespace HealthMonitor.Services.Scheduling
{
    public class MonthlyScheduleProvider : BaseScheduleProvider
    {
        private readonly IList<int> days;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">The <see cref="FrequencyInterval"/> enumerated type</param>
        /// <param name="startDate">The first instance to start from</param>
        /// <param name="evalDate">The Now Date -- added so can mock up tests</param>
        /// <param name="days">The list of days in a month that should be provided a date from</param>
        public MonthlyScheduleProvider(FrequencyInterval interval, DateTime startDate, IClock evalDate, IEnumerable<int> days) : base(interval, startDate, evalDate)
        {
            this.days = days.OrderBy(x => x).ToList();
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
                if (days == null || days.Count == 0)
                    throw new InvalidOperationException("no run days defined");

                DateTime current = StartDate > EvalDate
                    ? StartDate
                    : EvalDate.Date.Add(StartDate.TimeOfDay);

                while (true)
                {
                    foreach (var day in days)
                    {
                        //watch out for scenarios where there's less days in a month than 
                        //the specified day. For example February has 28/29 days and
                        //if 30th day is specified
                        if (DateTime.DaysInMonth(current.Year, current.Month) <= day)
                        {
                            //Note how we substruct (day -1). The reason we do that is that we start with the 1st
                            //if we added a day scheduled on the 15th, adding 15 will put the date at 16 and hence
                            //(day -1)
                            DateTime result = new DateTime(current.Year, current.Month, 1).Add(StartDate.TimeOfDay).AddDays(day - 1);
                            if (result >= EvalDate)
                                return result;
                        }
                    }
                    current = current.AddMonths(1);
                }
            }
        }
    }
}
