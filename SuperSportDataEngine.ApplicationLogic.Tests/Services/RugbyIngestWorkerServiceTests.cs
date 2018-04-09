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
using Team = SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team;

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
        public async Task IngestTournamentsWithNoDuplicates()
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
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTournaments.CountAsync());
            Assert.AreEqual("test competition 1", _publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(t => t.ProviderTournamentId == 0).Name);
            Assert.AreEqual("test competition 2", _publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(t => t.ProviderTournamentId == 1).Name);
        }

        [Test]
        public async Task IngestPlayersWithNoDuplicates()
        {
            _mockStatsProzoneIngestService
                .Setup(m =>
                    m.IngestRugbyReferenceData(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(
                    new RugbyEntitiesResponse()
                    {
                        Entities = new RugbyEntities()
                        {
                            players = new List<Player>()
                            {
                                new Player{ id = 0, name = "Player 1" },
                                new Player{ id = 1, name = "Player 2" },
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyPlayers.CountAsync());
            Assert.AreEqual("Player 1", _publicSportDataUnitOfWork.RugbyPlayers.FirstOrDefault(t => t.ProviderPlayerId == 0).FullName);
            Assert.AreEqual("Player 2", _publicSportDataUnitOfWork.RugbyPlayers.FirstOrDefault(t => t.ProviderPlayerId == 1).FullName);
        }

        [Test]
        public async Task IngestVenuesWithNoDuplicates()
        {
            _mockStatsProzoneIngestService
                .Setup(m =>
                    m.IngestRugbyReferenceData(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(
                    new RugbyEntitiesResponse()
                    {
                        Entities = new RugbyEntities()
                        {
                            venues = new List<Venue>()
                            {
                                new Venue{ id = 0, name = "Venue 1"},
                                new Venue{ id = 1, name = "Venue 2"},
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
            Assert.AreEqual("Venue 1", _publicSportDataUnitOfWork.RugbyVenues.FirstOrDefault(t => t.ProviderVenueId == 0).Name);
            Assert.AreEqual("Venue 2", _publicSportDataUnitOfWork.RugbyVenues.FirstOrDefault(t => t.ProviderVenueId == 1).Name);
        }

        [Test]
        public async Task IngestTeamsWithNoDuplicates()
        {
            _mockStatsProzoneIngestService
                .Setup(m =>
                    m.IngestRugbyReferenceData(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(
                    new RugbyEntitiesResponse()
                    {
                        Entities = new RugbyEntities()
                        {
                            teams = new List<Team>()
                            {
                                new Team{ id = 0, name = "Team 1"},
                                new Team{ id = 1, name = "Team 2"}
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTeams.CountAsync());
            Assert.AreEqual("Team 1", _publicSportDataUnitOfWork.RugbyTeams.FirstOrDefault(t => t.ProviderTeamId == 0).Name);
            Assert.AreEqual("Team 2", _publicSportDataUnitOfWork.RugbyTeams.FirstOrDefault(t => t.ProviderTeamId == 1).Name);
        }
    }
}