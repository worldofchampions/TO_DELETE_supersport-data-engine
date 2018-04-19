using System.Linq;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using System;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

    public class MotorsportLiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IMotorsportService _motorsportService;
        private readonly IMotorsportIngestWorkerService _motorsportIngestWorkerService;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public MotorsportLiveManagerJob(IRecurringJobManager recurringJobManager, IMotorsportService motorsportService, IMotorsportIngestWorkerService motorsportIngestWorkerService, ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _recurringJobManager = recurringJobManager;
            _motorsportService = motorsportService;
            _motorsportIngestWorkerService = motorsportIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveRaceData();
        }

        private async Task CreateChildJobsForFetchingLiveRaceData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetTodayEventForLeague(league.Id);

                if (raceEvent != null)
                {
                    await UpdateJobDefinitionForLiveRaceEvent(raceEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task UpdateJobDefinitionForLiveRaceEvent(MotorsportRaceEvent raceEvent)
        {
            var jobId = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] + raceEvent.MotorsportRace.RaceName;

            var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

            var threadSleepPollingInSeconds = int.Parse(ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_ThreadSleepPollingInSeconds"]);

            var jobMethod = _motorsportIngestWorkerService.IngestLiveRaceEventData(raceEvent, threadSleepPollingInSeconds, CancellationToken.None);

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            _recurringJobManager.AddOrUpdate(jobId, Job.FromExpression(() => jobMethod), jobCronExpression, jobOptions);

            var eventInRepo = (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.AllAsync())
                .FirstOrDefault(e => e.MotorsportRaceEventId == raceEvent.Id);

            if (eventInRepo != null && eventInRepo.IsJobRunning != true)
            {
                eventInRepo.IsJobRunning = true;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(eventInRepo);

                _recurringJobManager.Trigger(jobId);
            }
        }
    }
}