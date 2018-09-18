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
    public class TennisCalendarManagerJob
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ITennisIngestWorkerService _ingestWorkerService;

        public TennisCalendarManagerJob(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork, 
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork, 
            IRecurringJobManager recurringJobManager, 
            ITennisIngestWorkerService ingestWorkerService)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _recurringJobManager = recurringJobManager;
            _ingestWorkerService = ingestWorkerService;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingCalendarEvents();
            await DeleteChildJobsForFetchingCalendarEvents();
        }

        private async Task DeleteChildJobsForFetchingCalendarEvents()
        {
            // Get all the enabled leagues
            var enabledLeagues =
                _publicSportDataUnitOfWork.TennisLeagues.Where(
                    l => !l.IsDisabledInbound);

            // Get the active events
            var events =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.AllAsync()).ToList();

            var currentEvents = events.Where(IsEventCurrent).ToList();

            // For each of the leagues, check if there are any active events for that league
            // If not, delete the job that polls for the calendar for the league.
            foreach (var enabledLeague in enabledLeagues)
            {
                var eventIds = currentEvents.Select(e => e.TennisEventId).ToList();
                var tennisEvents =
                    _publicSportDataUnitOfWork.TennisEvents.Where(
                        e => eventIds.Contains(e.Id)).ToList();

                var leagueHasActiveEvent =
                    tennisEvents.Any(
                        e => e.TennisTournament.TennisLeagues.Contains(enabledLeague));

                if (leagueHasActiveEvent) continue;

                var calendarJobName = ConfigurationManager.AppSettings["TennisChildJob_Calendar_JobIdPrefix"] + enabledLeague.Abbreviation;
                _recurringJobManager.RemoveIfExists(calendarJobName);
            }
        }

        private async Task CreateChildJobsForFetchingCalendarEvents()
        {
            var events =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.AllAsync()).ToList();

            var currentEvents = events.Where(IsEventCurrent).ToList();

            foreach (var schedulerTrackingTennisEvent in currentEvents)
            {
                var leagues = _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.Id == schedulerTrackingTennisEvent.TennisEventId).TennisTournament.TennisLeagues;

                foreach (var league in leagues)
                {
                    var calendarJobName = ConfigurationManager.AppSettings["TennisChildJob_Calendar_JobIdPrefix"] + league.Abbreviation;
                    var calendarJobExpression = ConfigurationManager.AppSettings["TennisChildJob_Calendar_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        calendarJobName,
                        Job.FromExpression(() => _ingestWorkerService.IngestCalendarForLeague(league.ProviderLeagueId, CancellationToken.None)),
                        calendarJobExpression,
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
    }
}
