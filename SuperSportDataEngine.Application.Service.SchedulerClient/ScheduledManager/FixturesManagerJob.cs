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
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FixturesManagerJob
    {
        IUnityContainer _childContainer;
        IRecurringJobManager _recurringJobManager;

        public FixturesManagerJob(
            IRecurringJobManager recurringJobManager,
            IUnityContainer childContainer)
        {
            _recurringJobManager = recurringJobManager;
            _childContainer = childContainer;
        }

        public async Task DoWorkAsync()
        {
            CreateNewContainer();

            await CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments();
            await CreateChildJobsForFetchingFixturesForTournamentSeason();
            await DeleteChildJobsForInactiveAndEndedTournaments();
        }

        private void CreateNewContainer()
        {
            _childContainer = _childContainer.CreateChildContainer();
            UnityConfigurationManager.RegisterTypes(_childContainer, Container.Enums.ApplicationScope.ServiceSchedulerClient);
        }

        private async Task<int> CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments()
        {
            var _schedulerTrackingRugbyTournaments =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();

            var activeTournaments =
                    await _childContainer.Resolve<IRugbyService>().GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() => _childContainer.Resolve<IRugbyIngestWorkerService>().IngestOneMonthsFixturesForTournament(CancellationToken.None, tournament.ProviderTournamentId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Utc,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

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
                        _schedulerTrackingRugbyTournaments.Update(tournamentInDb);
                    }
                }
            }

            return await _schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task DeleteChildJobsForInactiveAndEndedTournaments()
        {
            var endedTournaments =
                await _childContainer.Resolve<IRugbyService>().GetEndedTournaments();

            var inactiveTournaments =
                await _childContainer.Resolve<IRugbyService>().GetInactiveTournaments();

            await DeleteJobsForFetchingFixturesForTournaments(endedTournaments);
            await DeleteJobsForFetchingFixturesForTournaments(inactiveTournaments);
            await StopSchedulingInactiveTournaments(inactiveTournaments);
        }

        private async Task<int> StopSchedulingInactiveTournaments(IEnumerable<RugbyTournament> inactiveTournaments)
        {
            var _schedulerTrackingRugbyTournaments =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();

            foreach (var tournament in inactiveTournaments)
            {
                var seasons = (await _schedulerTrackingRugbyTournaments.AllAsync()).Where(t => t.TournamentId == tournament.Id);
                foreach (var tournamentSeason in seasons)
                {
                    tournamentSeason.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    _schedulerTrackingRugbyTournaments.Update(tournamentSeason);
                }
            }

            return await _schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task<int> DeleteJobsForFetchingFixturesForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
            var _schedulerTrackingRugbyTournaments =
                _childContainer.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();

            foreach (var tournament in tournaments)
            {
                var activeTournamentJobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                var currentTournamentJobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                _recurringJobManager.RemoveIfExists(activeTournamentJobId);
                _recurringJobManager.RemoveIfExists(currentTournamentJobId);

                var tournamentInDb =
                    (await _schedulerTrackingRugbyTournaments.AllAsync())
                        .Where(
                            t => t.TournamentId == tournament.Id && t.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                        .FirstOrDefault();

                if (tournamentInDb != null)
                {
                    tournamentInDb.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                    _schedulerTrackingRugbyTournaments.Update(tournamentInDb);
                }
            }

            return await _schedulerTrackingRugbyTournaments.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingFixturesForTournamentSeason()
        {
            var _schedulerTrackingRugbySeasonRepository =
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
                            TimeZone = TimeZoneInfo.Utc,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

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
                        _schedulerTrackingRugbySeasonRepository.Update(season);
                    }
                }
            }

            return await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }
    }
}
