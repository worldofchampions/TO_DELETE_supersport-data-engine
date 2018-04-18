using System;
using System.Configuration;
using System.Threading.Tasks;
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

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{ 
    [Category("FixturesManagerJob")]
    public class FixturesManagerJobTests
    {
        FixturesManagerJob _fixturesManagerJob;
        IRugbyService _rugbyService;
        Mock<IRugbyIngestWorkerService> _mockRugbyIngestWorkerService;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        Mock<IRecurringJobManager> _mockRecurringJobManager;
        private Mock<ILoggingService> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            _mockRecurringJobManager = new Mock<IRecurringJobManager>();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _mockLogger = new Mock<ILoggingService>();

            _rugbyService = 
                new RugbyService(
                    _publicSportDataUnitOfWork,
                    _systemSportDataUnitOfWork,
                    _mockLogger.Object);

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _mockRecurringJobManager.Object,
                    _systemSportDataUnitOfWork,
                    _rugbyService,
                    _mockRugbyIngestWorkerService.Object
                    );
        }

        [Test]
        public async Task DoWorkAsync_ThrowsNoExceptions()
        {
            try
            {
                await _fixturesManagerJob.DoWorkAsync();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task ChildJobCreated_PollsActiveTournaments()
        {
            var tournamentId = Guid.NewGuid();
            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
                Name = "Test Tournament",
                LegacyTournamentId = 0,
                ProviderTournamentId = 0,
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);

            var rugbySeason = 
                new RugbySeason()
                {
                    Id = Guid.NewGuid(),
                    RugbyTournament = rugbyTournament,
                    IsCurrent = true,
                    ProviderSeasonId = 2000,
                    Name = "Test Season"
                };

            _publicSportDataUnitOfWork.RugbySeasons.Add(rugbySeason);

            try
            {
                await _fixturesManagerJob.DoWorkAsync();
                _mockRecurringJobManager
                    .Verify(
                        m => m.AddOrUpdate(
                            ConfigurationManager.AppSettings[
                                "ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + rugbyTournament.Name,
                            It.IsAny<Job>(),
                            ConfigurationManager.AppSettings[
                                "ScheduleManagerJob_Fixtures_ActiveTournaments_JobCronExpression"],
                            It.IsAny<RecurringJobOptions>()),
                        Times.Once());
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task WhenChildJobCreatedForPollingActiveTournaments_TrackingTableAlsoUpdated()
        {
            var tournamentId = Guid.NewGuid();
            var rugbyTournament = 
                new RugbyTournament()
                {
                    Id = tournamentId,
                    Name = "Test Tournament",
                    LegacyTournamentId = 0,
                    ProviderTournamentId = 0,
                    IsEnabled = true
                };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);

            var rugbySeason = 
                new RugbySeason()
                {
                    Id = Guid.NewGuid(),
                    RugbyTournament = rugbyTournament,
                    IsCurrent = true,
                    ProviderSeasonId = 2000,
                    Name = "Test Season"
                };  

            _publicSportDataUnitOfWork.RugbySeasons.Add(rugbySeason);

            var trackingEntry = 
                new SchedulerTrackingRugbyTournament()
                {
                    TournamentId = rugbyTournament.Id,
                    SeasonId = rugbySeason.Id,
                    SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning
                };

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Add(trackingEntry);

            try
            {
                await _fixturesManagerJob.DoWorkAsync();
                
                Assert.AreEqual(SchedulerStateForManagerJobPolling.Running, trackingEntry.SchedulerStateForManagerJobPolling);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task ChildJobDeleted_TournamentsInactive()
        {
            var tournamentId = Guid.NewGuid();
            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
                Name = "Test Tournament",
                LegacyTournamentId = 0,
                ProviderTournamentId = 0,
                IsEnabled = false
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);

            var rugbySeason =
                new RugbySeason()
                {
                    Id = Guid.NewGuid(),
                    RugbyTournament = rugbyTournament,
                    IsCurrent = true,
                    ProviderSeasonId = 2000,
                    Name = "Test Season"
                };

            _publicSportDataUnitOfWork.RugbySeasons.Add(rugbySeason);

            try
            {
                await _fixturesManagerJob.DoWorkAsync();

                _mockRecurringJobManager
                    .Verify(
                        m => m.RemoveIfExists(
                            ConfigurationManager.AppSettings[
                                "ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + rugbyTournament.Name),
                        Times.Once());
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task TrackingTableUpdated_TournamentInactive()
        {
            var tournamentId = Guid.NewGuid();
            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
                Name = "Test Tournament",
                LegacyTournamentId = 0,
                ProviderTournamentId = 0,
                IsEnabled = false
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(rugbyTournament);

            var rugbySeason =
                new RugbySeason()
                {
                    Id = Guid.NewGuid(),
                    RugbyTournament = rugbyTournament,
                    IsCurrent = true,
                    ProviderSeasonId = 2000,
                    Name = "Test Season"
                };

            _publicSportDataUnitOfWork.RugbySeasons.Add(rugbySeason);

            var schedulerEntry = new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId,
                SeasonId = rugbySeason.Id,
                SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running
            };

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Add(
                schedulerEntry);

            try
            {
                await _fixturesManagerJob.DoWorkAsync();

                Assert.AreEqual(SchedulerStateForManagerJobPolling.NotRunning, schedulerEntry.SchedulerStateForManagerJobPolling);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
