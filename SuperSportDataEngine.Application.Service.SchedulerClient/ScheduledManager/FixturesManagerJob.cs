namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FixturesManagerJob
    {
        IRugbyService _rugbyService;
        IRugbyIngestWorkerService _rugbyIngestWorkerService;
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtures;
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournaments;
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;

        public FixturesManagerJob(
            IRugbyService rugbyService,
            IRugbyIngestWorkerService rugbyIngestWorkerService,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugyFixtures,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournaments,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository)
        {
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;
            _schedulerTrackingRugbyFixtures = schedulerTrackingRugyFixtures;
            _schedulerTrackingRugbyTournaments = schedulerTrackingRugbyTournaments;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments();
            await CreateChildJobsForFetchingFixturesForTournamentSeason();
            await DeleteChildJobsForInactiveAndEndedTournaments();
        }

        private async Task<int> CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments()
        {
            var activeTournaments =
                    _rugbyService
                    .GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobCronExpression"];

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _rugbyIngestWorkerService.IngestOneMonthsFixturesForTournament(CancellationToken.None, tournament.ProviderTournamentId),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    var tournamentInDb =
                            _schedulerTrackingRugbyTournaments
                                .Where(
                                    t =>
                                        t.TournamentId == tournament.Id &&
                                        t.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning)
                                .FirstOrDefault();

                    if (tournamentInDb != null)
                    {
                        tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                    }
                }
            }

            return await _schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task DeleteChildJobsForInactiveAndEndedTournaments()
        {
            var endedTournaments =
                _rugbyService.GetEndedTournaments();

            var inactiveTournaments =
                _rugbyService.GetInactiveTournaments();

            await DeleteJobsForFetchingFixturesForTournaments(endedTournaments);
            await DeleteJobsForFetchingFixturesForTournaments(inactiveTournaments);
            await StopSchedulingInactiveTournaments(inactiveTournaments);
        }

        private async Task<int> StopSchedulingInactiveTournaments(IEnumerable<RugbyTournament> inactiveTournaments)
        {
            foreach (var tournament in inactiveTournaments)
            {
                var seasons = _schedulerTrackingRugbyTournaments.Where(t => t.TournamentId == tournament.Id);
                foreach (var tournamentSeason in seasons)
                {
                    tournamentSeason.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                }
            }

            return await _schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task<int> DeleteJobsForFetchingFixturesForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
            foreach (var tournament in tournaments)
            {
                var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_JobIdPrefix"] + tournament.Name;
                RecurringJob.RemoveIfExists(jobId);

                var tournamentInDb =
                    _schedulerTrackingRugbyTournaments
                        .Where(
                            t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                        .FirstOrDefault();

                if (tournamentInDb != null)
                {
                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                }
            }

            return await _schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingFixturesForTournamentSeason()
        {
            var currentTournaments =
                _rugbyService
                .GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                if (_rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobCronExpression"];

                    var seasonId = _rugbyService.GetCurrentProviderSeasonIdForTournament(tournament.Id);

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _rugbyIngestWorkerService.IngestFixturesForTournamentSeason(CancellationToken.None, tournament.ProviderTournamentId, seasonId),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    var season =
                        _schedulerTrackingRugbySeasonRepository
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
    }
}
