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
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUnityContainer _childContainer;

        public LiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer)
        {
            _childContainer = childContainer;
            _recurringJobManager = recurringJobManager;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures();
            await DeleteChildJobsForFetchingMatchDataForFixturesEnded();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesEnded()
        {
            var _schedulerTrackingRugbyFixtureRepository =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();

            var endedGames =
                    await _childContainer.Resolve<IRugbyService>().GetEndedFixtures();

            foreach (var fixture in endedGames)
            {
                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                        (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(
                            f => f.FixtureId == fixture.Id).FirstOrDefault();

                if (fixtureInDb != null)
                {
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PostLivePolling;
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
                            TimeZone = TimeZoneInfo.Utc,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var fixtureInDb =
                            (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(f => f.FixtureId == fixture.Id).FirstOrDefault();

                    if (fixtureInDb != null && 
                        fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.LivePolling)
                    {
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
