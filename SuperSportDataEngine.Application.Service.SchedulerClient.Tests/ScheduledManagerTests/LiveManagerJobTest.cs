using Hangfire;
using Hangfire.Common;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Tests.Common.Repositories.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    [Category("LiveManagerJob")]
    public class LiveManagerJobTest
    {
        LiveManagerJob _liveManagerJob;
        Mock<IRugbyService> _mockRugbyService;
        Mock<IRugbyIngestWorkerService> _mockRugbyIngestWorkerService;
        private TestSystemSportDataUnitOfWork _mockUnitOfWork;
        Mock<TestEntityFrameworkRepository<RugbyFixture>> _mockRugbyFixtures;
        Mock<IRecurringJobManager> _mockRecurringJobManager;

        [SetUp]
        public void SetUp()
        {
            _mockRugbyFixtures =
                new Mock<TestEntityFrameworkRepository<RugbyFixture>>(new List<RugbyFixture>());

            _mockRugbyService = new Mock<IRugbyService>();
            _mockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            _mockRecurringJobManager = new Mock<IRecurringJobManager>();
            _mockUnitOfWork = new TestSystemSportDataUnitOfWork();

            _liveManagerJob =
                new LiveManagerJob(
                    _mockRecurringJobManager.Object,
                    _mockRugbyService.Object,
                    _mockRugbyIngestWorkerService.Object,
                    _mockUnitOfWork);
        }

        [Test]
        public async Task NoJobsRemoved_CriteriaNotMet()
        {
            await _liveManagerJob.DoWorkAsync();

            _mockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task WhenFixtureEnds_JobIsRemoved()
        {
            _mockRugbyService
                .Setup(r => r.GetCompletedFixtures()).Returns(
                    Task.FromResult(new List<RugbyFixture>() {
                        new RugbyFixture
                        {
                            TeamA = new RugbyTeam() { Name = "TeamA" },
                            TeamB = new RugbyTeam() { Name = "TeamB" },
                            RugbyFixtureStatus = RugbyFixtureStatus.Result                        
                        }
                    }.AsEnumerable()));

            await _liveManagerJob.DoWorkAsync();

            _mockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task WhenNoCurrentTournaments_ServiceIsNotCalledForFixtures()
        {
            _mockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(Task.FromResult(new List<RugbyTournament>().AsEnumerable()));

            await _liveManagerJob.DoWorkAsync();

            _mockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never());
        }

        [Test]
        public async Task WhenOneCurrentTournament_OnlyOneCallToServiceForFixtures()
        {
            _mockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>(){
                        new RugbyTournament(){Id = Guid.NewGuid()}
                    }.AsEnumerable()));

            _mockRugbyService.Setup(r => r.GetCompletedFixtures()).Returns(
                    Task.FromResult(new List<RugbyFixture>().AsEnumerable()));

            await _liveManagerJob.DoWorkAsync();

            _mockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Once());
        }

        [Test]
        public async Task WhenTwoCurrentTournaments_TwoCallsMadeToService()
        {
            _mockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>()
                    {
                        new RugbyTournament(){Id = Guid.NewGuid()},
                        new RugbyTournament(){Id = Guid.NewGuid()}
                    }.AsEnumerable()));

            _mockRugbyService
                .Setup(r => r.GetCompletedFixtures()).Returns(
                    Task.FromResult(new List<RugbyFixture>().AsEnumerable()));

            await _liveManagerJob.DoWorkAsync();

            _mockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Test]
        public async Task WhenFixtureInProgress_CreateChildJob()
        {
            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            _mockUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
                new SchedulerTrackingRugbyFixture
                {
                    TournamentId = tournamentId,
                    FixtureId = fixtureId,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted,
                    IsJobRunning = false
                });

            var tournament = new RugbyTournament() { Id = tournamentId };

            _mockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>() { tournament }.AsEnumerable()));

            var rugbyFixtures =
                Task.FromResult(new List<RugbyFixture>()
                    {
                        new RugbyFixture
                        {
                            Id = fixtureId,
                            RugbyTournament = tournament,
                            TeamA = new RugbyTeam { Name = "TeamA" },
                            TeamB = new RugbyTeam { Name = "TeamB" },
                            LegacyFixtureId = 123,
                            RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf
                        }
                    }.AsEnumerable());

            _mockRugbyService.Setup(r => r.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentId)).Returns(rugbyFixtures);

            await _liveManagerJob.DoWorkAsync();

            _mockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "Rugby→StatsProzone→LiveManagerJob→LiveMatch→TeamA vs TeamB→123",
                        It.IsAny<Job>(),
                        "0 0 29 2/12000 WED",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Once());

            _mockRecurringJobManager.Verify(m => m.Trigger(
                        "Rugby→StatsProzone→LiveManagerJob→LiveMatch→TeamA vs TeamB→123"),
                        Times.Once());             

            var f = _mockUnitOfWork.SchedulerTrackingRugbyFixtures.All().FirstOrDefault();

            Assert.AreEqual(true, f != null && f.IsJobRunning);
        }

        [Test]
        public async Task WhenFixtureEnds_RemoveChildJob()
        {
            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            _mockUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
                new SchedulerTrackingRugbyFixture { TournamentId = tournamentId, FixtureId = fixtureId, SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling });

            var tournament = new RugbyTournament() { Id = tournamentId };

            _mockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>() { tournament }.AsEnumerable()));

            var rugbyFixtures =
                Task.FromResult(new List<RugbyFixture>()
                    {
                        new RugbyFixture
                        {
                            Id = fixtureId,
                            RugbyTournament = tournament,
                            TeamA = new RugbyTeam { Name = "TeamA" },
                            TeamB = new RugbyTeam { Name = "TeamB" },
                            LegacyFixtureId = 123,
                            RugbyFixtureStatus = RugbyFixtureStatus.Result
                        }
                    }.AsEnumerable());

            _mockRugbyService.Setup(r => r.GetCompletedFixtures()).Returns(rugbyFixtures);

            await _liveManagerJob.DoWorkAsync();

            _mockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "Rugby→StatsProzone→LiveManagerJob→LiveMatch→TeamA vs TeamB→123",
                        It.IsAny<Job>(),
                        "0 0 29 2/12000 WED",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Never());

            var f = _mockUnitOfWork.SchedulerTrackingRugbyFixtures.All().FirstOrDefault();

            _mockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.SchedulingCompleted, f.SchedulerStateFixtures);
        }

        [Test]
        public async Task WhenFixtureIsPreLivePolling_ChildJobGetsCreated()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            _mockUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
                new SchedulerTrackingRugbyFixture
                {
                    TournamentId = tournamentId,
                    FixtureId = fixtureId,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling,
                    IsJobRunning = false
                });

            var tournament = new RugbyTournament() { Id = tournamentId };

            _mockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>() { tournament }.AsEnumerable()));

            var rugbyFixtures =
                Task.FromResult(new List<RugbyFixture>()
                    {
                        new RugbyFixture
                        {
                            Id = fixtureId,
                            RugbyTournament = tournament,
                            TeamA = new RugbyTeam { Name = "TeamA" },
                            TeamB = new RugbyTeam { Name = "TeamB" },
                            RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                            StartDateTime = now + TimeSpan.FromMinutes(14),
                            LegacyFixtureId = 123
                        }
                    }.AsEnumerable());

            _mockRugbyFixtures.Object.AddRange(await rugbyFixtures);
            _mockRugbyService.Setup(r => r.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentId)).Returns(rugbyFixtures);

            await _liveManagerJob.DoWorkAsync();

            _mockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "Rugby→StatsProzone→LiveManagerJob→LiveMatch→TeamA vs TeamB→123",
                        It.IsAny<Job>(),
                        "0 0 29 2/12000 WED",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Once());

            _mockRecurringJobManager.Verify(m => m.Trigger(
                        "Rugby→StatsProzone→LiveManagerJob→LiveMatch→TeamA vs TeamB→123"),
                        Times.Once());

            var f = _mockUnitOfWork.SchedulerTrackingRugbyFixtures.All().FirstOrDefault();

            Assert.AreEqual(true, f.IsJobRunning);
        }
    }
}
