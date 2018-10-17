using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Common.Helpers;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Helpers
{
    [Category("TimezoneHelerTest")]
    public class TimezoneHelperTests
    {
        [Test]
        public void FromUtcToSastDateTimeOffset_StateUnderTest_ExpectedBehavior()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            Assert.AreEqual(dateTimeOffset.AddHours(2), dateTimeOffset.FromUtcToSastDateTimeOffset());
        }

        [Test]
        public void FromUtcToSastDateTimeOffset_StateUnderTest_ThrowsNoException_OnUtc()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            Assert.DoesNotThrow(delegate { dateTimeOffset.FromUtcToSastDateTimeOffset(); });
        }

        [Test]
        public void FromUtcToSastDateTimeOffset_StateUnderTest_ThrowsException_OnNonUtc()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
            Assert.Throws<ArgumentException>(delegate { dateTimeOffset.FromUtcToSastDateTimeOffset(); } );
        }

        [Test]
        public void FromUtcToSastDateTime_StateUnderTest_ExpectedBehavior()
        {
            DateTime dateTime = DateTime.UtcNow;
            Assert.AreEqual(dateTime.AddHours(2), dateTime.FromUtcToSastDateTime());
        }
    }
}
