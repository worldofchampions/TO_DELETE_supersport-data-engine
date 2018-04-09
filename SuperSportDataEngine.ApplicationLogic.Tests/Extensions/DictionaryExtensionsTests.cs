using System.Collections.Generic;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Extensions;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Extensions
{
    [Category("Extensions")]
    public class DictionaryExtensionsTests
    {
        [Test]
        public void GetValue()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            map.Add("test1", 10);

            Assert.AreEqual(10, map.GetValueOrDefault("test1"));
        }

        [Test]
        public void GetDefaultValue()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            Assert.AreEqual(0, map.GetValueOrDefault("test1"));
        }
    }
}