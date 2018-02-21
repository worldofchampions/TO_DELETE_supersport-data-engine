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

        public PlayerStatisticsManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer)
        {
            _recurringJobManager = recurringJobManager;
            _childContainer = childContainer;
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
            var todayTournaments =
                (await _childContainer.Resolve<IRugbyService>().GetTournamentsForJustEndedFixtures()).ToList();

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
