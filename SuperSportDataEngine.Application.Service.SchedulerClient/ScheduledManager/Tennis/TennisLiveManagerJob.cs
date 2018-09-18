using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager.Tennis
{
    public class TennisLiveManagerJob
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ITennisIngestWorkerService _tennisIngestWorkerService;
        private readonly int _preLivePollingDurationInMinutes;

        public TennisLiveManagerJob(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            IRecurringJobManager recurringJobManager,
            ITennisIngestWorkerService tennisIngestWorkerService)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _recurringJobManager = recurringJobManager;
            _tennisIngestWorkerService = tennisIngestWorkerService;

            _preLivePollingDurationInMinutes = 
                int.Parse(ConfigurationManager.AppSettings["TennisPreLivePollingDurationInMinutes"]);
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobPollingLiveMatchData();
            await DeleteChildJobPollingLiveMatchData();
        }

        private async Task DeleteChildJobPollingLiveMatchData()
        {
            var completedMatches =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.AllAsync())
                .Where(t =>
                    t.SchedulerStateForTennisMatchPolling == SchedulerStateForTennisMatchPolling.PollingComplete)
                .Select(t => t.TennisMatchId).ToList();

            //[TODO] Limit this list to the past few days matches.
            var matches =
                (await _publicSportDataUnitOfWork.TennisMatches.AllAsync())
                .Where(m =>
                    completedMatches
                        .Contains(m.Id)).ToList();

            foreach (var tennisMatch in matches)
            {
                var liveJobName = 
                    ConfigurationManager.AppSettings["TennisChildJob_Live_JobIdPrefix"] +
                    tennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.EventName + "→" + 
                    tennisMatch.ProviderMatchId;

                _recurringJobManager.RemoveIfExists(liveJobName);
            }
        }

        private async Task CreateChildJobPollingLiveMatchData()
        {
            var upcomingMatches = 
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.AllAsync())
                    .Where(t => 
                        IsUpcoming(t.StartDateTime) &&
                        t.SchedulerStateForTennisMatchPolling == SchedulerStateForTennisMatchPolling.NotStarted)
                    .Select(t => t.TennisMatchId).ToList();

            var matches =
                (await _publicSportDataUnitOfWork.TennisMatches.AllAsync())
                .Where(m => 
                    upcomingMatches
                .Contains(m.Id)).ToList();

            foreach (var tennisMatch in matches)
            {
                var liveJobName = 
                    ConfigurationManager.AppSettings["TennisChildJob_Live_JobIdPrefix"] + 
                    tennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.EventName + "→" + 
                    tennisMatch.ProviderMatchId;

                var liveJobExpression = ConfigurationManager.AppSettings["TennisChildJob_Live_JobCronExpression"];

                _recurringJobManager.AddOrUpdate(
                    liveJobName,
                    Job.FromExpression(() => _tennisIngestWorkerService.IngestResultsForMatch(
                        tennisMatch.AssociatedTennisEventTennisLeague.TennisLeague.ProviderSlug,
                        tennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.ProviderEventId,
                        tennisMatch.ProviderMatchId, CancellationToken.None)),
                    liveJobExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local,
                        QueueName = HangfireQueueConfiguration.NormalPriority
                    });

                var trackingEvent = _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.FirstOrDefault(
                    t => t.TennisMatchId == tennisMatch.Id);

                if (trackingEvent?.SchedulerStateForTennisMatchPolling != SchedulerStateForTennisMatchPolling.NotStarted) continue;

                trackingEvent.SchedulerStateForTennisMatchPolling = SchedulerStateForTennisMatchPolling.LivePolling;
                _recurringJobManager.Trigger(liveJobName);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private bool IsUpcoming(DateTimeOffset startDate)
        {
            var now = DateTimeOffset.UtcNow;

            var maxTime = now + TimeSpan.FromMinutes(
                _preLivePollingDurationInMinutes);

            return startDate > now && startDate < maxTime;
        }
    }
}
