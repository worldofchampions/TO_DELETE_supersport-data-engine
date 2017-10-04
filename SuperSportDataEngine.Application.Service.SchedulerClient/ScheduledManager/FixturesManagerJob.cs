namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Microsoft.Practices.Unity;
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
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournaments;
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        IUnityContainer _container;

        public FixturesManagerJob(
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournaments,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository,
            IUnityContainer container)
        {
            _schedulerTrackingRugbyTournaments = schedulerTrackingRugbyTournaments;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _container = container;
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
                    await _container.Resolve<IRugbyService>().GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobCronExpression"];

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _container.Resolve<IRugbyIngestWorkerService>().IngestOneMonthsFixturesForTournament(CancellationToken.None, tournament.ProviderTournamentId),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    var tournamentInDb =
                            (await _schedulerTrackingRugbyTournaments.AllAsync())
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
                await _container.Resolve<IRugbyService>().GetEndedTournaments();

            var inactiveTournaments =
                await _container.Resolve<IRugbyService>().GetInactiveTournaments();

            await DeleteJobsForFetchingFixturesForTournaments(endedTournaments);
            await DeleteJobsForFetchingFixturesForTournaments(inactiveTournaments);
            await StopSchedulingInactiveTournaments(inactiveTournaments);
        }

        private async Task<int> StopSchedulingInactiveTournaments(IEnumerable<RugbyTournament> inactiveTournaments)
        {
            foreach (var tournament in inactiveTournaments)
            {
                var seasons = (await _schedulerTrackingRugbyTournaments.AllAsync()).Where(t => t.TournamentId == tournament.Id);
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
                var activeTournamentJobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                var currentTournamentJobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                RecurringJob.RemoveIfExists(activeTournamentJobId);
                RecurringJob.RemoveIfExists(currentTournamentJobId);

                var tournamentInDb =
                    (await _schedulerTrackingRugbyTournaments.AllAsync())
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
                await _container.Resolve<IRugbyService>().GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                if ((await _container.Resolve<IRugbyService>().GetSchedulerStateForManagerJobPolling(tournament.Id)) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobCronExpression"];

                    var seasonId = await _container.Resolve<IRugbyService>().GetCurrentProviderSeasonIdForTournament(CancellationToken.None, tournament.Id);

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _container.Resolve<IRugbyIngestWorkerService>().IngestFixturesForTournamentSeason(CancellationToken.None, tournament.ProviderTournamentId, seasonId),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    var season =
                        (await _schedulerTrackingRugbySeasonRepository.AllAsync())
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
