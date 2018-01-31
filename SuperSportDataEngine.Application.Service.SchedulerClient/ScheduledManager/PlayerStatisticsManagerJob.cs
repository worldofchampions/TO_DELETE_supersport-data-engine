using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.Container.Enums;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    public class PlayerStatisticsManagerJob
    {
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _childContainer;
        ILoggingService _logger;

        public PlayerStatisticsManagerJob(
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

            await CreateChildJobsForFetchingPlayerStatistics();
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

        private async Task CreateChildJobsForFetchingPlayerStatistics()
        {
            var today = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;
            const int maxSelectCount = 30;
            const int timeValue = 3;

            var todayTournaments =
                (await _childContainer.Resolve<IRugbyService>().GetRecentResultsFixtures(maxSelectCount))
                .Where(f => f.StartDateTime.Date == today && f.StartDateTime > now - TimeSpan.FromHours(timeValue))
                .Select(f => f.RugbyTournament)
                .ToList();

            foreach (var tournament in todayTournaments)
            {
                var seasonId = 
                    await _childContainer.Resolve<IRugbyService>().GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                var jobId = 
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                var cronExpression =
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobCronExpression_OneMinute"];

                AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, cronExpression);
            }
        }

        private void AddOrUpdateHangfireJob(int tournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            var rugbyService = _childContainer.Resolve<IRugbyIngestWorkerService>();

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => rugbyService.IngestPlayerStatsForCurrentTournaments(tournamentId, seasonId ,CancellationToken.None)),
                jobCronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }
    }
}
