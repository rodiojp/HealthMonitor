using System;
using System.Collections.Generic;
using System.Text;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.Services.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HealthMonitor.Tests.SchedulingTests
{
    /// <summary>
    /// BaseScheduleProvider tests and common methods for derived tests
    /// </summary>
    [TestClass]
    public class BaseScheduleProviderTests
    {
        public void RunTest(DateTime startDate, DateTime expectedResult, BaseScheduleProvider scheduleProvider)
        {
            //Act
            var result = scheduleProvider.NextRunTime;
            //Assert
            DoTestsAnalysis(expectedResult, result);
        }

        protected void DoTestsAnalysis(DateTime expectedResult, DateTime result)
        {
            Assert.AreEqual(expectedResult.Date, result.Date);
            Assert.AreEqual(expectedResult.Hour, result.Hour);
            Assert.AreEqual(expectedResult.Minute, result.Minute);
            Assert.AreEqual(expectedResult.Second, result.Second);
        }

        protected IClock BuildClock(DateTime aDate)
        {
            var clock = new Mock<IClock>(MockBehavior.Strict);
            clock.Setup(x => x.Now).Returns(aDate);
            return clock.Object;
        }
      
    }
}
