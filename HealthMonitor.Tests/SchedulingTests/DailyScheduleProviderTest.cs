using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.SchedulingTests
{
    [TestClass]
    public class DailyScheduleProviderTest : BaseScheduleProviderTests
    {
        /// <summary>
        /// class DailyScheduleProvider Construcor StartDate in future GetNextRunDay (Uses StartDate as NextRunDate)
        /// </summary>
        [TestMethod]
        public void DailyScheduleProvider_Construcor_StartDateInFuture()
        {
            //setup
            var aDate = DateTime.Now;
            var startDate = aDate.AddDays(1);
            var evalDate = BuildClock(aDate);
            var dailyScheduleProvider = new DailyScheduleProvider(FrequencyInterval.Monthly, startDate, evalDate);
            //Assert
            RunTest(startDate, startDate, dailyScheduleProvider);
        }
    }
}
