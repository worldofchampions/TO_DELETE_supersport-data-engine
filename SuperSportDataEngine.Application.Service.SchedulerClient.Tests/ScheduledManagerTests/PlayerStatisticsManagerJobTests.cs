using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    [System.ComponentModel.Category("PlayerStatisticsManagerJob")]
    public class PlayerStatisticsManagerJobTests
    {
        private Mock<IRecurringJobManager> _mockRecurringJobManager;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private Mock<ILoggingService> _mockLogger;
        private PlayerStatisticsManagerJob _playerStatisticsJobManager;

        [SetUp]
        public void Setup()
        {
            _mockRecurringJobManager = new Mock<IRecurringJobManager>();
            _mockLogger = new Mock<ILoggingService>();
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _rugbyService = new RugbyService(
                _publicSportDataUnitOfWork,
                _systemSportDataUnitOfWork,
                _mockLogger.Object);

            _rugbyIngestWorkerService =
                new RugbyIngestWorkerService(
                    _publicSportDataUnitOfWork,
                    _systemSportDataUnitOfWork,
                    _mockLogger.Object,
                    (new Mock<IStatsProzoneRugbyIngestService>()).Object,
                    (new Mock<IMongoDbRugbyRepository>()).Object,
                    _rugbyService);

            _playerStatisticsJobManager = 
                new PlayerStatisticsManagerJob(
                    _mockRecurringJobManager.Object,
                    _rugbyService,
                    _rugbyIngestWorkerService);
        }

        [Test]
        public async Task ThrowsNoExceptions()
        {
            try
            {
                await _playerStatisticsJobManager.DoWorkAsync();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
