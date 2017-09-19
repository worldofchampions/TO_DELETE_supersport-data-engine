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
            await CreateChildJobsForPollingLogs();

            base.ResetTimer();
        }

        private async Task CreateChildJobsForPollingLogs()
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

                    QueueJobForLowFrequencyPolling(tournament.Id, tournament.ProviderTournamentId, seasonId, jobId);

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

        private void QueueJobForLowFrequencyPolling(Guid tournamentId, int providerTournamentId, int seasonId, string jobId)
        {
            string highFreqExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_HighFrequencyPolling_ExpiryInMinutes"];

            int udpateJobFrequencyOnThisMinute = int.Parse(highFreqExpiryFromConfig);

            var timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(udpateJobFrequencyOnThisMinute).TotalMilliseconds
            };

            timer.Elapsed += delegate
            {
                string jobExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_LowFrequencyPolling_ExpiryInMinutes"];

                var deleteJobOnThisMinute = int.Parse(jobExpiryFromConfig);

                var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_LowFrequencyPolling_CronExpression"];

                AddOrUpdateHangfireJob(providerTournamentId, seasonId, jobId, jobCronExpression);

                LogsJobCleanupManager.QueueJobForDeletion(tournamentId, jobId, deleteJobOnThisMinute);

                timer.Stop();
            };

            timer.Start();
        }
    }
}