using SuperSportDataEngine.Application.Container.Enums;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Practices.Unity;
    using Container;
    using Common.Hangfire.Configuration;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using ApplicationLogic.Services;
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
            UnityConfigurationManager.RegisterTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
            UnityConfigurationManager.RegisterApiGlobalTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
        }

        private async Task CreateChildJobsForFetchingLogs()
        {
            var today = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;

            var todayTournaments =
                (await _childContainer.Resolve<IRugbyService>().GetRecentResultsFixtures(30))
                .Where(f => f.StartDateTime.Date == today && f.StartDateTime > now - TimeSpan.FromHours(3))
                .Select(f => f.RugbyTournament)
                .ToList();

            var todayTournamentIds = todayTournaments.Select(t => t.ProviderTournamentId);

            var notTodayTournaments = (await _childContainer.Resolve<IRugbyService>().GetCurrentTournaments())
                .Where(t => todayTournamentIds.Contains(t.ProviderTournamentId));

            foreach (var tournament in notTodayTournaments)
            {
                var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                _recurringJobManager.RemoveIfExists(jobId);
            }

            foreach (var tournament in todayTournaments)
            {
                var seasonId = await _childContainer.Resolve<IRugbyService>().GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);
                var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                var cronExpression =
                    ConfigurationManager.AppSettings[
                        "ScheduleManagerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, cronExpression);
            }
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
    }
}
