using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.SchedulingTests
{
    [TestClass]
    public class DailyScheduleProviderTest : HelperScheduleProviderTests
    {
        /// <summary>
        /// class DailyScheduleProvider get NextRunTime if StartDate in future
        /// </summary>
        [TestMethod]
        public void DailyScheduleProvider_StartDate_Tomorrow_ExpectedResult_NextRunTime_Tomorrow()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(1);
            var expectedResult = aDateTimeNow.AddDays(1);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new DailyScheduleProvider(FrequencyInterval.Monthly, startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class DailyScheduleProvider get NextRunTime if StartDate in the past
        [TestMethod]
        public void DailyScheduleProvider_StartDate_Yesterday_ExpectedResult_NextRunTime_Tomorrow()
        {
            //       start          Expected
            //      Now-1Day   Now  Now+1Day
            //_________o________o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1);
            var expectedResult = aDateTimeNow.AddDays(1);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new DailyScheduleProvider(FrequencyInterval.Monthly, startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class DailyScheduleProvider get NextRunTime if StartDate in the past (uses Nest as NextRunDate)
        /// </summary>
        [TestMethod]
        public void DailyScheduleProvider_StartDate_23HoursAgo_NextRunTime_1HourAfterNow()
        {
            //       start              Expected
            //  Now-1Day+1Hour     Now  Now+1Hour
            //_________o____________o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(1);
            var expectedResult = aDateTimeNow.AddHours(1);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new DailyScheduleProvider(FrequencyInterval.Monthly, startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void DailyScheduleProvider_StartDate_25HoursAgo_NextRunTime_23HourAfterNow()
        {
            //       start              Expected
            //  Now-1Day-1Hour   Now  Now+1Day-1Hour
            //_________o__________o_________o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(-1);
            var expectedResult = aDateTimeNow.AddDays(1).AddHours(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new DailyScheduleProvider(FrequencyInterval.Monthly, startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
