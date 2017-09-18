using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using System.Timers;
using System;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System.Configuration;
using Hangfire;
using System.Threading;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    public class LogsScheduledManagerJob : AbstractManagerJob
    {
        private readonly IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestService;

        public LogsScheduledManagerJob(IRugbyService rugbyService, IRugbyIngestWorkerService rugbyIngestWorkerService, int sleepTimeInMinutes)
            : base(sleepTimeInMinutes)
        {
            _rugbyIngestService = rugbyIngestWorkerService;
            _rugbyService = rugbyService;
        }

        public override async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            await CreateChildJobsForMatchesInResultsState15MinutePollingAsync();

            await CreateChildJobsForMatchesInResultsStateOneMinutePolling();

            base.ResetTimer();
        }

        private async Task CreateChildJobsForMatchesInResultsStateOneMinutePolling()
        {
            var activeTournaments = _rugbyService.GetActiveTournamentsForMatchesInResultsState();

            foreach (var tournament in activeTournaments)
            {
                int seasonId = _rugbyService.GetSeasonIdForTournament(tournament.Id);

                if (_rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                    AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, jobCronExpression);

                    await _rugbyService.SetSchedulerStatusPollingForTournamentToRunning(tournament.Id);
                }
            }
        }

        private async Task CreateChildJobsForMatchesInResultsState15MinutePollingAsync()
        {
            var activeTournaments = _rugbyService.GetActiveTournamentsForMatchesInResultsState();

            foreach (var tournament in activeTournaments)
            {
                int seasonId = _rugbyService.GetSeasonIdForTournament(tournament.Id);

                if (_rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_JobCronExpression_15Minutes"];

                    AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, jobCronExpression);

                    await _rugbyService.SetSchedulerStatusPollingForTournamentToRunning(tournament.Id);
                }
            }
        }

        private void AddOrUpdateHangfireJob(int providerTournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            RecurringJob.AddOrUpdate(
                jobId,
                () => _rugbyIngestService.IngestLogsForTournamentSeason(CancellationToken.None, providerTournamentId, seasonId),
                jobCronExpression,
                TimeZoneInfo.Utc,
                HangfireQueueConfiguration.HighPriority);
        }
    }
}
