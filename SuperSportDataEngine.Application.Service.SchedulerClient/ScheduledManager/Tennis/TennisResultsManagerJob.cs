using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager.Tennis
{
    public class TennisResultsManagerJob
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly ITennisIngestWorkerService _tennisIngestWorkerService;
        private readonly IRecurringJobManager _recurringJobManager;

        public TennisResultsManagerJob(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            ITennisIngestWorkerService tennisIngestWorkerService, 
            IRecurringJobManager recurringJobManager)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _tennisIngestWorkerService = tennisIngestWorkerService;
            _recurringJobManager = recurringJobManager;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobFetchingResults();
            await DeleteChildJobFetchingResults();
        }

        private async Task DeleteChildJobFetchingResults()
        {
            var events =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.AllAsync()).ToList();

            var endedEvents = events.Where(HasEventEnded);

            foreach (var schedulerTrackingTennisEvent in endedEvents)
            {
                var tennisEvent = _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.Id == schedulerTrackingTennisEvent.TennisEventId);

                var leagues = tennisEvent.TennisTournament.TennisLeagues;

                foreach (var league in leagues)
                {
                    var resultsJobName = ConfigurationManager.AppSettings["TennisChildJob_Results_JobIdPrefix"] + league.Abbreviation + "→" + tennisEvent.EventName;

                    _recurringJobManager.RemoveIfExists(resultsJobName);
                }
            }
        }

        private async Task CreateChildJobFetchingResults()
        {
            var events =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.AllAsync()).ToList();

            var currentEvents = events.Where(IsEventCurrent);

            foreach (var schedulerTrackingTennisEvent in currentEvents)
            {
                var tennisEvent = _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.Id == schedulerTrackingTennisEvent.TennisEventId);

                var leagues = tennisEvent.TennisTournament.TennisLeagues;

                foreach (var league in leagues)
                {
                    var resultsJobName = ConfigurationManager.AppSettings["TennisChildJob_Results_JobIdPrefix"] + league.Abbreviation + "→" + tennisEvent.EventName;
                    var resultsJobExpression = ConfigurationManager.AppSettings["TennisChildJob_Results_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        resultsJobName,
                        Job.FromExpression(() => _tennisIngestWorkerService.IngestResultsForEvent(league.ProviderSlug, tennisEvent.ProviderEventId, CancellationToken.None)),
                        resultsJobExpression,
                        new RecurringJobOptions
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.NormalPriority
                        });
                }
            }
        }

        private static bool IsEventCurrent(SchedulerTrackingTennisEvent tennisEvent)
        {
            var minTime = tennisEvent.StartDateTime.AddHours(-6);
            var maxTime = tennisEvent.EndDateTime.AddHours(3);

            var now = DateTime.UtcNow;

            return now > minTime && now < maxTime;
        }

        private static bool HasEventEnded(SchedulerTrackingTennisEvent tennisEvent)
        {
            var maxTime = tennisEvent.EndDateTime.AddHours(4);

            var now = DateTime.UtcNow;

            return now > maxTime;
        }
    }
}
