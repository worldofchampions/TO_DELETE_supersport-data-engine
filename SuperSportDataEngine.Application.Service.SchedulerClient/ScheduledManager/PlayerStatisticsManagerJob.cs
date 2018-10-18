using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Helpers;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    public class PlayerStatisticsManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private static int _maxNumberOfRecentFixturesToConsider;
        private readonly IRugbyService _rugbyService;
        private readonly IRugbyIngestWorkerService _rugbyIngestWorkerService;

        public PlayerStatisticsManagerJob(
            IRecurringJobManager recurringJobManager,
            IRugbyService rugbyService, 
            IRugbyIngestWorkerService rugbyIngestWorkerService)
        {
            _recurringJobManager = recurringJobManager;
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;

            _maxNumberOfRecentFixturesToConsider =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfRecentFixturesToConsider"]);
        }

        public async Task DoWorkAsync()
        {
            await CreateAndDeleteChildJobsForFetchingPlayerStatistics();
        }

        private async Task CreateAndDeleteChildJobsForFetchingPlayerStatistics()
        {
            var today = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;

            var todayTournaments =
                (await _rugbyService.GetRecentResultsFixtures(_maxNumberOfRecentFixturesToConsider))
                .Where(f => f.StartDateTime.Date == today)
                .Select(f => f.RugbyTournament)
                .ToList();

            var todayTournamentIds = todayTournaments.Select(t => t.ProviderTournamentId);

            var notTodayTournaments = (await _rugbyService.GetCurrentTournaments())
                .Where(t => todayTournamentIds.Contains(t.ProviderTournamentId));

            foreach (var tournament in notTodayTournaments)
            {
                var jobId =
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                _recurringJobManager.RemoveIfExists(jobId);
            }

            foreach (var tournament in todayTournaments)
            {
                var seasonId = 
                    await _rugbyService.GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                var jobId = 
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                var cronExpression =
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobCronExpression"];

                AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, cronExpression);
            }
        }

        private void AddOrUpdateHangfireJob(int tournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => _rugbyIngestWorkerService.IngestPlayerStatsForCurrentTournaments(tournamentId, seasonId ,CancellationToken.None)),
                jobCronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimezoneHelper.LocalSAST,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }
    }
}
