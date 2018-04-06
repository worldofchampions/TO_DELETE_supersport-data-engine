using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    public class FixturesManagerJobTests
    {
        FixturesManagerJob _fixturesManagerJob;
        IRugbyService _rugbyService;
        Mock<IRugbyIngestWorkerService> _mockRugbyIngestWorkerService;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        Mock<IRecurringJobManager> _mockRecurringJobManager;

        [SetUp]
        public void SetUp()
        {
            _mockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            _mockRecurringJobManager = new Mock<IRecurringJobManager>();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();

            _rugbyService = 
                new RugbyService(
                    _publicSportDataUnitOfWork,
                    _systemSportDataUnitOfWork);

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _mockRecurringJobManager.Object,
                    _systemSportDataUnitOfWork,
                    _rugbyService,
                    _mockRugbyIngestWorkerService.Object
                    );
        }

        [Test]
        public async Task FixturesManagerJob_ThrowsNoExceptions()
        {
            try
            {
                await _fixturesManagerJob.DoWorkAsync();
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task FixturesManagerJob_TournamentActive_ChildJobGetsCreated()
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
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task FixturesManagerJob_TournamentActive_TrackingTableUpdated()
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
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
    }
}
