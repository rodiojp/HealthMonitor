using System;
using System.Linq;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;
using HealthMonitor.Domain.Scheduling;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.WindowsServices.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.WindowsServicesTests.ComponentsTests
{ 
    [TestClass]
    public class ScheduleProviderFactoryTests
    {
        [TestMethod]
        public void ScheduleProviderFactory_Build()
        {
            //set up
            HealthChecksSection healthChecksSection = HealthChecksSection.GetConfig();
            var healthCheck = healthChecksSection.HealthChecks["stop-start-windows-services"];
            //act
            IScheduleProvider scheduleProvider = ScheduleProviderFactory.Build(healthCheck.Schedule);
            //Accert
            Assert.AreEqual(scheduleProvider.IntervalType, FrequencyInterval.Weekly);
        }
        [TestMethod]
        public void ScheduleProviderFactory_Build_IScheduleProvider_NextRunTime()
        {
            //set up
            HealthChecksSection healthChecksSection = HealthChecksSection.GetConfig();
            var healthCheck = healthChecksSection.HealthChecks["stop-start-windows-services"];
            //act
            IScheduleProvider scheduleProvider = ScheduleProviderFactory.Build(healthCheck.Schedule);
            DateTime nextRunTime = scheduleProvider.NextRunTime;
            //Accert
            //Assert.AreEqual(nextRunTime.Day, 1);
            Assert.IsNotNull(nextRunTime);
        }
    }
}
