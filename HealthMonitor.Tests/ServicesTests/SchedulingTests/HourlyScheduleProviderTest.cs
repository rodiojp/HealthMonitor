using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.ServicesTests.SchedulingTests
{
    [TestClass]
    public class HourlyScheduleProviderTest : HelperScheduleProviderTests
    {
        [TestMethod]
        public void HourlyScheduleProvider_IntervalType_Is_FrequencyInterval_Hourly()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow);
            var interval = new[] { new TimeSpan(aDateTimeNow.Hour + 1, aDateTimeNow.Minute, 0) };
            var scheduleProvider = new HourlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            Assert.IsTrue(scheduleProvider.IntervalType == FrequencyInterval.Hourly);
        }
        [TestMethod]
        public void HourlyScheduleProvider_StartDate_NextHour_ExpectedResult_NextRunTime_NextHour()
        {
            //  start  Expected
            //   Now   Now+1Hour
            //____o________o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var interval = new[] { new TimeSpan(aDateTimeNow.Hour + 1, aDateTimeNow.Minute, 0) };
            var expectedResult = aDateTimeNow.Date.Add(interval[0]);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new HourlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void HourlyScheduleProvider_StartDate_2DaysAgo_ExpectedResult_NextRunTime_TomorrowAt2Am()
        {
            //     start        Expected
            //   Now-2Days   Tomorrow at 2AM
            //_______o_____________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-2);
            var interval = new[] { new TimeSpan(2, 0, 0) };
            var expectedResult = aDateTimeNow.Date.AddDays(1).Add(interval[0]);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new HourlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void HourlyScheduleProvider_StartDate_TodayAt3Am_ExpectedResult_NextRunTime_TodayAt5Am()
        {
            //       start           Expected
            // 2021-07-05 03:16   Today at 5AM
            //_________o_________________o______
            //setup
            var aDateTimeNow = new DateTime(2021, 7, 5, 3, 16, 0);
            var startDate = aDateTimeNow.AddDays(-2);
            var interval = new[] { new TimeSpan(1, 0, 0), new TimeSpan(3, 0, 0), new TimeSpan(5, 0, 0), new TimeSpan(23, 0, 0) };
            var expectedResult = aDateTimeNow.Date.Add(interval[2]);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new HourlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void HourlyScheduleProvider_StartDate_23HoursAgo_ExpectedResult_NextRunTime_TomorrowAt1Am()
        {
            //    start         Expected
            // Now-23Hours   Tomorrow at 1AM
            //______o_______________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(-1).AddHours(1);
            var interval = new[] { new TimeSpan(1, 0, 0) };
            var expectedResult = aDateTimeNow.Date.AddDays(1).Add(interval[0]);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new HourlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void HourlyScheduleProvider_StartDate_Tomorrow_ExpectedResult_NextRunTime_Tomorrow()
        {
            //        start & Expected
            //   Now  Now+1Day+2Hours  
            //____o__________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(1).AddHours(2);
            var interval = new[] { new TimeSpan(1, 0, 0) };
            var expectedResult = aDateTimeNow.Date.AddDays(2).AddHours(interval[0].Hours);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new HourlyScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
