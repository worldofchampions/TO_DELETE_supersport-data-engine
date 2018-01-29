using SuperSportDataEngine.Application.Container.Enums;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LogsManagerJob
    {
        IRecurringJobManager _recurringJobManager;
        IUnityContainer _childContainer;
        ILoggingService _logger;

        public LogsManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer,
            ILoggingService logger)
        {
            _recurringJobManager = recurringJobManager;
            _childContainer = childContainer;
            _logger = logger;
        }

        public async Task DoWorkAsync()
        {
            CreateContainer();
            ConfigureDependencies();

            await CreateChildJobsForFetchingLogs();
        }

        private void ConfigureDependencies()
        {
            _recurringJobManager = _childContainer.Resolve<IRecurringJobManager>();
            _logger = _childContainer.Resolve<ILoggingService>();
        }

        private void CreateContainer()
        {
            _childContainer?.Dispose();

            _childContainer = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(_childContainer, Container.Enums.ApplicationScope.ServiceSchedulerClient);
            UnityConfigurationManager.RegisterApiGlobalTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
        }

        public async Task<int> CreateChildJobsForFetchingLogs()
        {
            var _schedulerTrackingRugbySeasonRepository =
                            _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>();

            var activeTournaments = await _childContainer.Resolve<IRugbyService>().GetActiveTournamentsForMatchesInResultsState();

            foreach (var tournament in activeTournaments)
            {
                int seasonId = await _childContainer.Resolve<IRugbyService>().GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                if (await _childContainer.Resolve<IRugbyService>().GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                    AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, jobCronExpression);

                    QueueJobForLowFrequencyPolling(tournament.Id, tournament.ProviderTournamentId, seasonId, jobId);

                    var season =
                            (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                                .FirstOrDefault(s => 
                                        s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                                        s.TournamentId == tournament.Id &&
                                        s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning);

                    if (season != null)
                    {
                        season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                        _schedulerTrackingRugbySeasonRepository.Update(season);
                    }
                }
            }

            return await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }

        private void AddOrUpdateHangfireJob(int providerTournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => (_childContainer.Resolve<IRugbyIngestWorkerService>()).IngestLogsForTournamentSeason(CancellationToken.None, providerTournamentId, seasonId)),
                jobCronExpression,
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
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
