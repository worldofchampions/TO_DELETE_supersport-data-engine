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
            await CreateChildJobsForFetchingLiveRaceData();

            await DeleteChildJobsForEndedRaceEvents();
        }

        private async Task DeleteChildJobsForEndedRaceEvents()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetEndedRaceEventForLeague(league.Id);

                if (raceEvent == null) continue;

                var jobId = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] + raceEvent.MotorsportRace.RaceName;

                _recurringJobManager.RemoveIfExists(jobId);

                var eventInRepo =
                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

                eventInRepo.IsJobRunning = false;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobsForFetchingLiveRaceData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetLiveEventForLeague(league.Id);

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
                    ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] + raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

                var threadSleepPollingInSeconds = int.Parse(ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_ThreadSleepPollingInSeconds"]);

                var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

                _recurringJobManager.AddOrUpdate(
                    jobId,
                    Job.FromExpression(() => _motorsportIngestWorkerService.IngestLiveRaceEventData(raceEvent, threadSleepPollingInSeconds, CancellationToken.None)),
                    jobCronExpression,
                    jobOptions);

                trackingMotorsportRaceEvent.IsJobRunning = true;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingMotorsportRaceEvent);

                _recurringJobManager.Trigger(jobId);
            }
        }
    }
}