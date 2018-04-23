using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services
{
    public class LegacyAuthServiceTests
    {
        private ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private LegacyAuthService _legacyAuthService;
        private Mock<ILoggingService> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _mockLogger = new Mock<ILoggingService>();

            _legacyAuthService =
                new LegacyAuthService(
                    _mockLogger.Object,
                    _systemSportDataUnitOfWork);
        }

        [Test]
        public async Task WhenIncorrectKey_Unauthorised()
        {
            Assert.AreEqual(false, await _legacyAuthService.IsAuthorised("XXX"));
        }

        [Test]
        public async Task WhenCorrectKey_Authorised()
        {
            _systemSportDataUnitOfWork.LegacyAuthFeedConsumers.Add(
                new LegacyAuthFeedConsumer()
                {
                    AuthKey = "XXX",
                    Active = true,
                });

            Assert.AreEqual(true, await _legacyAuthService.IsAuthorised("XXX"));
        }

        [Test]
        public async Task WhenCorrectKey_IncorrectSiteId_ForNowIsStill_Authorised()
        {
            _systemSportDataUnitOfWork.LegacyAuthFeedConsumers.Add(
                new LegacyAuthFeedConsumer()
                {
                    AuthKey = "XXX",
                    Active = true,
                });

            Assert.AreEqual(true, await _legacyAuthService.IsAuthorised("XXX", 1));
        }
    }
}