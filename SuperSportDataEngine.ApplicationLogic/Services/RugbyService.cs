using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System.Threading;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        // TODO: This was commented out when deleting old Log DB table.
        private readonly IBaseEntityFrameworkRepository<RugbyFlatLog> _rugbyFlatLogsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyGroupedLog> _rugbyGroupedLogsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixturesRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;

        public RugbyService(
            IBaseEntityFrameworkRepository<RugbyGroupedLog> groupedLogsRepository,
            IBaseEntityFrameworkRepository<RugbyFlatLog> logRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixturesRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournamentRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepository)
        {
            _rugbyGroupedLogsRepository = groupedLogsRepository;
            _rugbyFlatLogsRepository = logRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _rugbyFixturesRepository = rugbyFixturesRepository;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepository;
            _schedulerTrackingRugbyTournamentRepository = schedulerTrackingRugbyTournamentRepository;
        }

        private SemaphoreSlim GetActiveTournamentsControl = new SemaphoreSlim(1, 1);
        public async Task<IEnumerable<RugbyTournament>> GetActiveTournaments()
        {
            try
            {
                await GetActiveTournamentsControl.WaitAsync();
                return (await _rugbyTournamentRepository.AllAsync()).Where(c => c.IsEnabled).ToList();
            }
            finally
            {
                GetActiveTournamentsControl.Release();
            }
        }

        public async Task<IEnumerable<RugbyTournament>> GetCurrentTournaments()
        {
            var tournamentsGuidsThatAreCurrent = (await _schedulerTrackingRugbySeasonRepository.AllAsync()).Where(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress).Select(s => s.TournamentId);
            return (await _rugbyTournamentRepository.AllAsync()).Where(t => t.IsEnabled && tournamentsGuidsThatAreCurrent.Contains(t.Id)).Select(t => t).ToList();
        }

        public async Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid tournamentId)
        {
            var season = 
                (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                    .Where(
                        s => s.TournamentId == tournamentId && 
                        ( s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive ))
                   .FirstOrDefault();

            return season != null ? season.SchedulerStateForManagerJobPolling : SchedulerStateForManagerJobPolling.Undefined;
        }

        public async Task<IEnumerable<RugbyTournament>> GetEndedTournaments()
        {
            var tournamentGuids = 
                (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                    .Where(
                        season => season.RugbySeasonStatus == RugbySeasonStatus.Ended && 
                        season.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                    .Select(s => s.TournamentId);

            return 
                (await _rugbyTournamentRepository.AllAsync())
                    .Where(tournament => tournamentGuids.Contains(tournament.Id))
                    .Select(t => t);
        }

        public async Task<IEnumerable<RugbyTournament>> GetInactiveTournaments()
        {
            return 
                (await _rugbyTournamentRepository.AllAsync())
                    .Where(t => t.IsEnabled == false);
        }

        private static SemaphoreSlim GetCurrentProviderSeasonIdForTournamentControl = new SemaphoreSlim(1, 1);
        public async Task<int> GetCurrentProviderSeasonIdForTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            try
            {
                await GetCurrentProviderSeasonIdForTournamentControl.WaitAsync(cancellationToken);

                var currentSeason =
                        (await _rugbySeasonRepository.AllAsync())
                            .Where(season => season.RugbyTournament.Id == tournamentId && season.IsCurrent)
                            .FirstOrDefault();

                if (currentSeason != null)
                {
                    return currentSeason.ProviderSeasonId;
                }
            }
            finally
            {
                GetCurrentProviderSeasonIdForTournamentControl.Release();
            }

            return DateTime.Now.Year;
        }

        private static SemaphoreSlim GetLiveFixturesForCurrentTournamentControl = new SemaphoreSlim(1, 1);
        public async Task<IEnumerable<RugbyFixture>> GetLiveFixturesForCurrentTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowPlus15Minutes = DateTimeOffset.UtcNow.AddMinutes(15);

            try
            {
                await GetLiveFixturesForCurrentTournamentControl.WaitAsync(cancellationToken);

                var liveGames = (await _rugbyFixturesRepository.AllAsync())
                        .Where(
                            fixture => fixture.RugbyTournament.Id == tournamentId &&
                            ((fixture.RugbyFixtureStatus != RugbyFixtureStatus.Final &&
                              fixture.StartDateTime <= nowPlus15Minutes && fixture.StartDateTime >= now) ||
                             (fixture.RugbyFixtureStatus == RugbyFixtureStatus.InProgress))).ToList();

                //Fake a game being played live!Specify the providerFixtureId
                //return
                //    (await _rugbyFixturesRepository.AllAsync())
                //        .Where(f => f.RugbyTournament.Id == tournamentId && (f.ProviderFixtureId == 20171211210 || f.ProviderFixtureId == 20171211220));

                return liveGames;
            }
            finally
            {
                GetLiveFixturesForCurrentTournamentControl.Release();
            }
        }

        private static SemaphoreSlim GetLiveFixturesCountControl = new SemaphoreSlim(1, 1);
        public async Task<int> GetLiveFixturesCount(CancellationToken cancellationToken)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowPlus15Minutes = DateTimeOffset.UtcNow.AddMinutes(15);

            try
            {
                await GetLiveFixturesCountControl.WaitAsync(cancellationToken);

                int count =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(
                            fixture =>
                            ((fixture.RugbyFixtureStatus != RugbyFixtureStatus.Final &&
                               fixture.StartDateTime <= nowPlus15Minutes && fixture.StartDateTime >= now) ||
                              (fixture.RugbyFixtureStatus == RugbyFixtureStatus.InProgress))).ToList().Count();

                return count;
            }
            finally
            {
                GetLiveFixturesCountControl.Release();
            }
        }

        public async Task<IEnumerable<RugbyFixture>> GetEndedFixtures()
        {
            return
                (await _rugbyFixturesRepository.AllAsync()).Where(
                    f => f.RugbyFixtureStatus == RugbyFixtureStatus.Final);
        }

        public async Task<bool> HasFixtureEnded(long providerFixtureId)
        {
            var fixture =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(
                            f => f.ProviderFixtureId == providerFixtureId)
                        .FirstOrDefault();

            if (fixture != null)
                return fixture.RugbyFixtureStatus == RugbyFixtureStatus.Final;

            // We can't find the fixture in the DB? But still running ingest code?
            // This is a bizzare condition but checking it nonetherless.
            return true;
        }

        public async Task<IEnumerable<RugbyTournament>> GetActiveTournamentsForMatchesInResultsState()
        {
            var tournamentsThatHaveFixturesInResultState =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(f => f.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                        .Select(f => f.RugbyTournament);

            return tournamentsThatHaveFixturesInResultState;
        }

        public async Task CleanupSchedulerTrackingTables(CancellationToken none)
        {
            await CleanupSchedulerTrackingRugbyFixturesTable();
            await CleanupSchedulerTrackingRugbySeasonsTable();
            await CleanupSchedulerTrackingRugbyTournamentsTable();
        }

        public async Task CleanupSchedulerTrackingRugbyTournamentsTable()
        {
            var disabledTournamentsIds = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.IsEnabled == false).Select(t => t.Id);
            var itemsToDelete = (await _schedulerTrackingRugbyTournamentRepository.AllAsync()).Where(t => disabledTournamentsIds.Contains(t.TournamentId)).ToList();

            foreach (var item in itemsToDelete)
                _schedulerTrackingRugbyTournamentRepository.Delete(item);

            await _schedulerTrackingRugbyTournamentRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbySeasonsTable()
        {
            var endedSeasons = (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                .Where(
                    s => s.RugbySeasonStatus == RugbySeasonStatus.Ended)
                .ToList();

            foreach (var item in endedSeasons)
                _schedulerTrackingRugbySeasonRepository.Delete(item);

            await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbyFixturesTable()
        {
            var nowMinus6Months = DateTimeOffset.UtcNow - TimeSpan.FromDays(180);
            var itemsToDelete = (await _schedulerTrackingRugbyFixtureRepository.AllAsync())
                .Where(
                    f => f.RugbyFixtureStatus == RugbyFixtureStatus.Final && f.StartDateTime < nowMinus6Months)
                .ToList();

            foreach (var item in itemsToDelete)
                _schedulerTrackingRugbyFixtureRepository.Delete(item);

            await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(Guid tournamentId, RugbyFixtureStatus fixtureStatus)
        {
            var fixtures = (await _rugbyFixturesRepository.AllAsync())
                .ToList()
                .Where(t => t.RugbyTournament.Id == tournamentId &&
                t.RugbyFixtureStatus == fixtureStatus);

            return fixtures;
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(string tournamentSlug)
        {
            Guid tournamentId = await GetTournamentId(tournamentSlug);

            var fixtures = await GetTournamentFixtures(tournamentId, RugbyFixtureStatus.PreMatch);

            return fixtures;
        }

        public async Task<Guid> GetTournamentId(string tournamentSlug)
        {
            return (await _rugbyTournamentRepository.AllAsync())
                .ToList()
                .Where(f => f.Slug == tournamentSlug)
                .FirstOrDefault().Id;
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentResults(string tournamentSlug)
        {
            Guid tournamentId = await GetTournamentId(tournamentSlug);
            var fixturesInResultsState = await GetTournamentFixtures(tournamentId, RugbyFixtureStatus.Final);

            return fixturesInResultsState;
        }

        public async Task<IEnumerable<RugbyGroupedLog>> GetGroupedLogs(string tournamentSlug)
        {
            var tournamentId = await GetTournamentId(tournamentSlug);

            var logs = (await _rugbyGroupedLogsRepository.AllAsync())
                .Where(t => t.RugbyTournament.IsEnabled && t.RugbyTournamentId == tournamentId)
                .ToList();

            return logs;
        }

        public async Task<IEnumerable<RugbyFlatLog>> GetFlatLogs(string tournamentSlug)
        {
            var tournamentId = await GetTournamentId(tournamentSlug);

            var flatLogs = (await _rugbyFlatLogsRepository.AllAsync())
                .Where(t => t.RugbyTournament.IsEnabled && t.RugbyTournamentId == tournamentId)
                .ToList();

            return flatLogs;
        }

        public async Task <IEnumerable<RugbyFixture>> GetCurrentDayFixturesForActiveTournaments()
        {
            var todayFixtures = (await _rugbyFixturesRepository.AllAsync())
                 .Where(f => f.StartDateTime.UtcDateTime.Date == DateTime.UtcNow.Date && f.RugbyTournament.IsEnabled)
                 .ToList();

            return todayFixtures;
        }
    }
}