namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using System;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq.Expressions;
    using Hangfire;
    using Hangfire.Common;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public class MotorsportLiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IMotorsportService _motorsportService;
        private readonly IMotorsportIngestWorkerService _motorsportIngestWorkerService;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public MotorsportLiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IMotorsportService motorsportService,
            IMotorsportIngestWorkerService motorsportIngestWorkerService,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _recurringJobManager = recurringJobManager;
            _motorsportService = motorsportService;
            _motorsportIngestWorkerService = motorsportIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveRaceEventData();

            await CreateChildJobsForFetchingPostLiveData();

            //await CreateChildJobsForFetchingRaceEventResults();

            //await CreateChildJobsForFetchingTeamStandings();

            //await CreateChildJobsForFetchingDriverStandings();

            await DeleteLiveJobsForEndedRaceEvents();
        }

        private async Task CreateChildJobsForFetchingPostLiveData()
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

            if (schedulerEvent is null ||
                schedulerEvent.SchedulerStateForMotorsportRaceEventPolling == SchedulerStateForMotorsportRaceEventPolling.PostLivePolling) return;

            UpdateJobDefinitionForRaceEventResults(raceEvent);

            UpdateJobDefinitionForDriverStandings(raceEvent);

            UpdateJobDefinitionForTeamStandings(raceEvent);

            schedulerEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.PostLivePolling;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

            await _systemSportDataUnitOfWork.SaveChangesAsync();

        }

        private async Task CreateChildJobsForFetchingRaceEventResults()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;
                    UpdateJobDefinitionForRaceEventResults(raceEvent);
                }
            }
        }

        private async Task CreateChildJobsForFetchingDriverStandings()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);
                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;
                    UpdateJobDefinitionForDriverStandings(raceEvent);
                }
            }
        }

        private async Task CreateChildJobsForFetchingTeamStandings()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;
                    UpdateJobDefinitionForTeamStandings(raceEvent);
                }
            }
        }

        private async Task CreateChildJobsForFetchingLiveRaceEventData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;

                    UpdateJobDefinitionForLiveRaceEvent(raceEvent);
                }
            }

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

            Expression<Action> jobMethod =
                () => _motorsportIngestWorkerService.IngestResultsForRaceEvent(
                    raceEvent,
                    threadSleepInSeconds,
                    pollingDurationInMinutes,
                    DeleteChildJobForRaceEventResults);

            _recurringJobManager.AddOrUpdate(jobId, Job.FromExpression(jobMethod), jobCronExpression, jobOptions);

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

            Expression<Action> jobMethod =
                () => _motorsportIngestWorkerService.IngestTeamStandingsForLeague(
                    raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes, DeleteChildJobForTeamStandings);

            _recurringJobManager.AddOrUpdate(jobId, Job.FromExpression(jobMethod), jobCronExpression, jobOptions);

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

            Expression<Action> jobMethod = () => _motorsportIngestWorkerService.IngestDriverStandingsForLeague(
                raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes, DeleteChildJobForDriverStandings);

            _recurringJobManager.AddOrUpdate(jobId, Job.FromExpression(jobMethod), jobCronExpression, jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private async Task DeleteLiveJobsForEndedRaceEvents()
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

                    var eventInRepo =
                        _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

                    if (eventInRepo == null) continue;

                    eventInRepo.IsJobRunning = false;

                    eventInRepo.SchedulerStateForMotorsportRaceEventPolling =
                        SchedulerStateForMotorsportRaceEventPolling.PostLivePolling;

                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();

        }

        private async void DeleteChildJobForRaceEventResults(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent == null) return;

            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobId"] +
                raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

            _recurringJobManager.RemoveIfExists(jobId);

            var trackingEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            if (trackingEvent == null) return;

            trackingEvent.IsJobRunning = false;

            trackingEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.PollingFinished;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingEvent);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async void DeleteChildJobForDriverStandings(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent == null) return;

            var jobId = ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobId"] + raceEvent.MotorsportRace.RaceName;

            _recurringJobManager.RemoveIfExists(jobId);

            var trackingEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            if (trackingEvent == null) return;

            trackingEvent.IsJobRunning = false;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingEvent);

            trackingEvent.IsJobRunning = false;

            trackingEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.PollingFinished;

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async void DeleteChildJobForTeamStandings(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent == null) return;

            var jobId = ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobId"] + raceEvent.MotorsportRace.RaceName;

            _recurringJobManager.RemoveIfExists(jobId);

            var trackingEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            if (trackingEvent == null) return;

            trackingEvent.IsJobRunning = false;

            trackingEvent.SchedulerStateForMotorsportRaceEventPolling = SchedulerStateForMotorsportRaceEventPolling.PollingFinished;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingEvent);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }
    }
}