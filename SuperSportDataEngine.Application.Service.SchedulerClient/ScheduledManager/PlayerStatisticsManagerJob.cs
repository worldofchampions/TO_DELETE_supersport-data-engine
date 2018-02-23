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
        private static int _maxNumberOfRecentFixturesToConsider;

        public PlayerStatisticsManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer)
        {
            _recurringJobManager = recurringJobManager;
            _childContainer = childContainer;
            _maxNumberOfRecentFixturesToConsider =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfRecentFixturesToConsider"]);
        }

        public async Task DoWorkAsync()
        {
            CreateContainer();
            ConfigureDependencies();

            await CreateAndDeleteChildJobsForFetchingPlayerStatistics();
        }

        private void ConfigureDependencies()
        {
            _recurringJobManager = _childContainer.Resolve<IRecurringJobManager>();
        }

        private void CreateContainer()
        {
            _childContainer?.Dispose();

            _childContainer = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
            UnityConfigurationManager.RegisterApiGlobalTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
        }

        private async Task CreateAndDeleteChildJobsForFetchingPlayerStatistics()
        {
            var today = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;

            var todayTournaments =
                (await _childContainer.Resolve<IRugbyService>().GetRecentResultsFixtures(_maxNumberOfRecentFixturesToConsider))
                .Where(f => f.StartDateTime.Date == today)
                .Select(f => f.RugbyTournament)
                .ToList();

            var todayTournamentIds = todayTournaments.Select(t => t.ProviderTournamentId);

            var notTodayTournaments = (await _childContainer.Resolve<IRugbyService>().GetCurrentTournaments())
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
                    await _childContainer.Resolve<IRugbyService>().GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                var jobId = 
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                var cronExpression =
                    ConfigurationManager.AppSettings["ScheduleManagerJob_PlayerStats_CurrentTournaments_JobCronExpression"];

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
