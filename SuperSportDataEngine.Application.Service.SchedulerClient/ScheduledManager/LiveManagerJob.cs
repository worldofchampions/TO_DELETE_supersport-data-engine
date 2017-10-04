namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
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
        private readonly IRugbyService _rugbyService;
        private readonly IRugbyIngestWorkerService _rugbyIngestService;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;

        public LiveManagerJob(
            IRugbyService rugbyService,
            IRugbyIngestWorkerService rugbyIngestWorkerService,
            IRecurringJobManager recurringJobManager,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepository)
        {
            _rugbyService = rugbyService;
            _rugbyIngestService = rugbyIngestWorkerService;
            _recurringJobManager = recurringJobManager;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepository;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures();
            await DeleteChildJobsForFetchingMatchDataForFixturesEnded();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesEnded()
        {
            var endedGames =
                    _rugbyService.GetEndedFixtures();

            foreach (var fixture in endedGames)
            {
                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                        _schedulerTrackingRugbyFixtureRepository.Where(
                            f => f.FixtureId == fixture.Id).FirstOrDefault();

                if (fixtureInDb != null)
                {
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PostLivePolling;
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures()
        {
            var currentTournaments =
                    _rugbyService.GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                var liveFixtures =
                    _rugbyService.GetLiveFixturesForCurrentTournament(tournament.Id);

                foreach (var fixture in liveFixtures)
                {
                    var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                    var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;
                    var jobCronExpression = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() => _rugbyIngestService.IngestMatchStatsForFixture(CancellationToken.None, fixture.ProviderFixtureId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Utc,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var fixtureInDb =
                            _schedulerTrackingRugbyFixtureRepository.All().Where(f => f.FixtureId == fixture.Id).FirstOrDefault();

                    if (fixtureInDb != null && 
                        fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.LivePolling)
                    {
                        _recurringJobManager.Trigger(jobId);
                        fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling;
                    }
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }
    }
}
