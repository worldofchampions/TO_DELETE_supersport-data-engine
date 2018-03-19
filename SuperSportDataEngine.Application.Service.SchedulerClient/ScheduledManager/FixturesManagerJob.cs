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
        private static int _maxNumberOfHoursToCheckForUpcomingFixtures;
        private static int _maxNumberOfHoursToCheckForPreviousFixtures;

        private ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

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
            _systemSportDataUnitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>();
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
                        (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id &&
                                             t.SchedulerStateForManagerJobPolling ==
                                             SchedulerStateForManagerJobPolling.NotRunning);

                    if (tournamentInDb == null) continue;

                    tournamentInDb.SchedulerStateForManagerJobPolling =
                        SchedulerStateForManagerJobPolling.Running;
                    _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }

                return await _systemSportDataUnitOfWork.SaveChangesAsync();
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
            {
                foreach (var tournament in inactiveTournaments)
                {
                    var seasons =
                        (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.AllAsync()).Where(t =>
                            t.TournamentId == tournament.Id);
                    foreach (var tournamentSeason in seasons)
                    {
                        tournamentSeason.SchedulerStateForManagerJobPolling =
                            SchedulerStateForManagerJobPolling.NotRunning;
                        _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentSeason);
                    }
                }

                return await _systemSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task<int> DeleteJobsForFetchingFixturesForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
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
                        (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling ==
                                             SchedulerStateForManagerJobPolling.Running);

                    if (tournamentInDb == null) continue;

                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }

                return await _systemSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task CreateAndDeleteChildJobsForFetchingFixturesForTournamentSeason()
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
                        (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.AllAsync())
                        .FirstOrDefault(t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling ==
                                             SchedulerStateForManagerJobPolling.Running);

                    if (tournamentInDb == null) continue;

                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }

                await _systemSportDataUnitOfWork.SaveChangesAsync();

                foreach (var tournament in currentTournaments)
                {
                    var seasonId = await _childContainer.Resolve<IRugbyService>()
                        .GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                    var jobId = ConfigurationManager.AppSettings[
                                    "ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression =
                        ConfigurationManager.AppSettings[
                            "ScheduleManagerJob_Fixtures_CurrentTournaments_JobCronExpression"];

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
                        (await _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.AllAsync())
                        .FirstOrDefault(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                                             s.TournamentId == tournament.Id);

                    if (season != null)
                    {
                        season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                        _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Update(season);
                    }

                }

                await _systemSportDataUnitOfWork.SaveChangesAsync();
            }

        }
    }
}