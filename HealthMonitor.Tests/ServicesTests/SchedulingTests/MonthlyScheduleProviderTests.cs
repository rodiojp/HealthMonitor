using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.ServicesTests.SchedulingTests
{
    /// <summary>
    /// <Schedule Frequency="Monthly" StartTime="Apr 6, 2021 9:40 AM">
    ///    <RunTimes>
    ///       <RunTime Value="1"></RunTime>
    ///    </RunTimes>
    /// </Schedule>
    /// </summary>
    [TestClass]
    public class MonthlyScheduleProviderTests : HelperScheduleProviderTests
    {
        [TestMethod]
        public void MonthlyScheduleProvider_IntervalType_Is_FrequencyInterval_Monthly()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow.Date);
            var interval = new[] { aDateTimeNow.AddDays(1).Day };
            var scheduleProvider = new MonthlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            Assert.IsTrue(scheduleProvider.IntervalType == FrequencyInterval.Monthly);
        }
        /// <summary>
        /// class MonthlyScheduleProvider get NextRunTime if StartDate in future
        /// </summary>
        [TestMethod]
        public void MonthlyScheduleProvider_StartDate_Tomorrow_ExpectedResult_NextRunTime_Tomorrow()
        {
            //  start     Expected
            //   Now   Now+1Month+1Day  
            //____o__________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(-1).Day };
            var expectedResult = aDateTimeNow.AddMonths(1).AddDays(-1);
            var scheduleProvider = new MonthlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class MonthlyScheduleProvider get NextRunTime if StartDate in the past
        [TestMethod]
        public void MonthlyScheduleProvider_StartDate_Yesterday_ExpectedResult_NextRunTime_Tomorrow()
        {
            //       start          Expected
            //      Now-1Day   Now  Now+1Day
            //_________o________o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(1).Day };
            var expectedResult = aDateTimeNow.AddDays(1);
            var scheduleProvider = new MonthlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class MonthlyScheduleProvider get NextRunTime if StartDate in the past
        /// </summary>
        [TestMethod]
        public void MonthlyScheduleProvider_StartDate_23HoursAgo_NextRunTime_1HourAfterNow()
        {
            //       start              Expected
            //  Now-1Day+1Hour     Now  Now+1Hour
            //_________o____________o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(1).Day };
            var expectedResult = aDateTimeNow.AddDays(1).AddHours(1);
            var scheduleProvider = new MonthlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void MonthlyScheduleProvider_StartDate_25HoursAgo_NextRunTime_23HourAfterNow()
        {
            //       start              Expected
            //  Now-1Day-1Hour   Now  Now+1Day-1Hour
            //_________o__________o_________o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(-1);
            var expectedResult = aDateTimeNow.AddDays(1).AddHours(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(1).Day };
            var scheduleProvider = new MonthlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
