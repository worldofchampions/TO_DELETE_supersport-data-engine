using SuperSportDataEngine.Application.Container.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

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
        private static int _maxNumberOfHoursToCheckForUpcomingFixtures;
        private static int _maxNumberOfHoursToCheckForPreviousFixtures;

        public FixturesManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer)
        {
            _recurringJobManager = recurringJobManager;
            _childContainer = childContainer.CreateChildContainer();

            _maxNumberOfHoursToCheckForUpcomingFixtures =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfHoursToCheckForUpcomingFixtures"]);

            _maxNumberOfHoursToCheckForPreviousFixtures =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfHoursToCheckForPreviousFixtures"]);
        }

        public async Task DoWorkAsync()
        {
            CreateNewContainer();
            ConfigureDependencies();

            await CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments();
            await CreateAndDeleteChildJobsForFetchingFixturesForTournamentSeason();
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
            using (var unitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>())
            {
                var activeTournaments =
                    await _childContainer.Resolve<IRugbyService>().GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                    var jobId = ConfigurationManager.AppSettings[
                                    "ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] +
                                tournament.Name;
                    var jobCronExpression =
                        ConfigurationManager.AppSettings[
                            "ScheduleManagerJob_Fixtures_ActiveTournaments_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() =>
                            _childContainer.Resolve<IRugbyIngestWorkerService>()
                                .IngestOneMonthsFixturesForTournament(CancellationToken.None,
                                    tournament.ProviderTournamentId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var tournamentInDb =
                        (await unitOfWork.SchedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id &&
                                             t.SchedulerStateForManagerJobPolling ==
                                             SchedulerStateForManagerJobPolling.NotRunning);

                    if (tournamentInDb == null) continue;

                    tournamentInDb.SchedulerStateForManagerJobPolling =
                        SchedulerStateForManagerJobPolling.Running;
                    unitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }

                return await unitOfWork.SaveChangesAsync();
            }

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
            using (var unitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>())
            {
                foreach (var tournament in inactiveTournaments)
                {
                    var seasons =
                        (await unitOfWork.SchedulerTrackingRugbyTournaments.AllAsync()).Where(t =>
                            t.TournamentId == tournament.Id);
                    foreach (var tournamentSeason in seasons)
                    {
                        tournamentSeason.SchedulerStateForManagerJobPolling =
                            SchedulerStateForManagerJobPolling.NotRunning;
                        unitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentSeason);
                    }
                }

                return await unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<int> DeleteJobsForFetchingFixturesForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
            using (var unitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>())
            {
                foreach (var tournament in tournaments)
                {
                    var activeTournamentJobId =
                        ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] +
                        tournament.Name;
                    var currentTournamentJobId =
                        ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] +
                        tournament.Name;

                    _recurringJobManager.RemoveIfExists(activeTournamentJobId);
                    _recurringJobManager.RemoveIfExists(currentTournamentJobId);

                    var tournamentInDb =
                        (await unitOfWork.SchedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling ==
                                             SchedulerStateForManagerJobPolling.Running);

                    if (tournamentInDb == null) continue;

                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    unitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }

                return await unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<int> CreateAndDeleteChildJobsForFetchingFixturesForTournamentSeason()
        {
            var maxTimeForCheckingUpcomingFixtures = DateTime.UtcNow + TimeSpan.FromHours(_maxNumberOfHoursToCheckForUpcomingFixtures);
            var minTimeForCheckingPreviousFixtures = DateTime.UtcNow - TimeSpan.FromHours(_maxNumberOfHoursToCheckForPreviousFixtures);

            var currentTournaments =
                (await _childContainer.Resolve<IRugbyService>().GetCurrentDayFixturesForActiveTournaments())
                    .Where(f => f.StartDateTime < maxTimeForCheckingUpcomingFixtures && 
                                f.StartDateTime > minTimeForCheckingPreviousFixtures)
                    .Select(f => f.RugbyTournament)
                    .ToList();

            var currentTournamentIds = currentTournaments.Select(t => t.ProviderTournamentId);

            var nonCurrentTournaments = (await _childContainer.Resolve<IRugbyService>().GetCurrentTournaments())
                .Where(t => !currentTournamentIds.Contains(t.ProviderTournamentId));

            using (var unitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>())
            {
                foreach (var tournament in nonCurrentTournaments)
                {
                    var activeTournamentJobId =
                        ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] +
                        tournament.Name;
                    var currentTournamentJobId =
                        ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] +
                        tournament.Name;

                    _recurringJobManager.RemoveIfExists(activeTournamentJobId);
                    _recurringJobManager.RemoveIfExists(currentTournamentJobId);

                    var tournamentInDb =
                        (await unitOfWork.SchedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling ==
                                             SchedulerStateForManagerJobPolling.Running);

                    if (tournamentInDb == null) continue;

                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    unitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }

                await unitOfWork.SaveChangesAsync();

                foreach (var tournament in currentTournaments)
                {
                    if ((await _childContainer.Resolve<IRugbyService>()
                            .GetSchedulerStateForManagerJobPolling(tournament.Id)) ==
                        SchedulerStateForManagerJobPolling.NotRunning)
                    {
                        if ((await _childContainer.Resolve<IRugbyService>()
                                .GetSchedulerStateForManagerJobPolling(tournament.Id)) ==
                            SchedulerStateForManagerJobPolling.NotRunning)
                        {
                            var jobId = ConfigurationManager.AppSettings[
                                            "ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] +
                                        tournament.Name;
                            var jobCronExpression =
                                ConfigurationManager.AppSettings[
                                    "ScheduleManagerJob_Fixtures_CurrentTournaments_JobCronExpression"];

                            var seasonId = await _childContainer.Resolve<IRugbyService>()
                                .GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                            _recurringJobManager.AddOrUpdate(
                                jobId,
                                Job.FromExpression(() =>
                                    _childContainer.Resolve<IRugbyIngestWorkerService>()
                                        .IngestFixturesForTournamentSeason(CancellationToken.None,
                                            tournament.ProviderTournamentId, seasonId)),
                                jobCronExpression,
                                new RecurringJobOptions()
                                {
                                    TimeZone = TimeZoneInfo.Local,
                                    QueueName = HangfireQueueConfiguration.HighPriority
                                });

                            var season =
                                (await unitOfWork.SchedulerTrackingRugbySeasons.AllAsync())
                                .FirstOrDefault(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                                                     s.TournamentId == tournament.Id &&
                                                     s.SchedulerStateForManagerJobPolling ==
                                                     SchedulerStateForManagerJobPolling.NotRunning);

                            if (season != null)
                            {
                                season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                                unitOfWork.SchedulerTrackingRugbySeasons.Update(season);
                            }
                        }
                    }
                }

                return await unitOfWork.SaveChangesAsync();
            }
        }
    }
}