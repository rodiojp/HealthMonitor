using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.ServicesTests.SchedulingTests
{
    /// <summary>
    /// <Schedule Frequency="Weekly" StartTime="Apr 6, 2021 9:40 AM">
    ///    <RunTimes>
    ///       <RunTime Value="Monday"></RunTime>
    ///    </RunTimes>
    /// </Schedule>
    /// </summary>
    [TestClass]
    public class WeeklyScheduleProviderTests : HelperScheduleProviderTests
    {
        [TestMethod]
        public void WeeklyScheduleProvider_IntervalType_Is_FrequencyInterval_Weekly()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.DayOfWeek };
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            Assert.IsTrue(scheduleProvider.IntervalType == FrequencyInterval.Weekly);
        }
        /// <summary>
        /// class WeeklyScheduleProvider get NextRunTime if StartDate in future
        /// </summary>
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_Tomorrow_ExpectedResult_NextRunTime_Tomorrow()
        {
            //  start     Expected
            //   Now   Now+1Month+1Day  
            //____o__________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(-1).DayOfWeek };
            var expectedResult = aDateTimeNow.AddDays(6);
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class WeeklyScheduleProvider get NextRunTime if StartDate in the past
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_Yesterday_ExpectedResult_NextRunTime_Tomorrow()
        {
            //       start          Expected
            //      Now-1Day   Now  Now+1Day
            //_________o________o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(1).DayOfWeek };
            var expectedResult = aDateTimeNow.AddDays(1);
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class WeeklyScheduleProvider get NextRunTime if StartDate in the past
        /// </summary>
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_23HoursAgo_NextRunTime_1HourAfterNow()
        {
            //       start              Expected
            //  Now-1Day+1Hour     Now  Now+1Hour
            //_________o____________o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(1).DayOfWeek };
            var expectedResult = aDateTimeNow.AddDays(1).AddHours(1);
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_25HoursAgo_NextRunTime_23HourAfterNow()
        {
            //       start              Expected
            //  Now-1Day-1Hour   Now  Now+1Day-1Hour
            //_________o__________o_________o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(-1);
            var expectedResult = aDateTimeNow.AddDays(1).AddHours(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(1).DayOfWeek };
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_25HoursAgo_NextRunTime_23HourAfterNow_2DaysOfWeek()
        {
            //       start      1Day             Expected
            //  Now-1Day-1Hour Now-1Day    Now  Now+1Day-1Hour
            //_________o__________o_________o_________o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.AddDays(-1).DayOfWeek, aDateTimeNow.AddDays(3).DayOfWeek };
            var expectedResult = aDateTimeNow.AddDays(3).AddHours(-1);
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_1HoursAgo_NextRunTime_OneWeekAfterNow()
        {
            //    start           Expected
            //  Now-1Hour Now  Now+7Days-1Hour
            //______o______o_________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddHours(-1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.DayOfWeek };
            var expectedResult = aDateTimeNow.AddDays(7).AddHours(-1);
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void WeeklyScheduleProvider_StartDate_1HourPlus_NextRunTime_1HourAfterNow()
        {
            //    start        Expected
            //  Now+1Hour Now  Now+1Hour
            //______o______o_______o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddHours(1);
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { aDateTimeNow.DayOfWeek };
            var expectedResult = aDateTimeNow.AddHours(1);
            var scheduleProvider = new WeeklyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
