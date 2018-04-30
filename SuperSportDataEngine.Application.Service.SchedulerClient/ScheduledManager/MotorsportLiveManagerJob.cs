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
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

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

            await CreateChildJobsForFetchingRaceEventResults();

            await CreateChildJobsForFetchingTeamStandings();

            await CreateChildJobsForFetchingDriverStandings();

            await DeleteLiveJobsForEndedRaceEvents();
        }

        private async Task CreateChildJobsForFetchingRaceEventResults()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetEndedRaceEventForLeague(league.Id);

                if (raceEvent != null)
                {
                    UpdateJobDefinitionForRaceEventResults(raceEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobsForFetchingDriverStandings()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetEndedRaceEventForLeague(league.Id);

                if (raceEvent != null)
                {
                    UpdateJobDefinitionForDriverStandings(raceEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();

        }

        private async Task CreateChildJobsForFetchingTeamStandings()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetEndedRaceEventForLeague(league.Id);

                if (raceEvent != null)
                {
                    UpdateJobDefinitionForTeamStandings(raceEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobsForFetchingLiveRaceEventData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetEndedRaceEventForLeague(league.Id);

                if (raceEvent != null)
                {
                    UpdateJobDefinitionForLiveRaceEvent(raceEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private void UpdateJobDefinitionForLiveRaceEvent(MotorsportRaceEvent raceEvent)
        {
            var trackingMotorsportRaceEvent =
                 _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                 e.MotorsportRaceEventId == raceEvent.Id);

            if (trackingMotorsportRaceEvent != null && trackingMotorsportRaceEvent.IsJobRunning != true)
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

                trackingMotorsportRaceEvent.IsJobRunning = true;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingMotorsportRaceEvent);

                _recurringJobManager.Trigger(jobId);
            }
        }

        private void UpdateJobDefinitionForRaceEventResults(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobId"] +
                raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

            var jobCronExpression =
                ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults__JobCronExpression"];

            var threadSleepInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_ThreadSleepInSeconds"]);

            var jobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.HighPriority
            };

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(
                    () => _motorsportIngestWorkerService.IngestResultsForRaceEvent(raceEvent, threadSleepInSeconds, DeleteChildJobForRaceEventResults)),
                jobCronExpression,
                jobOptions);

            _recurringJobManager.Trigger(jobId);

        }

        private void UpdateJobDefinitionForTeamStandings(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_ThreadSleepPollingInSeconds"]);

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => _motorsportIngestWorkerService.IngestTeamStandingsForLeague(raceEvent, threadSleepPollingInSeconds, DeleteChildJobForTeamStandings)),
                jobCronExpression,
                jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private void UpdateJobDefinitionForDriverStandings(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_ThreadSleepPollingInSeconds"]);

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => _motorsportIngestWorkerService.IngestDriverStandingsForLeague(raceEvent, threadSleepPollingInSeconds, DeleteChildJobForDriverStandings)),
                jobCronExpression,
                jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private async Task DeleteLiveJobsForEndedRaceEvents()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetEndedRaceEventForLeague(league.Id);

                if (raceEvent == null) continue;

                var jobId = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] +
                            raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                _recurringJobManager.RemoveIfExists(jobId);

                var eventInRepo =
                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

                eventInRepo.IsJobRunning = false;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);
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

            var eventInRepo =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            eventInRepo.IsJobRunning = false;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async void DeleteChildJobForDriverStandings(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent == null) return;

            var jobId = ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobId"] + raceEvent.MotorsportRace.RaceName;

            _recurringJobManager.RemoveIfExists(jobId);

            var eventInRepo =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            eventInRepo.IsJobRunning = false;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);

            await _systemSportDataUnitOfWork.SaveChangesAsync();

        }

        private async void DeleteChildJobForTeamStandings(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent == null) return;

            var jobId = ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobId"] + raceEvent.MotorsportRace.RaceName;

            _recurringJobManager.RemoveIfExists(jobId);

            var eventInRepo =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            eventInRepo.IsJobRunning = false;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

    }
}