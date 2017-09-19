using Microsoft.Practices.Unity;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using System.Configuration;
using Hangfire;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using System.Threading;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    public class ManagerJob
    {
        private System.Timers.Timer _timer;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestService;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;

        public ManagerJob(UnityContainer container)
        {
            // This timer will run on a separate thread.
            // Every minute a method will be called.
            _timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };

            _timer.Elapsed += new ElapsedEventHandler(UpdateManagerJobs);
            _timer.Start();

            _rugbyService = container.Resolve<IRugbyService>();
            _rugbyIngestService = container.Resolve<IRugbyIngestWorkerService>();
            _schedulerTrackingRugbyFixtureRepository = container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();
            _schedulerTrackingRugbySeasonRepository = container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>();
        }

        private async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Started updating manager jobs - " + DateTime.Now);
            await CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures();
            await DeleteChildJobsForFetchingMatchDataForFixturesEnded();
            await CreateChildJobsForFetchingFixturesForTournamentSeason();
            await CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments();
            await DeleteChildJobsForInactiveAndEndedTournaments();
            await CreateChildJobsForFetchingLogs();
            Console.WriteLine("Completed updating manager jobs - " + DateTime.Now);
            _timer.Start();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesEnded()
        {
            var endedGames =
                    _rugbyService.GetEndedFixtures();

            foreach(var fixture in endedGames)
            {
                var matchName = fixture.HomeTeam.Name + " vs " + fixture.AwayTeam.Name;
                var jobId = ConfigurationManager.AppSettings["LiveMangerJob_LiveMatch_JobIdPrefix"] + matchName;

                RecurringJob.RemoveIfExists(jobId);

                var fixtures =
                        await _schedulerTrackingRugbyFixtureRepository.WhereAsync(
                            f => f.FixtureId == fixture.Id);

                var fixtureInDb = fixtures.FirstOrDefault();

                if (fixtureInDb != null)
                {
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PostLivePolling;
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures()
        {
            var currentTournaments =
                    _rugbyService.GetCurrentTournaments().ToList();

            foreach (var tournament in currentTournaments)
            {
                var liveFixtures =
                    _rugbyService.GetLiveFixturesForCurrentTournament(tournament.Id);

                foreach (var fixture in liveFixtures)
                {
                    var matchName = fixture.HomeTeam.Name + " vs " + fixture.AwayTeam.Name;
                    var jobId = ConfigurationManager.AppSettings["LiveMangerJob_LiveMatch_JobIdPrefix"] + matchName;
                    var jobCronExpression = ConfigurationManager.AppSettings["LiveMangerJob_LiveMatch_JobCronExpression"];

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _rugbyIngestService.IngestMatchStatsForFixture(CancellationToken.None, fixture.ProviderFixtureId),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    RecurringJob.Trigger(jobId);
                    var fixtureInDb =
                            _schedulerTrackingRugbyFixtureRepository.Where(f => f.FixtureId == fixture.Id).FirstOrDefault();

                    if (fixtureInDb != null)
                    {
                        fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling;
                    }
                }
            }

            return await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingOneMonthsFixturesForActiveTournaments()
        {
            var activeTournaments =
                    _rugbyService
                    .GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_ActiveTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_ActiveTournaments_JobCronExpression"];

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _rugbyIngestService.IngestOneMonthsFixturesForTournament(CancellationToken.None, tournament.ProviderTournamentId),
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

        private async Task DeleteChildJobsForInactiveAndEndedTournaments()
        {
            var endedTournaments =
                _rugbyService.GetEndedTournaments();

            var inactiveTournaments =
                _rugbyService.GetInactiveTournaments();

            await DeleteJobsForFetchingFixturesForTournaments(endedTournaments);
            await DeleteJobsForFetchingFixturesForTournaments(inactiveTournaments);
        }

        private async Task<int> DeleteJobsForFetchingFixturesForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
            foreach (var tournament in tournaments)
            {
                var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobIdPrefix"] + tournament.Name;
                RecurringJob.RemoveIfExists(jobId);

                var season =
                    _schedulerTrackingRugbySeasonRepository
                        .Where(
                            s => s.TournamentId == tournament.Id && s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                        .FirstOrDefault();

                if (season != null)
                {
                    season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                }
            }

            return await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }

        private async Task<int> CreateChildJobsForFetchingFixturesForTournamentSeason()
        {
            var currentTournaments =
                _rugbyService
                .GetCurrentTournaments().ToList();

            foreach (var tournament in currentTournaments)
            {
                if (_rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_CurrentTournaments_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_CurrentTournaments_JobCronExpression"];

                    var seasonId = _rugbyService.GetCurrentProviderSeasonIdForTournament(tournament.Id);

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _rugbyIngestService.IngestFixturesForTournamentSeason(CancellationToken.None, tournament.ProviderTournamentId, seasonId),
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

        private async Task<int> CreateChildJobsForFetchingLogs()
        {
            var activeTournaments = _rugbyService.GetActiveTournamentsForMatchesInResultsState();

            foreach (var tournament in activeTournaments)
            {
                int seasonId = _rugbyService.GetSeasonIdForTournament(tournament.Id);

                if (_rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_JobIdPrefix"] + tournament.Name;

                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                    AddOrUpdateHangfireJob(tournament.ProviderTournamentId, seasonId, jobId, jobCronExpression);

                    QueueJobForLowFrequencyPolling(tournament.Id, tournament.ProviderTournamentId, seasonId, jobId);

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

        private void AddOrUpdateHangfireJob(int providerTournamentId, int seasonId, string jobId, string jobCronExpression)
        {
            RecurringJob.AddOrUpdate(
                jobId,
                () => _rugbyIngestService.IngestLogsForTournamentSeason(CancellationToken.None, providerTournamentId, seasonId),
                jobCronExpression,
                TimeZoneInfo.Utc,
                HangfireQueueConfiguration.HighPriority);
        }

        private void QueueJobForLowFrequencyPolling(Guid tournamentId, int providerTournamentId, int seasonId, string jobId)
        {
            string highFreqExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_HighFrequencyPolling_ExpiryInMinutes"];

            int udpateJobFrequencyOnThisMinute = int.Parse(highFreqExpiryFromConfig);

            var timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(udpateJobFrequencyOnThisMinute).TotalMilliseconds
            };

            timer.Elapsed += delegate
            {
                string jobExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_LowFrequencyPolling_ExpiryInMinutes"];

                var deleteJobOnThisMinute = int.Parse(jobExpiryFromConfig);

                var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Logs_CurrentTournaments_LowFrequencyPolling_CronExpression"];

                AddOrUpdateHangfireJob(providerTournamentId, seasonId, jobId, jobCronExpression);

                LogsJobCleanupManager.QueueJobForDeletion(tournamentId, jobId, deleteJobOnThisMinute);

                timer.Stop();
            };

            timer.Start();
        }
    }
}