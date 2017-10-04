namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LogsManagerJob
    {
        IRugbyService _rugbyService;
        IRugbyIngestWorkerService _rugbyIngestService;
        IRecurringJobManager _recurringJobManager;
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        public LogsManagerJob(
            IRugbyService rugbyService,
            IRugbyIngestWorkerService rugbyIngestService,
            IRecurringJobManager recurringJobManager,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository)
        {
            _rugbyService = rugbyService;
            _rugbyIngestService = rugbyIngestService;
            _recurringJobManager = recurringJobManager;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLogs();
        }

        private async Task<int> CreateChildJobsForFetchingLogs()
        {
            var activeTournaments = await _rugbyService.GetActiveTournamentsForMatchesInResultsState();

            foreach (var tournament in activeTournaments)
            {
                int seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                if (await _rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                    AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, jobCronExpression);

                    QueueJobForLowFrequencyPolling(tournament.Id, tournament.ProviderTournamentId, seasonId, jobId);

                    var season =
                            _schedulerTrackingRugbySeasonRepository.All()
                                .Where(
                                    s =>
                                        s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                                        s.TournamentId == tournament.Id &&
                                        s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning)
                                .FirstOrDefault();
                    if (season != null)
                    {
                        season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                    }
                }
            }

            return await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }

        private void AddOrUpdateHangfireJob(int providerTournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => _rugbyIngestService.IngestLogsForTournamentSeason(CancellationToken.None, providerTournamentId, seasonId)),
                jobCronExpression,
                TimeZoneInfo.Utc,
                HangfireQueueConfiguration.HighPriority);
        }

        private void QueueJobForLowFrequencyPolling(Guid tournamentId, int providerTournamentId, int seasonId, string jobId)
        {
            string highFreqExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_HighFrequencyPolling_ExpiryInMinutes"];

            int udpateJobFrequencyOnThisMinute = int.Parse(highFreqExpiryFromConfig);

            var timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(udpateJobFrequencyOnThisMinute).TotalMilliseconds
            };

            timer.Elapsed += delegate
            {
                var jobExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_LowFrequencyPolling_ExpiryInMinutes"];
                var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_LowFrequencyPolling_CronExpression"];

                var deleteJobOnThisMinute = int.Parse(jobExpiryFromConfig);

                AddOrUpdateHangfireJob(providerTournamentId, seasonId, jobId, jobCronExpression);

                LogsJobCleanupManager.QueueJobForDeletion(tournamentId, jobId, deleteJobOnThisMinute);

                timer.Stop();
            };

            timer.Start();
        }
    }
}
