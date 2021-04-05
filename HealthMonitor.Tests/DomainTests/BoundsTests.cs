using System;
using HealthMonitor.Domain;
using HealthMonitor.Domain.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthMonitor.Tests.DomainTests
{
    [TestClass]
    public class BoundsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Threshold_Constructor_MinIsBiggerThanMax_ThrowsOutOfRangeException()
        {
            var willBreak = new Threshold<int>(1, 0);
        }

        [TestMethod]
        public void Threshold_CheckWithinBounds_WithBounds()
        {
            //set up
            var threshold = new Threshold<int>(1, 100);
            //act
            var result = threshold.CheckWithinBounds(50);
            //Accert
            Assert.AreEqual(ThresholdResultType.WithinRange, result);
        }
        [TestMethod]
        public void Threshold_CheckWithinBounds_NumberBiggerMax_OverMaximum()
        {
            //set up
            var threshold = new Threshold<int>(1, 100);
            //act
            var result = threshold.CheckWithinBounds(150);
            //Accert
            Assert.AreEqual(ThresholdResultType.OverMaximum, result);
        }
        [TestMethod]
        public void Threshold_CheckWithinBounds_NumberLessMin_UnderMinimum()
        {
            //set up
            var threshold = new Threshold<int>(1, 100);
            //act
            var result = threshold.CheckWithinBounds(0);
            //Accert
            Assert.AreEqual(ThresholdResultType.UnderMinimum, result);
        }
        [TestMethod]
        public void Threshold_ExceedsBounds_NumberLessMin_True()
        {
            //set up
            var threshold = new Threshold<int>(1, 100);
            //act
            var result = threshold.ExceedsBounds(0);
            //Accert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void Threshold_ExceedsBounds_NumberBiggerMax_True()
        {
            //set up
            var threshold = new Threshold<int>(1, 100);
            //act
            var result = threshold.ExceedsBounds(150);
            //Accert
            Assert.IsTrue(result);
        }
    }
}
