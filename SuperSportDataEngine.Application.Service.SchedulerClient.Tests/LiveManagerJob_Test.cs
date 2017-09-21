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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests
{
    [Category("LiveManagerJob")]
    public class LiveManagerJob_Test
    {
        IQueryable<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtureData;

        LiveManagerJob LiveManagerJob;
        Mock<IRugbyService> MockRugbyService;
        Mock<IRugbyIngestWorkerService> MockRugbyIngestWorkerService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>> MockSchedulerTrackingFixtureRepository = null;
        Mock<IRecurringJobManager> MockRecurringJobManager;

        [SetUp]
        public void SetUp()
        {
            SchedulerTrackingRugbyFixtureData = new List<SchedulerTrackingRugbyFixture> { }.AsQueryable();

            var mockSet = new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();
            mockSet.As<IQueryable<SchedulerTrackingRugbyFixture>>().Setup(m => m.Provider).Returns(SchedulerTrackingRugbyFixtureData.Provider);
            mockSet.As<IQueryable<SchedulerTrackingRugbyFixture>>().Setup(m => m.Expression).Returns(SchedulerTrackingRugbyFixtureData.Expression);
            mockSet.As<IQueryable<SchedulerTrackingRugbyFixture>>().Setup(m => m.ElementType).Returns(SchedulerTrackingRugbyFixtureData.ElementType);
            mockSet.As<IQueryable<SchedulerTrackingRugbyFixture>>().Setup(m => m.GetEnumerator()).Returns(() => SchedulerTrackingRugbyFixtureData.GetEnumerator());          

            MockSchedulerTrackingFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(new List<SchedulerTrackingRugbyFixture>());

            MockRugbyService = new Mock<IRugbyService>();
            MockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            MockRecurringJobManager = new Mock<IRecurringJobManager>();

            LiveManagerJob =
                new LiveManagerJob(
                        MockRugbyService.Object,
                        MockRugbyIngestWorkerService.Object,
                        MockRecurringJobManager.Object,
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
                .Setup(r => r.GetEndedFixtures()).Returns(
                    new List<RugbyFixture>() {
                        new RugbyFixture
                        {
                            HomeTeam = new RugbyTeam() { Name = "HomeTeam1" },
                            AwayTeam = new RugbyTeam() { Name = "AwayTeam1" },
                            RugbyFixtureStatus = RugbyFixtureStatus.Final
                        }
                    });

            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task LiveManagerJob_NoCurrentTournaments_DoNotQueryForLiveFixtures()
        {
            MockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(new List<RugbyTournament>());

            await LiveManagerJob.DoWorkAsync();

            MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<Guid>()), Times.Never());
        }

        [Test]
        public async Task LiveManagerJob_OneCurrentTournament_OneCallForLiveFixtures()
        {
            MockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(
                    new List<RugbyTournament>(){
                        new RugbyTournament(){Id = Guid.NewGuid()}
                    });

            MockRugbyService.Setup(r => r.GetEndedFixtures()).Returns(
                    new List<RugbyFixture>());

            await LiveManagerJob.DoWorkAsync();

            MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<Guid>()), Times.Once());
        }

        [Test]
        public async Task LiveManagerJob_TwoCurrentTournaments_TwoCallsForLiveFixtures()
        {
            MockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    new List<RugbyTournament>()
                    {
                        new RugbyTournament(){Id = Guid.NewGuid()},
                        new RugbyTournament(){Id = Guid.NewGuid()}
                    });

            MockRugbyService
                .Setup(r => r.GetEndedFixtures()).Returns(
                    new List<RugbyFixture>());

            await LiveManagerJob.DoWorkAsync();

            MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Test]
        public async Task LiveManagerJob_LiveFixtureGetsScheduled()
        {
            Guid tournamentId = Guid.NewGuid();
            Guid fixtureId = Guid.NewGuid();

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture { TournamentId = tournamentId, FixtureId = fixtureId, SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted });

            var tournament = new RugbyTournament() { Id = tournamentId };

            MockRugbyService
                .Setup(r => r.GetCurrentTournaments()).Returns(
                    new List<RugbyTournament>() { tournament });

            var rugbyFixtures =
                new List<RugbyFixture>()
                    {
                        new RugbyFixture
                        {
                            Id = fixtureId,
                            RugbyTournament = tournament,
                            HomeTeam = new RugbyTeam { Name = "HomeTeam1" },
                            AwayTeam = new RugbyTeam { Name = "AwayTeam1" },
                            RugbyFixtureStatus = RugbyFixtureStatus.InProgress
                        }
                    };

            MockRugbyService.Setup(r => r.GetLiveFixturesForCurrentTournament(tournamentId)).Returns(rugbyFixtures);

            await LiveManagerJob.DoWorkAsync();

            MockRecurringJobManager.Verify(m => m.AddOrUpdate(
                        "LiveManagerJob→LiveMatch→HomeTeam1 vs AwayTeam1",
                        It.IsAny<Job>(),
                        "0 */2 * * *",
                        It.IsAny<RecurringJobOptions>()),
                        Times.Once());

            var f = MockSchedulerTrackingFixtureRepository.Object.All().FirstOrDefault();

            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.LivePolling, f.SchedulerStateFixtures);
        }
    }
}
