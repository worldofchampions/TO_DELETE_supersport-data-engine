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

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        // TODO: This was commented out when deleting old Log DB table.
        //private readonly IBaseEntityFrameworkRepository<Log> _logRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
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
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepository)
        {
            // TODO: This was commented out when deleting old Log DB table.
            //_logRepository = logRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _rugbyFixturesRepository = rugbyFixturesRepository;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepository;
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
                        ( (fixture.RugbyFixtureStatus != RugbyFixtureStatus.Final &&
                           fixture.StartDateTime <= nowPlus15Minutes) ||
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
    }
}