using SuperSportDataEngine.Application.Container.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
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

    public class FixturesManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private static int _maxNumberOfHoursToCheckForUpcomingFixtures;
        private static int _maxNumberOfHoursToCheckForPreviousFixtures;

        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly IRugbyService _rugbyService;
        private readonly IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private readonly ILoggingService _loggingService;

        public FixturesManagerJob(
            IRecurringJobManager recurringJobManager,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork, 
            IRugbyService rugbyService, 
            IRugbyIngestWorkerService rugbyIngestWorkerService,
            ILoggingService loggingService)
        {
            _recurringJobManager = recurringJobManager;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;
            _loggingService = loggingService;

            _maxNumberOfHoursToCheckForUpcomingFixtures =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfHoursToCheckForUpcomingFixtures"]);

            _maxNumberOfHoursToCheckForPreviousFixtures =
                int.Parse(ConfigurationManager.AppSettings["MaxNumberOfHoursToCheckForPreviousFixtures"]);
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments();
            await CreateAndDeleteChildJobsForFetchingFixturesForTournamentSeason();
            await DeleteChildJobsForInactiveAndEndedTournaments();
        }

        private async Task<int> CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments()
        {
            {
                var activeTournaments =
                    await _rugbyService.GetActiveTournaments();

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
                            _rugbyIngestWorkerService
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
                await _rugbyService.GetInactiveTournaments();

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
                (await _rugbyService.GetCurrentDayFixturesForActiveTournaments())
                    .Where(f => f.StartDateTime < maxTimeForCheckingUpcomingFixtures &&
                                f.StartDateTime > minTimeForCheckingPreviousFixtures)
                    .Select(f => f.RugbyTournament)
                    .ToList();

            var currentTournamentIds = currentTournaments.Select(t => t.ProviderTournamentId);

            var nonCurrentTournaments = (await _rugbyService.GetCurrentTournaments())
                .Where(t => !currentTournamentIds.Contains(t.ProviderTournamentId));

            {
                foreach (var tournament in nonCurrentTournaments)
                {
                    var currentTournamentJobId =
                        ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] +
                        tournament.Name;

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
                    var seasonId = await _rugbyService
                        .GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                    var jobId = ConfigurationManager.AppSettings[
                                    "ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression =
                        ConfigurationManager.AppSettings[
                            "ScheduleManagerJob_Fixtures_CurrentTournaments_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() =>
                            _rugbyIngestWorkerService
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