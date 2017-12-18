using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    public class RugbyIngestWorkerServiceTest
    {
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private Mock<ILoggingService> _loggingServiceMock;
        private Mock<IMongoDbRugbyRepository> _mongoDbRepositoryMock;
        private Mock<IStatsProzoneRugbyIngestService> _statsProzoneIngestServiceMock;
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private RugbyService _rugbyService;

        [SetUp]
        public void Setup()
        {
            _loggingServiceMock = new Mock<ILoggingService>();
            _mongoDbRepositoryMock = new Mock<IMongoDbRugbyRepository>();
            _statsProzoneIngestServiceMock = new Mock<IStatsProzoneRugbyIngestService>();
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _rugbyService = new RugbyService(
                _publicSportDataUnitOfWork,
                _systemSportDataUnitOfWork);

            _rugbyIngestWorkerService = new RugbyIngestWorkerService(
                _publicSportDataUnitOfWork,
                _systemSportDataUnitOfWork,
                _loggingServiceMock.Object,
                _statsProzoneIngestServiceMock.Object,
                _mongoDbRepositoryMock.Object,
                _rugbyService);
        }

        [Test]
        public async Task InitiallyNoVenuesIngested()
        {
            Assert.AreEqual(0, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
        }

        [Test]
        public async Task Ingest_1_Venue_From_Provider()
        {
            _statsProzoneIngestServiceMock.Setup(
                m => m.IngestRugbyReferenceData(It.IsAny<CancellationToken>())).Returns(
                    Task.FromResult(
                        new RugbyEntitiesResponse()
                        {
                            ResponseTime = DateTime.Now,
                            RequestTime = DateTime.Now,
                            Entities = new RugbyEntities()
                            {
                                venues = new List<Venue>()
                                {
                                    new Venue(){ id = 0, name = "Test Venue 1" }
                                }
                            }
                        }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(1, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
            Assert.AreEqual(0, (await _publicSportDataUnitOfWork.RugbyVenues.AllAsync()).ToList().First().ProviderVenueId);
            Assert.AreEqual("Test Venue 1", (await _publicSportDataUnitOfWork.RugbyVenues.AllAsync()).ToList().First().Name);
        }

        [Test]
        public async Task Ingest_2_Venue_From_Provider()
        {
            _statsProzoneIngestServiceMock.Setup(
                m => m.IngestRugbyReferenceData(It.IsAny<CancellationToken>())).Returns(
                Task.FromResult(
                    new RugbyEntitiesResponse()
                    {
                        ResponseTime = DateTime.Now,
                        RequestTime = DateTime.Now,
                        Entities = new RugbyEntities()
                        {
                            venues = new List<Venue>()
                            {
                                new Venue(){ id = 0, name = "Test Venue 1" },
                                new Venue(){ id = 1, name = "Test Venue 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
        }
    }
}
