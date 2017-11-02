using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
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
using System.Threading;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    [Category("LogsManagerJob")]
    public class LogsManagerJob_Test
    {
        Mock<IRugbyService> MockRugbyService;
        Mock<IRugbyIngestWorkerService> MockRugbyIngestWorkerService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>> MockSchedulerTrackingSeasonRepository = null;
        Mock<IRecurringJobManager> MockRecurringJobManager;
        Mock<IUnityContainer> MockUnityContainer;
        LogsManagerJob LogsManagerJob;
        Mock<ILoggingService> MockLogger;

        [SetUp]
        public void SetUp()
        {
            MockSchedulerTrackingSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(new List<SchedulerTrackingRugbySeason>());

            MockRugbyService = new Mock<IRugbyService>();
            MockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            MockRecurringJobManager = new Mock<IRecurringJobManager>();
            MockUnityContainer = new Mock<IUnityContainer>();

            LogsManagerJob =
                new LogsManagerJob(
                        MockRecurringJobManager.Object,
                        MockUnityContainer.Object,
                        MockLogger.Object);
        }

        //[Test]
        //public async Task LogsManagerJob_NoCurrentTournaments_NoJobsScheduled()
        //{
        //    MockRugbyService.Setup(r => r.GetActiveTournamentsForMatchesInResultsState()).Returns(Task.FromResult(new List<RugbyTournament>() { }.AsEnumerable()));

        //    await LogsManagerJob.DoWorkAsync();

        //    MockRecurringJobManager.Verify(m => m.AddOrUpdate(
        //                    It.IsAny<string>(),
        //                    It.IsAny<Job>(),
        //                    It.IsAny<string>(),
        //                    It.IsAny<RecurringJobOptions>()),
        //                    Times.Never());
        //}

        //[Test]
        //public async Task LogsManagerJob_OneCurrentTournament_GetActiveTournamentsForMatchesInResultsStateOnce()
        //{
        //    MockRugbyService.Setup(r => r.GetActiveTournamentsForMatchesInResultsState()).Returns(
        //    Task.FromResult(new List<RugbyTournament>() {
        //        new RugbyTournament(){
        //            Id = Guid.NewGuid()
        //        }
        //    }.AsEnumerable()));

        //    await LogsManagerJob.DoWorkAsync();

        //    MockRugbyService.Verify(r => r.GetCurrentProviderSeasonIdForTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Once());
        //}

        //[Test]
        //public async Task LogsManagerJob_TwoCurrentTournament_GetActiveTournamentsForMatchesInResultsStateTwice()
        //{
        //    MockRugbyService.Setup(r => r.GetActiveTournamentsForMatchesInResultsState()).Returns(
        //        Task.FromResult(
        //        new List<RugbyTournament>() {
        //            new RugbyTournament(){
        //                Id = Guid.NewGuid()
        //            },
        //            new RugbyTournament(){
        //                Id = Guid.NewGuid()
        //            }
        //    }.AsEnumerable()));

        //    await LogsManagerJob.DoWorkAsync();

        //    MockRugbyService.Verify(r => r.GetCurrentProviderSeasonIdForTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Exactly(2));
        //}

        //[Test]
        //public async Task LogsManagerJob_ScheduleLogsForTournamentXAndSeason2018()
        //{
        //    MockRugbyService.Setup(r => r.GetActiveTournamentsForMatchesInResultsState()).Returns(
        //        Task.FromResult(new List<RugbyTournament>() {
        //        new RugbyTournament(){
        //            Id = Guid.NewGuid(),
        //            Name = "X"
        //        }
        //    }.AsEnumerable()));

        //    MockRugbyService.Setup(r => r.GetCurrentProviderSeasonIdForTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>())).Returns(Task.FromResult(2018));
        //    MockRugbyService.Setup(r => r.GetSchedulerStateForManagerJobPolling(It.IsAny<Guid>())).Returns(Task.FromResult(SchedulerStateForManagerJobPolling.NotRunning));

        //    await LogsManagerJob.DoWorkAsync();

        //    MockRecurringJobManager.Verify(m => m.AddOrUpdate(
        //                It.IsAny<string>(),
        //                It.IsAny<Job>(),
        //                It.IsAny<string>(),
        //                It.IsAny<RecurringJobOptions>()),
        //                Times.Once());
        //}

        //[Test]
        //public async Task LogsManagerJob_ScheduleLogsForTournament_SchedulerStateSetToRunning()
        //{
        //    var tournamentId = Guid.NewGuid();
        //    var seasonId = Guid.NewGuid();
            
        //    MockSchedulerTrackingSeasonRepository.Object.Add(
        //        new SchedulerTrackingRugbySeason()
        //        {
        //            TournamentId = tournamentId,
        //            SeasonId = seasonId,
        //            RugbySeasonStatus = RugbySeasonStatus.InProgress,
        //            SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning
        //        });

        //    await MockSchedulerTrackingSeasonRepository.Object.SaveAsync();

        //    MockRugbyService.Setup(r => r.GetActiveTournamentsForMatchesInResultsState()).Returns(
        //        Task.FromResult(new List<RugbyTournament>() {
        //        new RugbyTournament(){
        //            Id = tournamentId,
        //            Name = "X"
        //        }
        //    }.AsEnumerable()));

        //    MockRugbyService.Setup(r => r.GetCurrentProviderSeasonIdForTournament(It.IsAny<CancellationToken>(), It.IsAny<Guid>())).Returns(Task.FromResult(2018));
        //    MockRugbyService.Setup(r => r.GetSchedulerStateForManagerJobPolling(It.IsAny<Guid>())).Returns(Task.FromResult(SchedulerStateForManagerJobPolling.NotRunning));

        //    await LogsManagerJob.DoWorkAsync();

        //    var seasonEntry = MockSchedulerTrackingSeasonRepository.Object.All().FirstOrDefault();

        //    Assert.IsNotNull(seasonEntry);
        //    Assert.AreEqual(SchedulerStateForManagerJobPolling.Running, seasonEntry.SchedulerStateForManagerJobPolling);
        //}
    }
}
