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
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager.Tennis
{
    public class TennisRankingsManagerJob
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ITennisIngestWorkerService _tennisIngestWorkerService;

        public TennisRankingsManagerJob(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            IRecurringJobManager recurringJobManager,
            ITennisIngestWorkerService tennisIngestWorkerService)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _recurringJobManager = recurringJobManager;
            _tennisIngestWorkerService = tennisIngestWorkerService;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobFetchingLeagueRankings();
            await DeleteChildJobFetchingLeagueRankings();
        }

        private async Task DeleteChildJobFetchingLeagueRankings()
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
                    var rankingsJobName = ConfigurationManager.AppSettings["TennisChildJob_Rankings_JobIdPrefix"] + league.Abbreviation;
                    _recurringJobManager.RemoveIfExists(rankingsJobName);

                    var raceRankingsJobName = ConfigurationManager.AppSettings["TennisChildJob_RaceRankings_JobIdPrefix"] + league.Abbreviation;
                    _recurringJobManager.RemoveIfExists(raceRankingsJobName);
                }
            }
        }

        private async Task CreateChildJobFetchingLeagueRankings()
        {
            var events =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.AllAsync()).ToList();

            var currentEvents = events.Where(IsEventCurrent).ToList();

            foreach (var schedulerTrackingTennisEvent in currentEvents)
            {
                var tennisEvent = _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.Id == schedulerTrackingTennisEvent.TennisEventId);

                var leagues = tennisEvent.TennisTournament.TennisLeagues;

                foreach (var league in leagues)
                {
                    var rankingsJobName = ConfigurationManager.AppSettings["TennisChildJob_Rankings_JobIdPrefix"] + league.Abbreviation;
                    var rankingsJobExpression = ConfigurationManager.AppSettings["TennisChildJob_Rankings_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        rankingsJobName,
                        Job.FromExpression(() => _tennisIngestWorkerService.IngestRankingsForLeague(league.ProviderSlug, CancellationToken.None)),
                        rankingsJobExpression,
                        new RecurringJobOptions
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.NormalPriority
                        });

                    var raceRankingsJobName = ConfigurationManager.AppSettings["TennisChildJob_RaceRankings_JobIdPrefix"] + league.Abbreviation;
                    var raceRankingsJobExpression = ConfigurationManager.AppSettings["TennisChildJob_RaceRankings_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        raceRankingsJobName,
                        Job.FromExpression(() => _tennisIngestWorkerService.IngestRaceRankingsForLeague(league.ProviderSlug, CancellationToken.None)),
                        raceRankingsJobExpression,
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
