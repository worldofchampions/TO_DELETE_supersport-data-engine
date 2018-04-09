using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services
{
    [Category("RugbyIngestWorkerService")]
    public class RugbyIngestWorkerServiceTests
    {
        private RugbyIngestWorkerService _rugbyIngestWorkerService;
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private Mock<IStatsProzoneRugbyIngestService> _mockStatsProzoneIngestService;
        private Mock<IMongoDbRugbyRepository> _mockMongoDbRepositoryService;
        private RugbyService _rugbyService;

        [SetUp]
        public void Setup()
        {
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _mockStatsProzoneIngestService = new Mock<IStatsProzoneRugbyIngestService>();
            _mockMongoDbRepositoryService = new Mock<IMongoDbRugbyRepository>();

            _rugbyService = 
                new RugbyService(
                    _publicSportDataUnitOfWork,
                    _systemSportDataUnitOfWork);

            _rugbyIngestWorkerService =
                new RugbyIngestWorkerService(
                    _publicSportDataUnitOfWork,
                    _systemSportDataUnitOfWork,
                    (new Mock<ILoggingService>()).Object,
                    _mockStatsProzoneIngestService.Object,
                    _mockMongoDbRepositoryService.Object,
                    _rugbyService);
        }

        [Test]
        public async Task ThrowsNoExceptions()
        {
            _mockStatsProzoneIngestService
                .Setup(m =>
                    m.IngestRugbyReferenceData(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(
                    new RugbyEntitiesResponse()
                    {
                        Entities = new RugbyEntities()
                        {
                            
                        }
                    }));

            try
            {
                await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task IngestTournaments()
        {
            _mockStatsProzoneIngestService
                .Setup(m =>
                    m.IngestRugbyReferenceData(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(
                    new RugbyEntitiesResponse()
                    {
                        Entities = new RugbyEntities()
                        {
                            competitions = new List<Competition>()
                            {
                                new Competition { name = "test competition 1", id = 0 },
                                new Competition { name = "test competition 2", id = 1 }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTournaments.CountAsync());
            Assert.AreEqual("test competition 1", _publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(t => t.ProviderTournamentId == 0).Name);
            Assert.AreEqual("test competition 2", _publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(t => t.ProviderTournamentId == 1).Name);
        }
    }
}