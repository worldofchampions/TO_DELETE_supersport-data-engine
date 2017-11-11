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
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IRugbyService _rugbyService;
        private readonly IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private readonly ILoggingService _logger;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;

        public LiveManagerJob(
            ILoggingService logger,
            IRecurringJobManager recurringJobManager,
            IRugbyService rugbyService,
            IRugbyIngestWorkerService rugbyIngestWorkerService,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepository)
        {
            _logger = logger;
            _recurringJobManager = recurringJobManager;
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepository;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures();
            try
            {
                await DeleteChildJobsForFetchingMatchDataForFixturesEnded();
                await DeleteChildJobsForFetchingMatchDataForFixturesTooFarInThePast();
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
            }
 
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesTooFarInThePast()
        {
            var postponedFixtures =
                    (await _rugbyService.GetPostponedFixtures()).ToList();

            var p = postponedFixtures.ToList();

            foreach (var fixture in postponedFixtures)
            {
                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                        (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).FirstOrDefault(f => f.FixtureId == fixture.Id);

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
            var completedFixtures =
                    await _rugbyService.GetCompletedFixtures();

            foreach (var fixture in completedFixtures)
            {
                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                        (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).FirstOrDefault(f => f.FixtureId == fixture.Id);

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
            var currentTournaments =
                    await _rugbyService.GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                try
                {
                    var liveFixtures =
                    (await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournament.Id)).ToList();

                    _logger.Info("There are " + liveFixtures.Count() + " live fixtures for tournament " + tournament.Name);

                    foreach (var fixture in liveFixtures)
                    {
                        try
                        {
                            var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;

                            var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;
                            var jobCronExpression = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobCronExpression"];
                            _logger.Info(jobId + " " + jobCronExpression);

                            _recurringJobManager.AddOrUpdate(
                                jobId,
                                Job.FromExpression(() => _rugbyIngestWorkerService.IngestLiveMatchData(CancellationToken.None, fixture.ProviderFixtureId)),
                                jobCronExpression,
                                new RecurringJobOptions()
                                {
                                    TimeZone = TimeZoneInfo.Local,
                                    QueueName = HangfireQueueConfiguration.HighPriority
                                });

                            var fixtureInDb =
                                (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).FirstOrDefault(f => f.FixtureId == fixture.Id);

                            if (fixtureInDb != null && fixtureInDb.IsJobRunning != true)
                            {
                                fixtureInDb.IsJobRunning = true;
                                _schedulerTrackingRugbyFixtureRepository.Update(fixtureInDb);
                                _recurringJobManager.Trigger(jobId);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e.StackTrace);
                        }

                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.StackTrace);
                }
                
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }
    }
}
