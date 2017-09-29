using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Entities;
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
        //private readonly IBaseEntityFrameworkRepository<Log> _logRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixturesRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;

        public RugbyService(
            // TODO: This was commented out when deleting old Log DB table.
            //IBaseEntityFrameworkRepository<Log> logRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixturesRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournamentRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepository)
        {
            // TODO: This was commented out when deleting old Log DB table.
            //_logRepository = logRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _rugbyFixturesRepository = rugbyFixturesRepository;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepository;
            _schedulerTrackingRugbyTournamentRepository = schedulerTrackingRugbyTournamentRepository;
        }

        public IEnumerable<LogEntity> GetLogs(string tournamentName)
        {
            // TODO: This was commented out when deleting old Log DB table.
            //var logs = _logRepository.All().Select(log => Mapper.Map<LogEntity>(log));
            //return logs;
            return null;
        }

        public IEnumerable<RugbyTournament> GetActiveTournaments()
        {
            return _rugbyTournamentRepository.Where(c => c.IsEnabled);
        }

        public IEnumerable<RugbyTournament> GetCurrentTournaments()
        {
            var tournamentsGuidsThatAreCurrent = _schedulerTrackingRugbySeasonRepository.Where(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress).Select(s => s.TournamentId);
            return _rugbyTournamentRepository.Where(t => t.IsEnabled && tournamentsGuidsThatAreCurrent.Contains(t.Id)).Select(t => t);
        }

        public SchedulerStateForManagerJobPolling GetSchedulerStateForManagerJobPolling(Guid tournamentId)
        {
            var season = 
                _schedulerTrackingRugbySeasonRepository
                    .Where(
                        s => s.TournamentId == tournamentId && 
                        ( s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive ))
                   .FirstOrDefault();

            return season != null ? season.SchedulerStateForManagerJobPolling : SchedulerStateForManagerJobPolling.Undefined;
        }

        public IEnumerable<RugbyTournament> GetEndedTournaments()
        {
            var tournamentGuids = 
                _schedulerTrackingRugbySeasonRepository
                    .Where(
                        season => season.RugbySeasonStatus == RugbySeasonStatus.Ended && 
                        season.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                    .Select(s => s.TournamentId);

            return 
                _rugbyTournamentRepository
                    .Where(tournament => tournamentGuids.Contains(tournament.Id))
                    .Select(t => t);
        }

        public IEnumerable<RugbyTournament> GetInactiveTournaments()
        {
            return 
                _rugbyTournamentRepository
                    .Where(t => t.IsEnabled == false);
        }

        public int GetCurrentProviderSeasonIdForTournament(Guid tournamentId)
        {
            var currentSeason =
                _rugbySeasonRepository
                    .Where(season => season.RugbyTournament.Id == tournamentId && season.IsCurrent)
                    .FirstOrDefault();

            if(currentSeason != null)
            {
                return currentSeason.ProviderSeasonId;
            }

            return DateTime.Now.Year;
        }

        public IEnumerable<RugbyFixture> GetLiveFixturesForCurrentTournament(Guid tournamentId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowPlus15Minutes = DateTimeOffset.UtcNow.AddMinutes(15);
            return 
                _rugbyFixturesRepository
                    .Where(
                        fixture => fixture.RugbyTournament.Id == tournamentId &&
                        ((fixture.RugbyFixtureStatus != RugbyFixtureStatus.Final &&
                          fixture.StartDateTime <= nowPlus15Minutes && fixture.StartDateTime >= now) ||
                         (fixture.RugbyFixtureStatus == RugbyFixtureStatus.InProgress)));
        }

        public IEnumerable<RugbyFixture> GetLiveFixtures()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowPlus15Minutes = DateTimeOffset.UtcNow.AddMinutes(15);
            return
                _rugbyFixturesRepository
                    .Where(
                        fixture =>
                        ((fixture.RugbyFixtureStatus != RugbyFixtureStatus.Final &&
                           fixture.StartDateTime <= nowPlus15Minutes && fixture.StartDateTime >= now) ||
                          (fixture.RugbyFixtureStatus == RugbyFixtureStatus.InProgress)));
        }

        public IEnumerable<RugbyFixture> GetEndedFixtures()
        {
            return
                _rugbyFixturesRepository.Where(
                    f => f.RugbyFixtureStatus == RugbyFixtureStatus.Final);
        }

        public bool HasFixtureEnded(long providerFixtureId)
        {
            var fixture = 
                    _rugbyFixturesRepository
                        .Where(
                            f => f.ProviderFixtureId == providerFixtureId)
                        .FirstOrDefault();

            if (fixture != null)
                return fixture.RugbyFixtureStatus == RugbyFixtureStatus.Final;

            // We can't find the fixture in the DB? But still running ingest code?
            // This is a bizzare condition but checking it nonetherless.
            return true;
        }

        public IEnumerable<RugbyTournament> GetActiveTournamentsForMatchesInResultsState()
        {
            var tournamentsThatHaveFixturesInResultState =
                    _rugbyFixturesRepository
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
            var disabledTournamentsIds = _rugbyTournamentRepository.Where(t => t.IsEnabled == false).Select(t => t.Id);
            var itemsToDelete = _schedulerTrackingRugbyTournamentRepository.Where(t => disabledTournamentsIds.Contains(t.TournamentId)).ToList();

            foreach (var item in itemsToDelete)
                _schedulerTrackingRugbyTournamentRepository.Delete(item);

            await _schedulerTrackingRugbyTournamentRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbySeasonsTable()
        {
            var endedSeasons = _schedulerTrackingRugbySeasonRepository
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
            var itemsToDelete = _schedulerTrackingRugbyFixtureRepository
                .Where(
                    f => f.RugbyFixtureStatus == RugbyFixtureStatus.Final && f.StartDateTime < nowMinus6Months)
                .ToList();

            foreach (var item in itemsToDelete)
                _schedulerTrackingRugbyFixtureRepository.Delete(item);

            await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        public IEnumerable<RugbyFixture> GetTournamentFixtures(Guid tournamentId, RugbyFixtureStatus fixtureStatus)
        {
            var fixtures = _rugbyFixturesRepository
                .All()
                .ToList()
                .Where(t => t.RugbyTournament.Id == tournamentId &&
                t.RugbyFixtureStatus == fixtureStatus);

            return fixtures;
        }

        public IEnumerable<RugbyFixture> GetTournamentFixtures(string tournamentSlug)
        {
            Guid tournamentId = GetTournamentId(tournamentSlug);

            var fixtures = GetTournamentFixtures(tournamentId, RugbyFixtureStatus.PreMatch);

            return fixtures;
        }

        public Guid GetTournamentId(string tournamentSlug)
        {
            return _rugbyTournamentRepository
                .All()
                .ToList()
                .Where(f => f.Slug == tournamentSlug)
                .FirstOrDefault().Id;
        }

        public IEnumerable<RugbyFixture> GetTournamentResults(string tournamentSlug)
        {
            Guid tournamentId = GetTournamentId(tournamentSlug);

            var fixturesInResultsState = GetTournamentFixtures(tournamentId, RugbyFixtureStatus.Final);

            return fixturesInResultsState;
        }
    }
}