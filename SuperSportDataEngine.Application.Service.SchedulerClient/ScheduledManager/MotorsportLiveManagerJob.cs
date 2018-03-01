using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{

    public class MotorsportLiveManagerJob
    {
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _childContainer;
        private readonly IMotorsportService _motorsportService;
        private readonly IMotorsportIngestWorkerService _motorsportIngestWorkerService;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        private int _tempCount = 0;

        public MotorsportLiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer,
            IMotorsportService motorsportService,
            IMotorsportIngestWorkerService motorsportIngestWorkerService,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _childContainer = childContainer;
            _motorsportService = motorsportService;
            _motorsportIngestWorkerService = motorsportIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _recurringJobManager = recurringJobManager;
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
                var raceEvent = await _motorsportService.GetTodayEventForRace(league.Id);

                if (ShouldSpawnLiveJobForRaceEvent(raceEvent))
                {
                    UpdateJobDefinitionForLiveRaceEvent(raceEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private void UpdateJobDefinitionForLiveRaceEvent(MotorsportRaceEvent raceEvent)
        {
            var raceInTrackingRepo =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaces.FirstOrDefault(r => r.MotorsportRaceId == raceEvent.MotorsportRace.Id);

            if (raceInTrackingRepo is null)
            {
                var jobId = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] + raceEvent.MotorsportRace.RaceName;

                var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

                var jobMethod = _motorsportIngestWorkerService.IngestLiveRaceEventData(raceEvent.MotorsportRace, CancellationToken.None);

                var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

                _recurringJobManager.AddOrUpdate(jobId, Job.FromExpression(() => jobMethod), jobCronExpression, jobOptions);

                _recurringJobManager.Trigger(jobId);

                raceInTrackingRepo = new SchedulerTrackingMotorsportRace
                {
                    IsJobRunning = true,
                    //MotorsportRaceId = raceEvent.MotorsportRaceId,
                    //MotorsportRaceStatus = raceEvent.MotorsportRace.MotorsportRaceStatus,
                    SchedulerStateForMotorsportRacePolling = SchedulerStateForMotorsportRacePolling.RunningAt1MinuteCycle
                };

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaces.Add(raceInTrackingRepo);
            }
        }

        private bool ShouldSpawnLiveJobForRaceEvent(MotorsportRaceEvent raceEvent)
        {
            //if (_tempCount != 0)
            //{
            //    var currentTime = DateTimeOffset.UtcNow;

            //    return currentTime >= raceEvent.StartDateTimeUtc && currentTime <= raceEvent.EndDateTimeUtc;
            //}

            return true;
        }
    }
}