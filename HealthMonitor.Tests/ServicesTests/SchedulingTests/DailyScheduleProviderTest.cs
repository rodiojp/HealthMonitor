using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.ServicesTests.SchedulingTests
{
    [TestClass]
    public class DailyScheduleProviderTest : HelperScheduleProviderTests
    {
        [TestMethod]
        public void DailyScheduleProvider_IntervalType_Is_FrequencyInterval_Daily()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new DailyScheduleProvider(startDate, evalDate);
            //Assert
            Assert.IsTrue(scheduleProvider.IntervalType == FrequencyInterval.Daily);
        }
        /// <summary>
        /// class DailyScheduleProvider get NextRunTime if StartDate in future
        /// </summary>
        [TestMethod]
        public void DailyScheduleProvider_StartDate_Tomorrow_ExpectedResult_NextRunTime_Tomorrow()
        {
            //       start & Expected
            //   Now    Now+1Day  
            //____o________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(1);
            var expectedResult = aDateTimeNow.AddDays(1);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new DailyScheduleProvider(startDate, evalDate);
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
            var scheduleProvider = new DailyScheduleProvider(startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        /// <summary>
        /// class DailyScheduleProvider get NextRunTime if StartDate in the past
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
            var scheduleProvider = new DailyScheduleProvider(startDate, evalDate);
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
            var scheduleProvider = new DailyScheduleProvider(startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
