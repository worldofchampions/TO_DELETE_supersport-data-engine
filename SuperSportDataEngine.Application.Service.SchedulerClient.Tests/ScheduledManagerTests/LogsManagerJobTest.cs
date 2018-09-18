using Hangfire;
using Hangfire.Common;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    [Category("LogsManagerJob")]
    public class LogsManagerJobTest
    {
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private Mock<IRecurringJobManager> _mockRecurringJobManager;
        private LogsManagerJob _logsManagerJob;
        private Mock<ILoggingService> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();

            _mockRecurringJobManager = new Mock<IRecurringJobManager>();
            _mockRecurringJobManager.Setup(c =>
                c.AddOrUpdate(It.IsAny<string>(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()));

            _mockRecurringJobManager.Setup(c =>
                c.RemoveIfExists(It.IsAny<string>()));

            _mockLogger = new Mock<ILoggingService>();

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

            _logsManagerJob =
                new LogsManagerJob(
                        _mockRecurringJobManager.Object,
                        _rugbyService,
                        _rugbyIngestWorkerService,
                        _systemSportDataUnitOfWork,
                        _publicSportDataUnitOfWork);
        }

        [Test]
        public async Task DoWorkAsync_ThrowsNoExceptions()
        {
            try
            {
                await _logsManagerJob.DoWorkAsync();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task WhenNoCurrentTournaments_NoJobsAreCreated()
        {
            await _logsManagerJob.CreateChildJobsForFetchingLogs();

            _mockRecurringJobManager.Verify(m => m.AddOrUpdate(
                            It.IsAny<string>(),
                            It.IsAny<Job>(),
                            It.IsAny<string>(),
                            It.IsAny<RecurringJobOptions>()),
                            Times.Never());
        }

        [Test]
        public async Task WhenOneCurrentTournament_OnlyOneJobIsCreated()
        {
            var tournamentId = Guid.NewGuid();
            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
                Name = "TestTournament",
                IsEnabled = true,
                Slug = "test",
            };

            var rugbyFixture = new RugbyFixture()
            {
                RugbyTournament = rugbyTournament,
                Id = Guid.NewGuid(),
                LegacyFixtureId = 0,
                ProviderFixtureId = 0,
                RugbyFixtureStatus = RugbyFixtureStatus.Result,
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1)
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);
            _publicSportDataUnitOfWork.RugbyFixtures.Add(rugbyFixture);

            await _logsManagerJob.CreateChildJobsForFetchingLogs();

            _mockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        It.IsAny<string>(),
                        It.IsAny<Job>(),
                        It.IsAny<string>(),
                        It.IsAny<RecurringJobOptions>()),
                        Times.Once());
        }

        [Test]
        public async Task WhenNoFixturesTodayForTournament_DeleteLogsJob()
        {
            var tournamentId = Guid.NewGuid();
            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
                Name = "TestTournament",
                IsEnabled = true,
                Slug = "test",
            };

            var season = new RugbySeason
            {
                Id = Guid.NewGuid(),
                RugbyTournament = rugbyTournament,
                ProviderSeasonId = 0,
                IsCurrent = true
            };

            var schedulerSeason = new SchedulerTrackingRugbySeason
            {
                SeasonId = season.Id,
                RugbySeasonStatus = RugbySeasonStatus.InProgress,
                SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning,
                TournamentId = tournamentId
            };
            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(schedulerSeason);
            _publicSportDataUnitOfWork.RugbySeasons.Add(season);

            var rugbyFixture = new RugbyFixture()
            {
                RugbyTournament = rugbyTournament,
                Id = Guid.NewGuid(),
                LegacyFixtureId = 0,
                ProviderFixtureId = 0,
                RugbyFixtureStatus = RugbyFixtureStatus.Result,
                StartDateTime = DateTimeOffset.UtcNow + TimeSpan.FromDays(2),
                RugbySeason = season
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);
            _publicSportDataUnitOfWork.RugbyFixtures.Add(rugbyFixture);

            await _logsManagerJob.DoWorkAsync();

            _mockRecurringJobManager.Verify(m => m.RemoveIfExists(
                    It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public async Task WhenTwoCurrentTournaments_TwoJobsAreCreated()
        {
            var tournamentId = Guid.NewGuid();
            var tournament2Id = Guid.NewGuid();
            var tournaments = new List<RugbyTournament>()
            {
                new RugbyTournament()
                {
                    Id = tournamentId,
                    Name = "TestTournament",
                    IsEnabled = true,
                    Slug = "test",
                },
                new RugbyTournament()
                {
                    Id = tournament2Id,
                    Name = "TestTournament2",
                    IsEnabled = true,
                    Slug = "test2",
                }
            };

            var fixtures = new List<RugbyFixture>()
            {
                new RugbyFixture()
                {
                    RugbyTournament = tournaments[0],
                    Id = Guid.NewGuid(),
                    LegacyFixtureId = 0,
                    ProviderFixtureId = 0,
                    RugbyFixtureStatus = RugbyFixtureStatus.Result,
                    StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromHours(1)
                },
                new RugbyFixture()
                {
                    RugbyTournament = tournaments[1],
                    Id = Guid.NewGuid(),
                    LegacyFixtureId = 0,
                    ProviderFixtureId = 0,
                    StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromHours(1),
                    RugbyFixtureStatus = RugbyFixtureStatus.Result,
                }
            };

            tournaments.ForEach(t => _publicSportDataUnitOfWork.RugbyTournaments.Add(t));
            fixtures.ForEach(f => _publicSportDataUnitOfWork.RugbyFixtures.Add(f));

            await _logsManagerJob.CreateChildJobsForFetchingLogs();

            _mockRecurringJobManager.Verify(m => m.AddOrUpdate(
                    It.IsAny<string>(),
                    It.IsAny<Job>(),
                    It.IsAny<string>(),
                    It.IsAny<RecurringJobOptions>()),
                Times.Exactly(2));
        }

        [Test]
        public async Task SchedulerStateUpdatedWhenJobIsCreated()
        {
            var seasonId = Guid.NewGuid();

            var tournamentId = Guid.NewGuid();
            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
                Name = "TestTournament",
                IsEnabled = true,
                Slug = "test",
            };

            var season = new RugbySeason
            {
                Id = seasonId,
                RugbyTournament = rugbyTournament,
                ProviderSeasonId = 0,
                IsCurrent = true
            };

            _publicSportDataUnitOfWork.RugbySeasons.Add(season);

            var rugbyFixture = new RugbyFixture()
            {
                RugbyTournament = rugbyTournament,
                Id = Guid.NewGuid(),
                LegacyFixtureId = 0,
                ProviderFixtureId = 0,
                RugbyFixtureStatus = RugbyFixtureStatus.Result,
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1)
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);
            _publicSportDataUnitOfWork.RugbyFixtures.Add(rugbyFixture);

            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(
                new SchedulerTrackingRugbySeason()
                {
                    TournamentId = tournamentId,
                    SeasonId = seasonId,
                    RugbySeasonStatus = RugbySeasonStatus.InProgress,
                    SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning
                });

            await _logsManagerJob.CreateChildJobsForFetchingLogs();

            var seasonEntry = _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.All().FirstOrDefault();

            Assert.IsNotNull(seasonEntry);
            Assert.AreEqual(SchedulerStateForManagerJobPolling.Running, seasonEntry.SchedulerStateForManagerJobPolling);
        }
    }
}
