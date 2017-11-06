namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUnityContainer _childContainer;
        private readonly ILoggingService _logger;

        public LiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer,
            ILoggingService logger)
        {
            _childContainer = childContainer;
            _recurringJobManager = recurringJobManager;
            _logger = logger;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures();
            await DeleteChildJobsForFetchingMatchDataForFixturesEnded();
            await DeleteChildJobsForFetchingMatchDataForFixturesTooFarInThePast();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesTooFarInThePast()
        {
            var _schedulerTrackingRugbyFixtureRepository =
                            _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();

            var postponedFixtures =
                    await _childContainer.Resolve<IRugbyService>().GetPostponedFixtures();

            var p = postponedFixtures.ToList();

            foreach (var fixture in postponedFixtures)
            {
                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                        (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(
                            f => f.FixtureId == fixture.Id).FirstOrDefault();

                if (fixtureInDb != null &&
                    fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted)
                {
                    _logger.Info("Setting SchedulerStateFixtures for " + matchName + " to SchedulingNotYetStarted.");
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted;
                    _schedulerTrackingRugbyFixtureRepository.Update(fixtureInDb);
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesEnded()
        {
            var _schedulerTrackingRugbyFixtureRepository =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();

            var completedFixtures =
                    await _childContainer.Resolve<IRugbyService>().GetCompletedFixtures();

            foreach (var fixture in completedFixtures)
            {
                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                        (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(
                            f => f.FixtureId == fixture.Id).FirstOrDefault();

                if (fixtureInDb != null && 
                    fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingCompleted)
                {
                    _logger.Info("Setting SchedulerStateFixtures for " + matchName + " to SchedulingCompleted.");
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted;
                    _schedulerTrackingRugbyFixtureRepository.Update(fixtureInDb);
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures()
        {
            var _schedulerTrackingRugbyFixtureRepository =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();

            var currentTournaments =
                    await _childContainer.Resolve<IRugbyService>().GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                var liveFixtures =
                    await _childContainer.Resolve<IRugbyService>().GetLiveFixturesForCurrentTournament(CancellationToken.None, tournament.Id);

                _logger.Info("There are " + liveFixtures.Count() + " live fixtures for tournament " + tournament.Name);

                foreach (var fixture in liveFixtures)
                {
                    var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                    var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;
                    var jobCronExpression = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() => _childContainer.Resolve<IRugbyIngestWorkerService>().IngestLiveMatchData(CancellationToken.None, fixture.ProviderFixtureId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var fixtureInDb =
                            (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(f => f.FixtureId == fixture.Id).FirstOrDefault();

                    if (fixtureInDb != null && 
                        fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.LivePolling)
                    {
                        _logger.Info("Setting SchedulerStateFixtures for " + matchName + " to LivePolling.");
                        _recurringJobManager.Trigger(jobId);
                        fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling;
                        _schedulerTrackingRugbyFixtureRepository.Update(fixtureInDb);
                    }
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }
    }
}
