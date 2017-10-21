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
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        private readonly IBaseEntityFrameworkRepository<RugbyMatchEvent> _rugbyMatchEventsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyMatchStatistics> _rugbyMatchStatisticsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyCommentary> _rugbyCommentaryRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyPlayerLineup> _rugbyPlayerLineupsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFlatLog> _rugbyFlatLogsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyGroupedLog> _rugbyGroupedLogsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixturesRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;

        public RugbyService(
            IBaseEntityFrameworkRepository<RugbyMatchEvent> rugbyMatchEventsRepository,
            IBaseEntityFrameworkRepository<RugbyMatchStatistics> rugbyMatchStatisticsRepository,
            IBaseEntityFrameworkRepository<RugbyPlayerLineup> rugbyPlayerLineupsRepository,
            IBaseEntityFrameworkRepository<RugbyCommentary> rugbyCommentaryRepository,
            IBaseEntityFrameworkRepository<RugbyGroupedLog> groupedLogsRepository,
            IBaseEntityFrameworkRepository<RugbyFlatLog> logRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixturesRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournamentRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepository)
        {
            _rugbyMatchEventsRepository = rugbyMatchEventsRepository;
            _rugbyMatchStatisticsRepository = rugbyMatchStatisticsRepository;
            _rugbyPlayerLineupsRepository = rugbyPlayerLineupsRepository;
            _rugbyCommentaryRepository = rugbyCommentaryRepository;
            _rugbyGroupedLogsRepository = groupedLogsRepository;
            _rugbyFlatLogsRepository = logRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _rugbyFixturesRepository = rugbyFixturesRepository;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepository;
            _schedulerTrackingRugbyTournamentRepository = schedulerTrackingRugbyTournamentRepository;
        }
        public async Task<IEnumerable<RugbyTournament>> GetActiveTournaments()
        {
            return (await _rugbyTournamentRepository.AllAsync()).Where(c => c.IsEnabled);
        }

        public async Task<IEnumerable<RugbyTournament>> GetCurrentTournaments()
        {
            var tournamentsGuidsThatAreCurrent = (await _schedulerTrackingRugbySeasonRepository.AllAsync()).Where(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress).Select(s => s.TournamentId);
            return (await _rugbyTournamentRepository.AllAsync()).Where(t => t.IsEnabled && tournamentsGuidsThatAreCurrent.Contains(t.Id)).Select(t => t);
        }

        public async Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid tournamentId)
        {
            var season =
                (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                    .Where(
                        s => s.TournamentId == tournamentId &&
                        (s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive))
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

        public async Task<int> GetCurrentProviderSeasonIdForTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            var currentSeason =
                    (await _rugbySeasonRepository.AllAsync())
                        .Where(season => season.RugbyTournament.Id == tournamentId && season.IsCurrent)
                        .FirstOrDefault();

            if (currentSeason != null)
            {
                return currentSeason.ProviderSeasonId;
            }

            return DateTime.Now.Year;
        }

        public async Task<IEnumerable<RugbyFixture>> GetLiveFixturesForCurrentTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowPlus15Minutes = DateTimeOffset.UtcNow.AddMinutes(15);

            var liveGames = (await _rugbyFixturesRepository.AllAsync())
                    .Where(
                        fixture => (fixture.RugbyTournament != null && fixture.RugbyTournament.Id == tournamentId) &&
                        ((fixture.RugbyFixtureStatus != RugbyFixtureStatus.PostMatch &&
                            fixture.StartDateTime <= nowPlus15Minutes && fixture.StartDateTime >= now) ||
                            (fixture.RugbyFixtureStatus == RugbyFixtureStatus.InProgress)));

            // For debugging when there's no live games.
            //return
            //    (await _rugbyFixturesRepository.AllAsync())
            //        .Where(f => f.RugbyTournament.Id == tournamentId && (f.ProviderFixtureId == 20171211210 || f.ProviderFixtureId == 20171211220));

            return liveGames;
        }

        public async Task<int> GetLiveFixturesCount(CancellationToken cancellationToken)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowPlus15Minutes = DateTimeOffset.UtcNow.AddMinutes(15);

            int count =
                (await _rugbyFixturesRepository.AllAsync())
                    .Where(
                        fixture =>
                        ((fixture.RugbyFixtureStatus != RugbyFixtureStatus.PostMatch &&
                            fixture.StartDateTime <= nowPlus15Minutes && fixture.StartDateTime >= now) ||
                            (fixture.RugbyFixtureStatus == RugbyFixtureStatus.InProgress))).Count();

            // For debugging when there's no live games.
            //return 2;

            return count;
        }

        public async Task<IEnumerable<RugbyFixture>> GetEndedFixtures()
        {
            // For debugging when there's no live games.
            //return new List<RugbyFixture>();
            return
                (await _rugbyFixturesRepository.AllAsync()).Where(
                    f => f.RugbyFixtureStatus == RugbyFixtureStatus.PostMatch ||
                         f.RugbyFixtureStatus == RugbyFixtureStatus.Result);
        }

        public async Task<bool> HasFixtureEnded(long providerFixtureId)
        {
            var fixture =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(f => f.ProviderFixtureId == providerFixtureId)
                        .FirstOrDefault();

            if (fixture != null)
                return fixture.RugbyFixtureStatus == RugbyFixtureStatus.PostMatch;

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
            var itemsToDelete = (await _schedulerTrackingRugbyTournamentRepository.AllAsync()).Where(t => disabledTournamentsIds.Contains(t.TournamentId));

            foreach (var item in itemsToDelete)
                _schedulerTrackingRugbyTournamentRepository.Delete(item);

            await _schedulerTrackingRugbyTournamentRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbySeasonsTable()
        {
            var endedSeasons = (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                .Where(
                    s => s.RugbySeasonStatus == RugbySeasonStatus.Ended);

            foreach (var item in endedSeasons)
                _schedulerTrackingRugbySeasonRepository.Delete(item);

            await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbyFixturesTable()
        {
            var nowMinus6Months = DateTimeOffset.UtcNow - TimeSpan.FromDays(180);
            var itemsToDelete = (await _schedulerTrackingRugbyFixtureRepository.AllAsync())
                .Where(
                    f => f.RugbyFixtureStatus == RugbyFixtureStatus.PostMatch && f.StartDateTime < nowMinus6Months);

            foreach (var item in itemsToDelete)
                _schedulerTrackingRugbyFixtureRepository.Delete(item);

            await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(Guid tournamentId, RugbyFixtureStatus fixtureStatus)
        {
            var fixtures = (await _rugbyFixturesRepository.AllAsync())
                .Where(t => t.RugbyTournament.Id == tournamentId &&
                t.RugbyFixtureStatus == fixtureStatus);

            return fixtures;
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(string tournamentSlug)
        {
            Guid tournamentId = await GetTournamentId(tournamentSlug);

            var fixtures = (await _rugbyFixturesRepository.AllAsync())
                            .ToList()
                            .Where(t => t.RugbyTournament.Id == tournamentId &&
                            t.RugbyFixtureStatus == RugbyFixtureStatus.PreMatch ||
                            t.RugbyFixtureStatus == RugbyFixtureStatus.InProgress);

            return fixtures;
        }

        public async Task<Guid> GetTournamentId(string tournamentSlug)
        {
            return (await _rugbyTournamentRepository.AllAsync())
                .Where(f => f.Slug == tournamentSlug)
                .FirstOrDefault().Id;
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentResults(string tournamentSlug)
        {
            Guid tournamentId = await GetTournamentId(tournamentSlug);
            var fixturesInResultsState = await GetTournamentFixtures(tournamentId, RugbyFixtureStatus.PostMatch);

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

        public async Task<IEnumerable<RugbyFixture>> GetCurrentDayFixturesForActiveTournaments()
        {
            var todayFixtures = (await _rugbyFixturesRepository.AllAsync())
                 .Where(f => f.StartDateTime.UtcDateTime.Date == DateTime.UtcNow.Date && f.RugbyTournament.IsEnabled)
                 .ToList();

            return todayFixtures;
        }

        public async Task<RugbyMatchDetailsEntity> GetMatchDetails(int LegacyMatchId)
        {
            var fixture = GetRugbyFixtureByLegacyMatchId(LegacyMatchId);

            var teamAlineup = GetTeamLineupForFixture(fixture.Id, fixture.TeamA.Id);

            var teamBlineup = GetTeamLineupForFixture(fixture.Id, fixture.TeamB.Id);

            var bothTeamsLineups = teamAlineup.Concat(teamBlineup).OrderBy(p => p.JerseyNumber).ToList();

            var statsA = GetMatchStatsForTeam(fixture.Id, fixture.TeamA.Id);

            var statsB = GetMatchStatsForTeam(fixture.Id, fixture.TeamB.Id);

            var events = GetRugbyFixtureEvents(fixture);

            var matchCommentaryAsEvents =  await GetCommentaryAsRugbyMatchEvents(fixture.Id);

            events.AddRange(matchCommentaryAsEvents);

            var teamAScorers =  GetTeamScorersForRugbyFixture(fixture.Id, fixture.TeamA.Id);

            var teamBScorers =  GetTeamScorersForRugbyFixture(fixture.Id, fixture.TeamB.Id);

            var matchDetails = new RugbyMatchDetailsEntity
            {
                TeamALineup = teamAlineup,
                TeamBLineup = teamBlineup,
                TeamAMatchStatistics = statsA,
                TeamBMatchStatistics = statsB,
                RugbyFixture = fixture,
                TeamsLineups = bothTeamsLineups,
                MatchEvents = events,
                TeamAScorers = teamAScorers,
                TeamBScorers = teamBScorers
            };

            return matchDetails;
        }

        private List<LegacyRugbyScorerEntity> GetTeamScorersForRugbyFixture(Guid fixtureId, Guid TeamId)
        {
            var events = ( _rugbyMatchEventsRepository.All())
                .Where(s => s.RugbyFixture.Id == fixtureId && s.RugbyTeamId == TeamId)
                .ToList()
                .Where(e => e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Try ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Conversion ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Penalty ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.PenaltyTryFivePoints ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.PenaltyTrySevenPoints ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.DropGoal ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.DropGoalFromMark)
                .ToList();

            var scorers = new List<LegacyRugbyScorerEntity>();

            foreach (var rugbyEvent in events)
            {
                var scorer = new LegacyRugbyScorerEntity
                {
                    CombinedName = rugbyEvent.RugbyPlayer1.FullName,
                    DisplayName = rugbyEvent.RugbyPlayer1.FullName,
                    EventId = rugbyEvent.RugbyEventType.EventCode,
                    Name = rugbyEvent.RugbyPlayer1.FirstName,
                    NickName = null,
                    PersonId = rugbyEvent.RugbyPlayer1.LegacyPlayerId,
                    Surname = rugbyEvent.RugbyPlayer1.LastName,
                    Time = rugbyEvent.GameTimeInMinutes.ToString(),
                    Type = rugbyEvent.RugbyEventType.EventName
                };

                scorers.Add(scorer);
            }

            return scorers;
        }

        private RugbyFixture GetRugbyFixtureByLegacyMatchId(int LegacyMatchId)
        {
            return ( _rugbyFixturesRepository.All())
                .Where(f => f.LegacyFixtureId == LegacyMatchId)
                .ToList()
                .FirstOrDefault();
        }

        private List<RugbyMatchEvent> GetRugbyFixtureEvents(RugbyFixture fixture)
        {
            return ( _rugbyMatchEventsRepository.All())
                .Where(s => s.RugbyFixture.Id == fixture.Id)
                .ToList();
        }

        private RugbyMatchStatistics GetMatchStatsForTeam(Guid fixtureId, Guid TeamId)
        {
            return ( _rugbyMatchStatisticsRepository.All())
                    .Where(s => s.RugbyFixture.Id == fixtureId)
                    .ToList()
                    .Where(f => f.RugbyTeamId == TeamId).FirstOrDefault();
        }

        private List<RugbyPlayerLineup> GetTeamLineupForFixture(Guid fixtureId, Guid TeamId)
        {
            return (_rugbyPlayerLineupsRepository.All())
                 .Where(l => l.RugbyFixture.Id == fixtureId)
                 .ToList()
                 .Where(l => l.RugbyTeam.Id == TeamId).ToList();
        }

        private async Task<List<LegacyRugbyMatchEventEntity>> GetCommentaryAsRugbyMatchEvents(Guid fixtureId)
        {
            var matchCommentary =  await GetRugbyMatchCommentary(fixtureId);

            var matchCommentaryAsEvents = new List<LegacyRugbyMatchEventEntity>();

            foreach (var commentary in matchCommentary)
            {
                var matchEvent = new LegacyRugbyMatchEventEntity
                {
                    GameTimeInMinutes = commentary.GameTimeRawMinutes,
                    GameTimeInSeconds = commentary.GameTimeRawSeconds,
                    RugbyPlayer1 = commentary.RugbyPlayer,
                    RugbyFixtureId = fixtureId,
                    RugbyFixture = commentary.RugbyFixture,
                    RugbyTeam = commentary.RugbyTeam,
                    RugbyTeamId = commentary.RugbyTeam.Id,
                    Id = commentary.Id,
                    Comments = commentary.CommentaryText
                };

                matchCommentaryAsEvents.Add(matchEvent);
            }

            return matchCommentaryAsEvents;
        }

        private async Task<List<RugbyCommentary>> GetRugbyMatchCommentary(Guid fixtureId)
        {
            return (await _rugbyCommentaryRepository.AllAsync())
                 .Where(c => c.RugbyFixture.Id == fixtureId)
                 .ToList();
        }
    }
}