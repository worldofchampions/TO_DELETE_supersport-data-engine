using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
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
        LiveManagerJob LiveManagerJob;
        Mock<IRugbyService> MockRugbyService;
        Mock<IRugbyIngestWorkerService> MockRugbyIngestWorkerService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>> MockSchedulerTrackingFixtureRepository;
        Mock<TestEntityFrameworkRepository<RugbyFixture>> MockRugbyFixtures;
        Mock<IRecurringJobManager> MockRecurringJobManager;
        Mock<ILoggingService> MockLogger;

        [SetUp]
        public void SetUp()
        {
            MockSchedulerTrackingFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(new List<SchedulerTrackingRugbyFixture>());

            MockRugbyFixtures =
                new Mock<TestEntityFrameworkRepository<RugbyFixture>>(new List<RugbyFixture>());

            MockRugbyService = new Mock<IRugbyService>();
            MockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            MockRecurringJobManager = new Mock<IRecurringJobManager>();

            MockLogger = new Mock<ILoggingService>();

            LiveManagerJob =
                new LiveManagerJob(
                    MockLogger.Object,
                    MockRecurringJobManager.Object,
                    MockRugbyService.Object,
                    MockRugbyIngestWorkerService.Object,
                    MockSchedulerTrackingFixtureRepository.Object);
        }

        [Test]
        public async Task LiveManagerJob_EmptyDataSet_AssertEmpty()
        {
            long count = await MockSchedulerTrackingFixtureRepository.Object.CountAsync();
            Assert.AreEqual(0, count);
        }

        [Test]
        public async Task LiveManagerJob_NoEndedFixtures_NoJobsRemoved()
        {
            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task LiveManagerJob_HasEndedFixture_OnlyOneJobRemoved()
        {
            MockRugbyService
                .Setup(r => r.GetCompletedFixtures()).Returns(
                    Task.FromResult(new List<RugbyFixture>() {
                        new RugbyFixture
                        {
                            TeamA = new RugbyTeam() { Name = "TeamA" },
                            TeamB = new RugbyTeam() { Name = "TeamB" },
                            RugbyFixtureStatus = RugbyFixtureStatus.Result                        }
                    }.AsEnumerable()));

            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task LiveManagerJob_NoCurrentTournaments_DoNotQueryForLiveFixtures()
        {
            MockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(Task.FromResult(new List<RugbyTournament>().AsEnumerable()));

            await LiveManagerJob.DoWorkAsync();

            MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never());
        }

        [Test]
        public async Task LiveManagerJob_OneCurrentTournament_OneCallForLiveFixtures()
        {
            MockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>(){
                        new RugbyTournament(){Id = Guid.NewGuid()}
                    }.AsEnumerable()));

            MockRugbyService.Setup(r => r.GetCompletedFixtures()).Returns(
                    Task.FromResult(new List<RugbyFixture>().AsEnumerable()));

            await LiveManagerJob.DoWorkAsync();

            MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Once());
        }

        [Test]
        public async Task LiveManagerJob_TwoCurrentTournaments_TwoCallsForLiveFixtures()
        {
            MockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    Task.FromResult(new List<RugbyTournament>()
                    {
                        new RugbyTournament(){Id = Guid.NewGuid()},
                        new RugbyTournament(){Id = Guid.NewGuid()}
                    }.AsEnumerable()));

            MockRugbyService
                .Setup(r => r.GetCompletedFixtures()).Returns(
                    Task.FromResult(new List<RugbyFixture>().AsEnumerable()));

            await LiveManagerJob.DoWorkAsync();

            MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Test]
        public async Task LiveManagerJob_InProgressFixtureGetsScheduled()
        {
            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture
                {
                    TournamentId = tournamentId,
                    FixtureId = fixtureId,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted,
                    IsJobRunning = false
                });

            var tournament = new RugbyTournament() { Id = tournamentId };

            MockRugbyService
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
                            RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf
                        }
                    }.AsEnumerable());

            MockRugbyService.Setup(r => r.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentId)).Returns(rugbyFixtures);

            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "LiveManagerJob→LiveMatch→TeamA vs TeamB",
                        It.IsAny<Job>(),
                        "0 */2 * * *",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Once());

            MockRecurringJobManager.Verify(m => m.Trigger(
                        "LiveManagerJob→LiveMatch→TeamA vs TeamB"),
                        Times.Once());

            var f = MockSchedulerTrackingFixtureRepository.Object.All().FirstOrDefault();

            Assert.AreEqual(true, f.IsJobRunning);
        }

        [Test]
        public async Task LiveManagerJob_EndedFixtureGetsRemoved()
        {
            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture { TournamentId = tournamentId, FixtureId = fixtureId, SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling });

            var tournament = new RugbyTournament() { Id = tournamentId };

            MockRugbyService
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
                            RugbyFixtureStatus = RugbyFixtureStatus.Result
                        }
                    }.AsEnumerable());

            MockRugbyService.Setup(r => r.GetCompletedFixtures()).Returns(rugbyFixtures);

            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "LiveManagerJob→LiveMatch→TeamA vs TeamB",
                        It.IsAny<Job>(),
                        "0 */2 * * *",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Never());

            var f = MockSchedulerTrackingFixtureRepository.Object.All().FirstOrDefault();

            MockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.SchedulingCompleted, f.SchedulerStateFixtures);
        }

        [Test]
        public async Task LiveManagerJob_FixtureGetsPolledPreGame()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture
                {
                    TournamentId = tournamentId,
                    FixtureId = fixtureId,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling,
                    IsJobRunning = false
                });

            var tournament = new RugbyTournament() { Id = tournamentId };

            MockRugbyService
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
                            StartDateTime = now + TimeSpan.FromMinutes(14)
                        }
                    }.AsEnumerable());

            MockRugbyFixtures.Object.AddRange(await rugbyFixtures);
            MockRugbyService.Setup(r => r.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentId)).Returns(rugbyFixtures);

            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "LiveManagerJob→LiveMatch→TeamA vs TeamB",
                        It.IsAny<Job>(),
                        "0 */2 * * *",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Once());

            MockRecurringJobManager.Verify(m => m.Trigger(
                        "LiveManagerJob→LiveMatch→TeamA vs TeamB"),
                        Times.Once());

            var f = MockSchedulerTrackingFixtureRepository.Object.All().FirstOrDefault();

            Assert.AreEqual(true, f.IsJobRunning);
        }
    }
}