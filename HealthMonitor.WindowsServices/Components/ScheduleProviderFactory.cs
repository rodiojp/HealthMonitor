using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Configuration.Interfaces;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Common;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.Services.Scheduling;
using HealthMonitor.WindowsServices.Common;

namespace HealthMonitor.WindowsServices.Components
{
    /// <summary>
    /// Factory Pattern to produce ScheduleProvider's depending on parameters
    /// </summary>
    /// <remarks>
    /// Class: ScheduleProviderFactory.cs
    /// </remarks>
    /// 
    public class ScheduleProviderFactory
    {
        public static IScheduleProvider Build(IScheduleParameters schedule)
        {
            IScheduleProvider result;
            switch (schedule.Frequency)
            {
                case "Monthly":
                    result = BuildMonthlyScheduleProvider(schedule);
                    break;
                case "Daily":
                    result = BuildDailyScheduleProvider(schedule);
                    break;
                case "Hourly":
                    result = BuildHourlyScheduleProvider(schedule);
                    break;
                case "Interval":
                    result = BuildIntervalScheduleProvider(schedule);
                    break;
                case "Weekly":
                    result = BuildWeeklyScheduleProvider(schedule);
                    break;
                default:
                    throw new ScheduleProviderException(schedule.Frequency);
            }
            return result;
        }

        /// <summary>
        /// Builds a WeeklyScheduleProvider
        /// </summary>
        /// <param name="schedule">Any implementation of <see cref="IScheduleProvider"/> object</param>
        /// <returns>An instantiated <see cref="WeeklyScheduleProvider"/> object</returns>
        private static IScheduleProvider BuildWeeklyScheduleProvider(IScheduleParameters schedule)
        {
            var startTime = DateTime.Parse(schedule.StartTime);
            var daysOfWeek = schedule.TimeParameters.Select(x=>(DayOfWeek)Enum.Parse(typeof(DayOfWeek), x));
            return new WeeklyScheduleProvider(FrequencyInterval.Weekly, startTime, new CurrentClock(), daysOfWeek);
        }

        /// <summary>
        /// Builds a IntervalScheduleProvider
        /// </summary>
        /// <param name="schedule">Any implementation of <see cref="IScheduleProvider"/> object</param>
        /// <returns>An instantiated <see cref="IntervalScheduleProvider"/> object</returns>
        private static IScheduleProvider BuildIntervalScheduleProvider(IScheduleParameters schedule)
        {
            var startTime = DateTime.Parse(schedule.StartTime);
            var interval = TimeSpan.Parse(schedule.TimeParameters.First());
            return new IntervalScheduleProvider(FrequencyInterval.Interval, startTime, new CurrentClock(), interval);
        }

        /// <summary>
        /// Builds a HourlyScheduleProvider
        /// </summary>
        /// <param name="schedule">Any implementation of <see cref="IScheduleProvider"/> object</param>
        /// <returns>An instantiated <see cref="HourlyScheduleProvider"/> object</returns>
        private static IScheduleProvider BuildHourlyScheduleProvider(IScheduleParameters schedule)
        {
            var startTime = DateTime.Parse(schedule.StartTime);
            var timesToRun = schedule.TimeParameters.Select(TimeSpan.Parse);
            return new HourlyScheduleProvider(FrequencyInterval.Hourly, startTime, new CurrentClock(), timesToRun);
        }
        /// <summary>
        /// Builds a DailyScheduleProvider
        /// </summary>
        /// <param name="schedule">Any implementation of <see cref="IScheduleProvider"/> object</param>
        /// <returns>An instantiated <see cref="DailyScheduleProvider"/> object</returns>
        private static IScheduleProvider BuildDailyScheduleProvider(IScheduleParameters schedule)
        {
            return new DailyScheduleProvider(FrequencyInterval.Daily, DateTime.Parse(schedule.StartTime), new CurrentClock());
        }
        /// <summary>
        /// Builds a MonthlyScheduleProvider
        /// </summary>
        /// <param name="schedule">Any implementation of <see cref="IScheduleProvider"/> object</param>
        /// <returns>An instantiated <see cref="MonthlyScheduleProvider"/> object</returns>
        private static IScheduleProvider BuildMonthlyScheduleProvider(IScheduleParameters schedule)
        {
            var startTime = DateTime.Parse(schedule.StartTime);
            var daysToRun = schedule.TimeParameters.Select(int.Parse);
            return new MonthlyScheduleProvider(FrequencyInterval.Monthly, startTime, new CurrentClock(), daysToRun);
        }
    }
}
