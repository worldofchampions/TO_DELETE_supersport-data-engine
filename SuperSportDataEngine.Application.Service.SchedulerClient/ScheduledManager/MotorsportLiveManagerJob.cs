namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using System;
    using System.Linq;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public class MotorsportLiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IMotorsportService _motorsportService;
        private readonly IMotorsportIngestWorkerService _motorsportIngestWorkerService;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        public MotorsportLiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IMotorsportService motorsportService,
            IMotorsportIngestWorkerService motorsportIngestWorkerService,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _recurringJobManager = recurringJobManager;
            _motorsportService = motorsportService;
            _motorsportIngestWorkerService = motorsportIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForLiveData();

            await CreateChildJobsForPostLiveData();

            await DeleteChildJobsForLiveData();

            await DeleteChildJobsForPostLiveData();

        }

        private async Task CreateChildJobsForLiveData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetLiveEventForLeague(league.Id);

                if (raceEvent == null) continue;

                UpdateJobDefinitionForLiveRaceEvent(raceEvent);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobsForPostLiveData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;

                    await UpdateJobDefinitionForPostLiveData(raceEvent);
                }
            }
        }

        private async Task UpdateJobDefinitionForPostLiveData(MotorsportRaceEvent raceEvent)
        {
            var schedulerEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                e.MotorsportRaceEventId == raceEvent.Id &&
                e.MotorsportRaceEventStatus == MotorsportRaceEventStatus.Result);

            if (schedulerEvent is null
                || schedulerEvent.SchedulerStateForMotorsportRaceEventPolling == SchedulerStateForMotorsportRaceEventPolling.PostLivePolling
                || schedulerEvent.SchedulerStateForMotorsportRaceEventPolling == SchedulerStateForMotorsportRaceEventPolling.PollingFinished) return;

            UpdateJobDefinitionForRaceEventResults(raceEvent);

            UpdateJobDefinitionForDriverStandings(raceEvent);

            UpdateJobDefinitionForTeamStandings(raceEvent);

            schedulerEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.PostLivePolling;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

            await _systemSportDataUnitOfWork.SaveChangesAsync();

        }

        private void UpdateJobDefinitionForLiveRaceEvent(MotorsportRaceEvent raceEvent)
        {
            var schedulerEvent =
                 _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                 e.MotorsportRaceEventId == raceEvent.Id);

            if (schedulerEvent != null && schedulerEvent.IsJobRunning != true)
            {
                var jobId =
                    ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] +
                    raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

                var threadSleepPollingInSeconds =
                    int.Parse(ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_ThreadSleepPollingInSeconds"]);

                var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

                _recurringJobManager.AddOrUpdate(
                    jobId,
                    Job.FromExpression(() =>
                    _motorsportIngestWorkerService.IngestLiveRaceEventData(raceEvent, threadSleepPollingInSeconds, CancellationToken.None)),
                    jobCronExpression,
                    jobOptions);

                schedulerEvent.IsJobRunning = true;

                schedulerEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.LivePolling;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

                _recurringJobManager.Trigger(jobId);
            }
        }

        private void UpdateJobDefinitionForRaceEventResults(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                    ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobIdPrefix"] +
                    raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

            var jobCronExpression =
                ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobCronExpression"];

            var threadSleepInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_PollingDurationInMinutes"]);

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                _motorsportIngestWorkerService.IngestResultsForRaceEvent(raceEvent, threadSleepInSeconds, pollingDurationInMinutes)),
                jobCronExpression,
                jobOptions);

            _recurringJobManager.Trigger(jobId);

        }

        private void UpdateJobDefinitionForTeamStandings(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobCronExpression"];

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_PollingDurationInMinutes"]);

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                _motorsportIngestWorkerService.IngestTeamStandingsForLeague(raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes)),
                jobCronExpression, jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private void UpdateJobDefinitionForDriverStandings(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobCronExpression"];

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_PollingDurationInMinutes"]);

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                _motorsportIngestWorkerService.IngestDriverStandingsForLeague(raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes)),
                jobCronExpression,
                jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private async Task DeleteChildJobsForLiveData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;

                    var jobId = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] +
                                raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                    _recurringJobManager.RemoveIfExists(jobId);

                }
            }

        }

        private async Task DeleteChildJobsForPostLiveData()
        {
            await DeleteChildJobForRaceEventResults();

            await DeleteChildJobsForStandings();
        }

        private async Task DeleteChildJobForRaceEventResults()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var trackingMotorsportRaceEvents =
                    (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.WhereAsync(e =>
                        e.MotorsportLeagueId == league.Id
                        && e.SchedulerStateForMotorsportRaceEventPolling == SchedulerStateForMotorsportRaceEventPolling.PostLivePolling))
                    .Where(IsResultsPollingDurationOver);

                foreach (var trackingEvent in trackingMotorsportRaceEvents)
                {
                    if (trackingEvent == null) continue;

                    var raceEvent =
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.Id == trackingEvent.MotorsportRaceEventId);

                    if (raceEvent == null) continue;

                    var jobId =
                        ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobIdPrefix"] +
                        raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                    _recurringJobManager.RemoveIfExists(jobId);

                }
            }
        }

        private async Task DeleteChildJobsForStandings()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var trackingMotorsportRaceEvents =
                    (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.WhereAsync(e =>
                        e.MotorsportLeagueId == league.Id
                        && e.SchedulerStateForMotorsportRaceEventPolling == SchedulerStateForMotorsportRaceEventPolling.PostLivePolling))
                    .Where(IsStandingsPollingDurationOver);

                foreach (var trackingEvent in trackingMotorsportRaceEvents)
                {
                    if (trackingEvent == null) continue;

                    var raceEvent =
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.Id == trackingEvent.MotorsportRaceEventId);

                    if (raceEvent == null) continue;

                    var jobId = 
                        ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

                    _recurringJobManager.RemoveIfExists(jobId);

                    jobId = 
                        ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

                    _recurringJobManager.RemoveIfExists(jobId);

                    trackingEvent.IsJobRunning = false;

                    trackingEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.PollingFinished;

                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingEvent);

                    await _systemSportDataUnitOfWork.SaveChangesAsync();
                }
            }
        }

        private static bool IsStandingsPollingDurationOver(SchedulerTrackingMotorsportRaceEvent raceEvent)
        {
            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_PollingDurationInMinutes"]);

            return raceEvent.EndedDateTimeUtc != null && DateTimeOffset.UtcNow.Subtract(raceEvent.EndedDateTimeUtc.Value).TotalMinutes >= pollingDurationInMinutes;
        }

        private static bool IsResultsPollingDurationOver(SchedulerTrackingMotorsportRaceEvent raceEvent)
        {
            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_PollingDurationInMinutes"]);

            return raceEvent.EndedDateTimeUtc != null && DateTimeOffset.UtcNow.Subtract(raceEvent.EndedDateTimeUtc.Value).TotalMinutes >= pollingDurationInMinutes;
        }
    }
}