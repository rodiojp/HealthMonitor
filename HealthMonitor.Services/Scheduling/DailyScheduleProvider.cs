using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Interfaces;

namespace HealthMonitor.Services.Scheduling
{
    public class DailyScheduleProvider : BaseScheduleProvider
    {
        public DailyScheduleProvider(DateTime startDate, IClock evalDate) : base(startDate, evalDate)
        {
            IntervalType = FrequencyInterval.Daily;
        }
        /// <summary>
        /// Calculates the Daily Next Run Time
        /// </summary>
        /// <remarks>
        /// Remember that EvalDate is just DateTime.Now but is 
        /// used for allowing Test Driven Development (TDD)
        /// </remarks>
        public override DateTime NextRunTime
        {
            get
            {
                DateTime result = StartDate > EvalDate
                    ? StartDate
                    : EvalDate.Date.Add(StartDate.TimeOfDay);
                return result > EvalDate ? result : result.AddDays(1);
            }
        }
    }
}
