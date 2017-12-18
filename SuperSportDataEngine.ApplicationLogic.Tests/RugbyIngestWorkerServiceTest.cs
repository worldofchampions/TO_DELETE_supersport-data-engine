using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;
using Season = SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Season;

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    [Category("RugbyIngestWorkerServiceTest")]
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
        public async Task When_Provider_Returns_Null_Throw_Exception()
        {
            _statsProzoneIngestServiceMock.Setup(
                m => m.IngestRugbyReferenceData(It.IsAny<CancellationToken>())).Returns((Task<RugbyEntitiesResponse>) null);

            Assert.ThrowsAsync<NullReferenceException>(async () => await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None));
        }

        [Test]
        public async Task Persist_ToMongoDb_WhenIngestingReferenceData()
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
                            teams = new List<Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team>()
                            {
                                new Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team(){ id = 1, name = "Test Team 1" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            _mongoDbRepositoryMock.Verify(m => m.SaveEntities(It.IsAny<RugbyEntitiesResponse>()), Times.Once);
        }

        [Test]
        public async Task Ingest_1_Team_From_Provider()
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
                                teams = new List<Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team>()
                                {
                                    new Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team(){ id = 1, name = "Test Team 1" }
                                }
                            }
                        }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(1, await _publicSportDataUnitOfWork.RugbyTeams.CountAsync());
            Assert.AreEqual(1, (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).ToList().First().ProviderTeamId);
            Assert.AreEqual("Test Team 1", (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).ToList().First().Name);
        }

        [Test]
        public async Task Ingest_2_Teams_From_Provider()
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
                            teams = new List<Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team>()
                            {
                                new Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team(){ id = 1, name = "Test Team 1" },
                                new Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team(){ id = 2, name = "Test Team 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTeams.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Teams_From_Provider_Twice_NoDuplicates()
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
                            teams = new List<Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team>()
                            {
                                new Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team(){ id = 1, name = "Test Team 1" },
                                new Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team(){ id = 2, name = "Test Team 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTeams.CountAsync());
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
                                    new Venue(){ id = 1, name = "Test Venue 1" }
                                }
                            }
                        }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(1, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
            Assert.AreEqual(1, (await _publicSportDataUnitOfWork.RugbyVenues.AllAsync()).ToList().First().ProviderVenueId);
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
                                new Venue(){ id = 1, name = "Test Venue 1" },
                                new Venue(){ id = 2, name = "Test Venue 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Venue_From_Provider_Twice_NoDuplicates()
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
                                new Venue(){ id = 1, name = "Test Venue 1" },
                                new Venue(){ id = 2, name = "Test Venue 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyVenues.CountAsync());
        }

        [Test]
        public async Task Ingest_1_Tournament_From_Provider()
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
                            competitions = new List<Competition>()
                            {
                                new Competition(){ id = 1, name = "Test Competition 1" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(1, await _publicSportDataUnitOfWork.RugbyTournaments.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Tournaments_From_Provider()
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
                            competitions = new List<Competition>()
                            {
                                new Competition(){ id = 1, name = "Test Competition 1" },
                                new Competition(){ id = 2, name = "Test Competition 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTournaments.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Tournaments_From_Provider_Twice_NoDuplicates()
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
                            competitions = new List<Competition>()
                            {
                                new Competition(){ id = 1, name = "Test Competition 1" },
                                new Competition(){ id = 2, name = "Test Competition 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyTournaments.CountAsync());
        }

        [Test]
        public async Task Ingest_1_Player_From_Provider()
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
                            players = new List<Player>()
                            {
                                new Player(){ id = 1, name = "Test Player 1" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(1, await _publicSportDataUnitOfWork.RugbyPlayers.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Players_From_Provider()
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
                            players = new List<Player>()
                            {
                                new Player(){ id = 1, name = "Test Player 1" },
                                new Player(){ id = 2, name = "Test Player 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyPlayers.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Players_From_Provider_Twice_NoDuplicates()
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
                            players = new List<Player>()
                            {
                                new Player(){ id = 1, name = "Test Player 1" },
                                new Player(){ id = 2, name = "Test Player 2" }
                            }
                        }
                    }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);
            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbyPlayers.CountAsync());
        }

        [Test]
        public async Task Ingest_2_Seasons_ForTournament_From_Provider()
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
                            competitions = new List<Competition>()
                            {
                                new Competition(){ id = 1, name = "Test Competition 1" }
                            }
                        }
                    }));

            _statsProzoneIngestServiceMock.Setup(
                m => m.IngestSeasonData(It.IsAny<CancellationToken>(), 1, DateTime.Today.Year)).Returns(
                Task.FromResult(new RugbySeasonResponse()
                {
                    RequestTime = DateTime.Now,
                    ResponseTime = DateTime.Now,
                    RugbySeasons = new RugbySeasons()
                    {
                        competitionId = 1,
                        season = new List<Season>()
                        {
                            new Season(){ id = DateTime.Now.Year, name = DateTime.Now.Year.ToString(), active = true, currentSeason = true }
                        }
                    }
                }));

            _statsProzoneIngestServiceMock.Setup(
                m => m.IngestSeasonData(It.IsAny<CancellationToken>(), 1, DateTime.Today.Year + 1)).Returns(
                Task.FromResult(new RugbySeasonResponse()
                {
                    RequestTime = DateTime.Now,
                    ResponseTime = DateTime.Now,
                    RugbySeasons = new RugbySeasons()
                    {
                        competitionId = 1,
                        season = new List<Season>()
                        {
                            new Season(){ id = DateTime.Now.Year + 1, name = (DateTime.Now.Year + 1).ToString(), active = true, currentSeason = true }
                        }
                    }
                }));

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            // ReSharper disable once PossibleNullReferenceException
            (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault().IsEnabled = true;

            await _rugbyIngestWorkerService.IngestReferenceData(CancellationToken.None);

            Assert.AreEqual(2, await _publicSportDataUnitOfWork.RugbySeasons.CountAsync());
        }
    }
}
