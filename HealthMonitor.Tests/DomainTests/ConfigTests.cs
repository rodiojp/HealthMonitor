using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.DomainTests
{
    /// <summary>
    /// Summary description for ConfigTests
    /// </summary>
    [TestClass]
    public class ConfigTests
    {
        private HealthChecksSection healthChecksSection;
        public ConfigTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        [TestInitialize]
        public void Initialize()
        {
            healthChecksSection = HealthChecksSection.GetConfig();
        }
        /// <summary>
        /// class HealthChecksSection property HealthChecks Count
        /// </summary>
        [TestMethod]
        public void AppConfig_HealthChecksSection_HealthChecksCount()
        {
            //Accert
            Assert.AreEqual(3, healthChecksSection.HealthChecks.Count);
        }

        /// <summary>
        /// interface IHealthCheck can get value
        /// </summary>
        [TestMethod]
        public void AppConfig_IHealthCheck_ScheduleParams_CanGetValues()
        {
            var healthChecks = new List<IHealthCheck>();
            var checks = healthChecksSection.HealthChecks;
            for (int ii = 0; ii < checks.Count; ii++)
            {
                healthChecks.Add(checks[ii]);
            }
            var stopStartWindowsServices = healthChecks.First(x => x.Name.Equals("stop-start-windows-services"));
            //Accert
            Assert.IsNotNull(stopStartWindowsServices);
            Assert.AreEqual("00:05", stopStartWindowsServices.ScheduleParams.TimeParameters.First());
            Assert.AreEqual("May 1, 2020", stopStartWindowsServices.ScheduleParams.StartTime);
            Assert.AreEqual("Interval", stopStartWindowsServices.ScheduleParams.Frequency);
        }
        /// <summary>
        /// class HealthChecks returns all properties correctly
        /// </summary>
        [TestMethod]
        public void AppConfig_HealthChecks_Parameters_ReturnCorrectly()
        {
            var stopStartWindowsServices = healthChecksSection.HealthChecks["stop-start-windows-services"];
            //Accert
            Assert.AreEqual("Email", stopStartWindowsServices.AlertType);
            Assert.AreEqual("example@gmail.com", stopStartWindowsServices.To);
            Assert.AreEqual("example@gmail.com", stopStartWindowsServices.From);
            Assert.AreEqual("Interval", stopStartWindowsServices.Schedule.Frequency);
            Assert.AreEqual("May 1, 2020", stopStartWindowsServices.Schedule.StartTime);
            Assert.AreEqual(1, stopStartWindowsServices.Schedule.TimeParameters.Count());
            Assert.AreEqual("00:05", stopStartWindowsServices.Schedule.TimeParameters.First());
            Assert.AreEqual(1, stopStartWindowsServices.HealthCheckParameters.Count);
            Assert.AreEqual("AdobeARMservice", stopStartWindowsServices.HealthCheckParameters["AdobeARMservice"].Name);
        }
    }
}
