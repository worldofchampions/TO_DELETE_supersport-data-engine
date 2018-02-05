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
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FixturesManagerJob
    {
        private IUnityContainer _childContainer;
        private IRecurringJobManager _recurringJobManager;
        private ILoggingService _logger;
        private IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonsRepository;

        public FixturesManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer,
            ILoggingService logger)
        {
            _recurringJobManager = recurringJobManager;
            _childContainer = childContainer.CreateChildContainer();
            _logger = logger;
        }

        public async Task DoWorkAsync()
        {
            CreateNewContainer();
            ConfigureDependencies();

            await CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments();
            await CreateChildJobsForFetchingFixturesForTournamentSeason();
            await DeleteChildJobsForInactiveAndEndedTournaments();
        }

        private void ConfigureDependencies()
        {
            _logger = _childContainer.Resolve<ILoggingService>();
            _recurringJobManager = _childContainer.Resolve<IRecurringJobManager>();
            _rugbySeasonsRepository = _childContainer.Resolve<IBaseEntityFrameworkRepository<RugbySeason>>();
        }

        private void CreateNewContainer()
        {
            _childContainer?.Dispose();

            _childContainer = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
            UnityConfigurationManager.RegisterApiGlobalTypes(_childContainer, ApplicationScope.ServiceSchedulerClient);
        }

        private async Task<int> CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments()
        {
            var schedulerTrackingRugbyTournaments =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();

            var activeTournaments =
                    await _childContainer.Resolve<IRugbyService>().GetActiveTournaments();

            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        
            foreach (var tournament in activeTournaments)
            {
                await _logger.Debug(methodName, "Tournament " + tournament.Name + " is active.");

                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobCronExpression"];

                    await _logger.Debug(methodName, "Updating recurring job " + jobId);

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() => _childContainer.Resolve<IRugbyIngestWorkerService>().IngestOneMonthsFixturesForTournament(CancellationToken.None, tournament.ProviderTournamentId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var tournamentInDb =
                            (await schedulerTrackingRugbyTournaments.AllAsync())
                                .FirstOrDefault(t => t.TournamentId == tournament.Id &&
                                        t.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning);

                    if (tournamentInDb == null) continue;

                    await _logger.Debug(methodName, "Setting SchedulerStateForManagerJobPolling for tournament " + tournament.Name + " to running.");
                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                    schedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }
            }

            return await schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task DeleteChildJobsForInactiveAndEndedTournaments()
        {
            var inactiveTournaments =
                await _childContainer.Resolve<IRugbyService>().GetInactiveTournaments();

            var rugbyTournaments = inactiveTournaments as IList<RugbyTournament> ?? inactiveTournaments.ToList();
            await DeleteJobsForFetchingFixturesForTournaments(rugbyTournaments);
            await StopSchedulingInactiveTournaments(rugbyTournaments);
        }

        private async Task<int> StopSchedulingInactiveTournaments(IEnumerable<RugbyTournament> inactiveTournaments)
        {
            var schedulerTrackingRugbyTournaments =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();

            foreach (var tournament in inactiveTournaments)
            {
                var seasons = (await schedulerTrackingRugbyTournaments.AllAsync()).Where(t => 
                    t.TournamentId == tournament.Id);

                foreach (var tournamentSeason in seasons)
                {
                    tournamentSeason.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    schedulerTrackingRugbyTournaments.Update(tournamentSeason);
                }
            }

            return await schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task<int> DeleteJobsForFetchingFixturesForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
            var schedulerTrackingRugbyTournaments =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();

            foreach (var tournament in tournaments)
            {
                var activeTournamentJobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                var currentTournamentJobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                _recurringJobManager.RemoveIfExists(activeTournamentJobId);
                _recurringJobManager.RemoveIfExists(currentTournamentJobId);

                var tournamentInDb =
                    (await schedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running);

                if (tournamentInDb == null) continue;

                tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                schedulerTrackingRugbyTournaments.Update(tournamentInDb);
            }

            return await schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingFixturesForTournamentSeason()
        {
            var schedulerTrackingRugbySeasonRepository =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>();

            var currentTournaments =
                await _childContainer.Resolve<IRugbyService>().GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                if ((await _childContainer.Resolve<IRugbyService>().GetSchedulerStateForManagerJobPolling(tournament.Id)) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobCronExpression"];

                    var seasonId = await _childContainer.Resolve<IRugbyService>().GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() => _childContainer.Resolve<IRugbyIngestWorkerService>().IngestFixturesForTournamentSeason(CancellationToken.None, tournament.ProviderTournamentId, seasonId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var season =
                        (await schedulerTrackingRugbySeasonRepository.AllAsync())
                            .FirstOrDefault(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                                    s.TournamentId == tournament.Id &&
                                    s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning);

                    if (season != null)
                    {
                        season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                        schedulerTrackingRugbySeasonRepository.Update(season);
                    }
                }
            }

            return await schedulerTrackingRugbySeasonRepository.SaveAsync();
        }
    }
}
