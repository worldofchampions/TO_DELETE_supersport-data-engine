using System;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Helpers;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Helpers
{
    [TestFixture]
    public class DateTimerTests
    {
        [Test]
        public void StartOfDayTest()
        {
            var startOfDay = new DateTime(2000, 01, 01, 12, 0, 0).StartOfDay();
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0), startOfDay);
        }

        [Test]
        public void EndOfDayTest()
        {
            var endOfDay = new DateTime(2000, 01, 01, 12, 0, 0).EndOfDay();
            Assert.AreEqual(new DateTime(2000, 1, 2, 0, 0, 0).Subtract(TimeSpan.FromTicks(1)), endOfDay);
        }
    }
}