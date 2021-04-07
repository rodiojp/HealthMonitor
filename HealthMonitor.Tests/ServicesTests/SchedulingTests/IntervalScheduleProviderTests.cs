using System;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.ServicesTests.SchedulingTests
{
    /// <summary>
    ///    <Schedule Frequency="Interval" StartTime="Apr 6, 2021 9:40 AM">
    ///       <RunTimes>
    ///          <RunTime Value="00:15:05"></RunTime>
    ///       </RunTimes>
    ///    </Schedule>
    /// </summary>
    [TestClass]
    public class IntervalScheduleProviderTests : HelperScheduleProviderTests
    {
        [TestMethod]
        public void IntervalScheduleProvider_IntervalType_Is_FrequencyInterval_Interval()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var evalDate = BuildClock(aDateTimeNow);
            var timeSpan = new TimeSpan(0, 15, 0);
            var scheduleProvider = new IntervalScheduleProvider(startDate, evalDate, timeSpan);
            //Assert
            Assert.IsTrue(scheduleProvider.IntervalType == FrequencyInterval.Interval);
        }
        [TestMethod]
        public void IntervalScheduleProvider_ExpectedResult_NextRunTime_FifteenMinutesFromStart()
        {
            //  start   Now     Expected
            //  11min  23min  11min+15min
            //____o______o_________o______
            //setup
            var aDateTimeNow = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(23);
            var startDate = aDateTimeNow.Date.AddHours(aDateTimeNow.Hour).AddMinutes(11);
            var interval = new TimeSpan(0, 15, 0);
            var expectedResult = aDateTimeNow.Date.AddHours(aDateTimeNow.Hour).AddMinutes(startDate.Minute + interval.Minutes);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new IntervalScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void IntervalScheduleProvider_ExpectedResult_NextRunTime_15MinutesFromNow()
        {
            //  start   Expected
            //   Now    Now+15min
            //____o________o______
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var interval = new TimeSpan(0, 15, 0);
            var expectedResult = aDateTimeNow.AddMinutes(interval.Minutes);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new IntervalScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void IntervalScheduleProvider_ExpectedResult_NextRunTime_17HoursFromStart()
        {
            //      Start           Expected
            //  Now-12Hours  Now  Start+17Hours
            //________o_______o________o____
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddHours(-12);
            var interval = new TimeSpan(17, 0, 0);
            var expectedResult = startDate.AddHours(interval.Hours);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new IntervalScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
        [TestMethod]
        public void IntervalScheduleProvider_StartDate_Tomorrow_ExpectedResult_NextRunTime_Tomorrow()
        {
            //        start & Expected
            //   Now  Now+1Day+2Hours  
            //____o__________o_________
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow.AddDays(1).AddHours(2);
            var interval = new TimeSpan(6, 0, 0);
            var expectedResult = startDate;
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new IntervalScheduleProvider(startDate, evalDate, interval);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
