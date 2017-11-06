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
    public class LiveManagerJob_Test
    {
        LiveManagerJob LiveManagerJob;
        Mock<IRugbyService> MockRugbyService;
        Mock<IRugbyIngestWorkerService> MockRugbyIngestWorkerService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>> MockSchedulerTrackingFixtureRepository = null;
        Mock<IRecurringJobManager> MockRecurringJobManager;
        Mock<IUnityContainer> MockUnityContainer;
        Mock<ILoggingService> MockLogger;

        [SetUp]
        public void SetUp()
        {
            MockSchedulerTrackingFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(new List<SchedulerTrackingRugbyFixture>());

            MockRugbyService = new Mock<IRugbyService>();
            MockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            MockRecurringJobManager = new Mock<IRecurringJobManager>();
            MockUnityContainer = new Mock<IUnityContainer>();
            MockLogger = new Mock<ILoggingService>();

            LiveManagerJob =
                new LiveManagerJob();
        }

        //[Test]
        //public async Task LiveManagerJob_EmptyDataSet_AssertEmpty()
        //{
        //    long count = await MockSchedulerTrackingFixtureRepository.Object.CountAsync();
        //    Assert.AreEqual(0, count);
        //}

        //[Test]
        //public async Task LiveManagerJob_NoEndedFixtures_NoJobsRemoved()
        //{
        //    await LiveManagerJob.DoWorkAsync();

        //    MockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Never());
        //}

        //[Test]
        //public async Task LiveManagerJob_HasEndedFixture_OnlyOneJobRemoved()
        //{
        //    MockRugbyService
        //        .Setup(r => r.GetEndedFixtures()).Returns(
        //            Task.FromResult(new List<RugbyFixture>() {
        //                new RugbyFixture
        //                {
        //                    TeamA = new RugbyTeam() { Name = "TeamA" },
        //                    TeamB = new RugbyTeam() { Name = "TeamB" },
        //                    RugbyFixtureStatus = RugbyFixtureStatus.Final
        //                }
        //            }.AsEnumerable()));

        //    await LiveManagerJob.DoWorkAsync();

        //    MockRecurringJobManager.Verify(m => m.RemoveIfExists(It.IsAny<string>()), Times.Once());
        //}

        //[Test]
        //public async Task LiveManagerJob_NoCurrentTournaments_DoNotQueryForLiveFixtures()
        //{
        //    MockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(Task.FromResult(new List<RugbyTournament>().AsEnumerable()));

        //    await LiveManagerJob.DoWorkAsync();

        //    MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never());
        //}

        //[Test]
        //public async Task LiveManagerJob_OneCurrentTournament_OneCallForLiveFixtures()
        //{
        //    MockRugbyService.Setup(r => r.GetCurrentTournaments()).Returns(
        //            Task.FromResult(new List<RugbyTournament>(){
        //                new RugbyTournament(){Id = Guid.NewGuid()}
        //            }.AsEnumerable()));

        //    MockRugbyService.Setup(r => r.GetEndedFixtures()).Returns(
        //            Task.FromResult(new List<RugbyFixture>().AsEnumerable()));

        //    await LiveManagerJob.DoWorkAsync();

        //    MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Once());
        //}

        //[Test]
        //public async Task LiveManagerJob_TwoCurrentTournaments_TwoCallsForLiveFixtures()
        //{
        //    MockRugbyService
        //        .Setup(r => r.GetCurrentTournaments()).Returns(
        //            Task.FromResult(new List<RugbyTournament>()
        //            {
        //                new RugbyTournament(){Id = Guid.NewGuid()},
        //                new RugbyTournament(){Id = Guid.NewGuid()}
        //            }.AsEnumerable()));

        //    MockRugbyService
        //        .Setup(r => r.GetEndedFixtures()).Returns(
        //            Task.FromResult(new List<RugbyFixture>().AsEnumerable()));

        //    await LiveManagerJob.DoWorkAsync();

        //    MockRugbyService.Verify(m => m.GetLiveFixturesForCurrentTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Exactly(2));
        //}

        //[Test]
        //public async Task LiveManagerJob_InProgressFixtureGetsScheduled()
        //{
        //    Guid tournamentId = Guid.NewGuid();
        //    Guid fixtureId = Guid.NewGuid();

        //    MockSchedulerTrackingFixtureRepository.Object.Add(
        //        new SchedulerTrackingRugbyFixture { TournamentId = tournamentId, FixtureId = fixtureId, SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted });

        //    var tournament = new RugbyTournament() { Id = tournamentId };

        //    MockRugbyService
        //        .Setup(r => r.GetCurrentTournaments()).Returns(
        //            Task.FromResult(new List<RugbyTournament>() { tournament }.AsEnumerable()));

        //    var rugbyFixtures =
        //        Task.FromResult(new List<RugbyFixture>()
        //            {
        //                new RugbyFixture
        //                {
        //                    Id = fixtureId,
        //                    RugbyTournament = tournament,
        //                    TeamA = new RugbyTeam { Name = "TeamA" },
        //                    TeamB = new RugbyTeam { Name = "TeamB" },
        //                    RugbyFixtureStatus = RugbyFixtureStatus.InProgress
        //                }
        //            }.AsEnumerable());

        //    MockRugbyService.Setup(r => r.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentId)).Returns(rugbyFixtures);

        //    await LiveManagerJob.DoWorkAsync();

        //    MockRecurringJobManager.Verify(m => m.AddOrUpdate(
        //                "LiveManagerJob→LiveMatch→TeamA vs TeamB",
        //                It.IsAny<Job>(),
        //                "0 */2 * * *",
        //                It.IsAny<RecurringJobOptions>()),
        //                Times.Once());

        //    var f = MockSchedulerTrackingFixtureRepository.Object.All().FirstOrDefault();

        //    Assert.AreEqual(SchedulerStateForRugbyFixturePolling.LivePolling, f.SchedulerStateFixtures);
        //}
    }
}
