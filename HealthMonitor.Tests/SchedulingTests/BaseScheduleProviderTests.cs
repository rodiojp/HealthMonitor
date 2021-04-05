using System;
using System.Collections.Generic;
using System.Text;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HealthMonitor.Tests.SchedulingTests
{
    /// <summary>
    /// Unit Tests for BaseScheduleProvider class
    /// </summary>
    [TestClass]
    public class BaseScheduleProviderTests : HelperScheduleProviderTests
    {
        [TestMethod]
        public void BaseScheduleProvider_StartDate_Now_ExpectedResult_NextRunTime_IsTomorrow()
        {
            //setup
            var aDateTimeNow = DateTime.Now;
            var startDate = aDateTimeNow;
            var expectedResult = aDateTimeNow.Date.AddDays(1);
            var evalDate = BuildClock(aDateTimeNow);
            var scheduleProvider = new BaseScheduleProvider(FrequencyInterval.Monthly, startDate, evalDate);
            //Assert
            RunTest(expectedResult, scheduleProvider);
        }
    }
}
