using SuperSportDataEngine.Application.Container.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Practices.Unity;
    using Container;
    using Common.Hangfire.Configuration;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using ApplicationLogic.Services;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LogsManagerJob
    {
        readonly IRecurringJobManager _recurringJobManager;
        private static int _maxNumberOfRecentFixturesToConsider;
        private static int _maxNumberOfHoursToCheckForResults;
        private readonly IRugbyService _rugbyService;
        private readonly IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public LogsManagerJob(
            IRecurringJobManager recurringJobManager,
            IRugbyService rugbyService, 
            IRugbyIngestWorkerService rugbyIngestWorkerService, 
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork, 
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _recurringJobManager = recurringJobManager;
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;

            _maxNumberOfRecentFixturesToConsider =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfRecentFixturesToConsider"]);

            _maxNumberOfHoursToCheckForResults =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfHoursToCheckForResults"]);
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLogs();
        }

        public async Task CreateChildJobsForFetchingLogs()
        {
            var today = DateTime.UtcNow.Date;
            var minTimeToCheckForResults = DateTime.UtcNow - TimeSpan.FromHours(_maxNumberOfHoursToCheckForResults);

            // We only want to poll logs for tournaments that have 
            // fixtures in result state within the last few hours.
            // This is configurable.
            var todayTournaments =
                (await _rugbyService.GetRecentResultsFixtures(_maxNumberOfRecentFixturesToConsider))
                .Where(f => f.StartDateTime.Date == today)
                .Where(f => f.StartDateTime > minTimeToCheckForResults)
                .Select(f => f.RugbyTournament)
                .ToList();

            var todayTournamentIds = todayTournaments.Select(t => t.ProviderTournamentId).ToList();

            var notTodayTournaments = (await _rugbyService.GetCurrentTournaments())
                .Where(t => !todayTournamentIds.Contains(t.ProviderTournamentId));

            foreach (var tournament in notTodayTournaments)
            {
                var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                _recurringJobManager.RemoveIfExists(jobId);
            }

            foreach (var tournament in todayTournaments)
            {
                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);
                var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                var cronExpression =
                    ConfigurationManager.AppSettings[
                        "ScheduleManagerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, cronExpression);

                var season = _publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(s =>
                    s.RugbyTournament.Id == tournament.Id &&
                    s.ProviderSeasonId == seasonId);

                if (season == null)
                    continue;

                var trackingSeason = _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.FirstOrDefault(s =>
                    s.TournamentId == tournament.Id &&
                    s.SeasonId == season.Id);

                if (trackingSeason == null)
                    continue;

                trackingSeason.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                _systemSportDataUnitOfWork.SaveChanges();
            }
        }

        private void AddOrUpdateHangfireJob(int providerTournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => _rugbyIngestWorkerService.IngestLogsForTournamentSeason(CancellationToken.None, providerTournamentId, seasonId)),
                jobCronExpression,
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }
    }
}
