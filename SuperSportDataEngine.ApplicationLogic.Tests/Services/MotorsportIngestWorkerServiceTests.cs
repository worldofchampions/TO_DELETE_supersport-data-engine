using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Interfaces;
using SuperSportDataEngine.Gateway.Http.Stats.Services;
using System;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services
{
    [TestClass]
    [Category("MotorsportIngestWorkerService")]
    public class MotorsportIngestWorkerServiceTests
    {
        private string _statsApiSharedSecret;
        private string _statsApiKey;
        private string _statsApiBaseUrl;
        private IStatsMotorsportWebRequest _statsMotorsportWebRequest;

        [SetUp]
        public void Setup()
        {
            _statsApiBaseUrl = "http://api.stats.com";
            _statsApiSharedSecret = "JDgQnhPVZQ";
            _statsApiKey = "ta3dprpc4sn79ecm2wg7tqbg";

            _statsMotorsportWebRequest = new StatsMotorsportMotorsportWebRequest(_statsApiBaseUrl, _statsApiKey, _statsApiSharedSecret);
        }

        [Test]
        public void CallingStatsApiWithProductionCredetialsReturnsResponseCode200()
        {
            var request = _statsMotorsportWebRequest.GetRequestForLeagues();

            try
            {
                using (request.GetResponse()) { }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
