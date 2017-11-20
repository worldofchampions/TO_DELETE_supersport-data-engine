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
                    .FirstOrDefault(
                        s => s.TournamentId == tournamentId &&
                        (s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive));

            return season?.SchedulerStateForManagerJobPolling ?? SchedulerStateForManagerJobPolling.Undefined;
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
                        .FirstOrDefault(season => season.RugbyTournament.Id == tournamentId && season.IsCurrent);

            if (currentSeason != null)
            {
                return currentSeason.ProviderSeasonId;
            }

            return DateTime.Now.Year;
        }

        public async Task<IEnumerable<RugbyFixture>> GetLiveFixturesForCurrentTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            var liveGames = (await _rugbyFixturesRepository.AllAsync())
                    .Where(IsFixtureLive)
                    .Where(fixture => fixture.RugbyTournament != null && fixture.RugbyTournament.Id == tournamentId);

            // For debugging when there's no live games.
            //return
            //    (await _rugbyFixturesRepository.AllAsync())
            //        .Where(f => f.RugbyTournament.Id == tournamentId && (f.ProviderFixtureId == 20171211210 || f.ProviderFixtureId == 20171211220));

            return liveGames;
        }

        private bool IsFixtureLive(RugbyFixture fixture)
        {
            var fixtureSchedule = _schedulerTrackingRugbyFixtureRepository.All().ToList()
                    .FirstOrDefault(s => s.FixtureId == fixture.Id);

            if (fixtureSchedule == null)
                return false;

            return fixtureSchedule.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.LivePolling ||
                   fixtureSchedule.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.PreLivePolling;
        }

        public async Task<int> GetLiveFixturesCount(CancellationToken cancellationToken)
        {
            var liveGames = (await _rugbyFixturesRepository.AllAsync())
                                .Where(IsFixtureLive);

            // For debugging when there's no live games.
            //return 2;

            return liveGames.Count();
        }

        public async Task<IEnumerable<RugbyFixture>> GetCompletedFixtures()
        {
            // For debugging when there's no live games.
            //return new List<RugbyFixture>();
            var completedSchedules = (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(
                    f => f.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.SchedulingCompleted).Select(s => s.FixtureId).ToList();
            var fixturesCompleted = (await _rugbyFixturesRepository.AllAsync()).Where(f => completedSchedules.Contains(f.Id));

            return fixturesCompleted;
        }

        public async Task<bool> HasFixtureEnded(long providerFixtureId)
        {
            var fixture =
                    (await _rugbyFixturesRepository.AllAsync())
                        .FirstOrDefault(f => f.ProviderFixtureId == providerFixtureId);

            if (fixture != null)
                return fixture.RugbyFixtureStatus == RugbyFixtureStatus.Result;

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

            _schedulerTrackingRugbyTournamentRepository.DeleteRange(itemsToDelete);

            await _schedulerTrackingRugbyTournamentRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbySeasonsTable()
        {
            var endedSeasons = (await _schedulerTrackingRugbySeasonRepository.AllAsync())
                .Where(
                    s => s.RugbySeasonStatus == RugbySeasonStatus.Ended).ToList();

            _schedulerTrackingRugbySeasonRepository.DeleteRange(endedSeasons);

            await _schedulerTrackingRugbySeasonRepository.SaveAsync();
        }

        public async Task CleanupSchedulerTrackingRugbyFixturesTable()
        {
            var nowMinus6Months = DateTimeOffset.UtcNow - TimeSpan.FromDays(180);
            var itemsToDelete = (await _schedulerTrackingRugbyFixtureRepository.AllAsync())
                .Where(
                    f => (f.RugbyFixtureStatus == RugbyFixtureStatus.Result || f.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.SchedulingCompleted) && f.StartDateTime < nowMinus6Months).ToList();

            _schedulerTrackingRugbyFixtureRepository.DeleteRange(itemsToDelete);

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
            if (IsNationalTeamSlug(tournamentSlug))
            {
                return await GetNationalTeamFixtures();
            }

            var tournament = await GetTournamentBySlug(tournamentSlug);

            var fixtures = Enumerable.Empty<RugbyFixture>();

            if (tournament == null) return fixtures;

            fixtures = (await _rugbyFixturesRepository.AllAsync())
                .Where(t => t.RugbyTournament.Id == tournament.Id && t.RugbyFixtureStatus != RugbyFixtureStatus.Result);
                   

            return fixtures.OrderBy(f => f.StartDateTime);
        }

        public async Task<Guid> GetTournamentId(string tournamentSlug)
        {
            var tournament = (await _rugbyTournamentRepository.AllAsync()).FirstOrDefault(f => f.Slug == tournamentSlug);

            if (tournament == null)
                throw new Exception("Tournament slug does not exist.");

            return tournament.Id;
        }

        public async Task<RugbyTournament> GetTournamentBySlug(string tournamentSlug)
        {
            return (await _rugbyTournamentRepository.AllAsync())
                .FirstOrDefault(f => f.Slug.Equals(tournamentSlug, StringComparison.InvariantCultureIgnoreCase));
        }


        public async Task<IEnumerable<RugbyFixture>> GetTournamentResults(string tournamentSlug)
        {
            if (IsNationalTeamSlug(tournamentSlug))
            {
                return await GetNationalTeamResults();
            }

            var tournament = await GetTournamentBySlug(tournamentSlug);

            var fixturesInResultsState = Enumerable.Empty<RugbyFixture>();

            if (tournament != null)
            {
                fixturesInResultsState = (await GetTournamentFixtures(tournament.Id, RugbyFixtureStatus.Result))
                    .OrderByDescending(f => f.StartDateTime);
            }

            return fixturesInResultsState;
        }

        private static bool IsNationalTeamSlug(string slug)
        {
            var result = slug.Equals("springboks", StringComparison.CurrentCultureIgnoreCase);

            return result;
        }

        private async Task<IEnumerable<RugbyFixture>> GetNationalTeamResults()
        {
            const string nationalTeamName = "South Africa";

            var fixtures = (await _rugbyFixturesRepository.AllAsync())
                    .Where(f => 
                    ((f.TeamA != null && f.TeamA.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase)) ||
                    (f.TeamB != null && f.TeamB.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase))) &&
                    f.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                    .OrderByDescending(f => f.StartDateTime);

            return fixtures;
        }

        private async Task<IEnumerable<RugbyFixture>> GetNationalTeamFixtures()
        {
            const string nationalTeamName = "South Africa";

            var fixtures = (await _rugbyFixturesRepository.AllAsync())
                    .Where(f =>
                    ((f.TeamA != null && f.TeamA.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase)) ||
                     (f.TeamB != null && f.TeamB.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase))) &&
                    f.RugbyFixtureStatus != RugbyFixtureStatus.Result)
                    .OrderBy(f => f.StartDateTime);

            return fixtures;
        }

        public async Task<IEnumerable<RugbyGroupedLog>> GetGroupedLogs(string tournamentSlug)
        {
            var tournament = await GetTournamentBySlug(tournamentSlug);

            var logs = Enumerable.Empty<RugbyGroupedLog>();

            if (tournament != null)
            {
                logs = (await _rugbyGroupedLogsRepository.AllAsync())
                    .Where(t => t.RugbyTournament.IsEnabled && t.RugbyTournamentId == tournament.Id)
                    .OrderBy(g => g.RugbyLogGroup.Id)
                    .ThenBy(t => t.LogPosition);

                return logs;
            }

            return logs;
        }

        public async Task<IEnumerable<RugbyFlatLog>> GetFlatLogs(string tournamentSlug)
        {
            var tournament = await GetTournamentBySlug(tournamentSlug);

            var flatLogs = Enumerable.Empty<RugbyFlatLog>();

            if (tournament != null)
            {
                flatLogs = (await _rugbyFlatLogsRepository.AllAsync())
                    .Where(t => t.RugbyTournament.IsEnabled && t.RugbyTournamentId == tournament.Id && t.RugbySeason.IsCurrent)
                    .OrderBy(t => t.LogPosition);
            }

            return flatLogs;
        }

        public async Task<IEnumerable<RugbyFixture>> GetCurrentDayFixturesForActiveTournaments()
        {
            var todayFixtures = (await _rugbyFixturesRepository.AllAsync())
                 .Where(f => f.StartDateTime.UtcDateTime.Date == DateTime.UtcNow.Date && f.RugbyTournament.IsEnabled)
                 .ToList();

            return todayFixtures.OrderBy(f => f.StartDateTime);
        }

        public async Task<RugbyMatchDetailsEntity> GetMatchDetailsByLegacyMatchId(int legacyMatchId)
        {
            var fixture = await GetRugbyFixtureByLegacyMatchId(legacyMatchId);

            if (fixture is null)
            {
                return null;
            }

            var teamAlineup = await GetTeamLineupForFixture(fixture.Id, fixture.TeamA?.Id ?? Guid.Empty);

            var teamBlineup = await GetTeamLineupForFixture(fixture.Id, fixture.TeamB?.Id ?? Guid.Empty);

            var bothTeamsLineups = teamAlineup.Concat(teamBlineup).OrderBy(p => p.JerseyNumber).ToList();

            var statsA = await GetMatchStatsForTeam(fixture.Id, fixture.TeamA?.Id ?? Guid.Empty);

            var statsB = await GetMatchStatsForTeam(fixture.Id, fixture.TeamB?.Id ?? Guid.Empty);

            var events = await GetRugbyFixtureEvents(fixture.Id);

            var matchCommentaryAsEvents = await GetCommentaryAsRugbyMatchEvents(fixture.Id);

            events.AddRange(matchCommentaryAsEvents);

            events = events.OrderByDescending(e => e.GameTimeInSeconds).ThenByDescending(e => e.TimestampCreated).ToList();

            var teamAScorers = await GetTeamScorersForFixture(fixture.Id, fixture.TeamA?.Id ?? Guid.Empty);

            var teamBScorers = await GetTeamScorersForFixture(fixture.Id, fixture.TeamB?.Id ?? Guid.Empty);

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

        private async Task<List<LegacyRugbyScorerEntity>> GetTeamScorersForFixture(Guid fixtureId, Guid teamId)
        {
            var teamScoringEvents = await GetTeamScoringEventsForFixture(fixtureId, teamId);

            return teamScoringEvents.Select(scoringEvent => new LegacyRugbyScorerEntity
            {
                CombinedName = scoringEvent.RugbyPlayer1.FullName,
                DisplayName = scoringEvent.RugbyPlayer1.FullName,
                EventId = scoringEvent.RugbyEventType.EventCode,
                Name = scoringEvent.RugbyPlayer1.FirstName,
                NickName = null,
                PersonId = scoringEvent.RugbyPlayer1.LegacyPlayerId,
                Surname = scoringEvent.RugbyPlayer1.LastName,
                Time = scoringEvent.GameTimeInMinutes.ToString(),
                Type = scoringEvent.RugbyEventType.EventName
            })
            .ToList();
        }

        private async Task<List<RugbyMatchEvent>> GetTeamScoringEventsForFixture(Guid fixtureId, Guid teamId)
        {
            return (await _rugbyMatchEventsRepository.AllAsync())
                .Where(s => s.RugbyFixture.Id == fixtureId && s.RugbyTeamId == teamId)
                .ToList().Where(e =>
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.PenaltyTryFivePoints ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.PenaltyTrySevenPoints ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.DropGoalFromMark ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Conversion ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.DropGoal ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Penalty ||
                e.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Try)
                .ToList();
        }

        private async Task<RugbyFixture> GetRugbyFixtureByLegacyMatchId(int legacyMatchId)
        {
            return (await _rugbyFixturesRepository.AllAsync())
                .FirstOrDefault(f => f.LegacyFixtureId == legacyMatchId);
        }

        private async Task<List<RugbyMatchEvent>> GetRugbyFixtureEvents(Guid fixtureId)
        {
            return (await _rugbyMatchEventsRepository.AllAsync())
                .Where(s => s.RugbyFixture.Id == fixtureId)
                .ToList();
        }

        private async Task<RugbyMatchStatistics> GetMatchStatsForTeam(Guid fixtureId, Guid teamId)
        {
            return (await _rugbyMatchStatisticsRepository.AllAsync())
                    .FirstOrDefault(s => s.RugbyFixture.Id == fixtureId && s.RugbyTeamId == teamId);
        }

        private async Task<List<RugbyPlayerLineup>> GetTeamLineupForFixture(Guid fixtureId, Guid teamId)
        {
            return (await _rugbyPlayerLineupsRepository.AllAsync())
                 .Where(l => l.RugbyFixture.Id == fixtureId && l.RugbyTeam.Id == teamId).ToList();
        }

        private async Task<List<LegacyRugbyMatchEventEntity>> GetCommentaryAsRugbyMatchEvents(Guid fixtureId)
        {
            var matchCommentaries = await GetRugbyMatchCommentary(fixtureId);

            var matchCommentariesAsEvents = new List<LegacyRugbyMatchEventEntity>();

            foreach (var commentary in matchCommentaries)
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

                matchCommentariesAsEvents.Add(matchEvent);
            }

            return matchCommentariesAsEvents;
        }

        private async Task<List<RugbyCommentary>> GetRugbyMatchCommentary(Guid fixtureId)
        {
            return (await _rugbyCommentaryRepository.AllAsync())
                 .Where(c => c.RugbyFixture.Id == fixtureId)
                 .ToList();
        }

        public async Task<IEnumerable<RugbyFixture>> GetPostponedFixtures()
        {
            return (await _rugbyFixturesRepository.AllAsync())
                .Where(f =>
                    f.RugbyFixtureStatus == RugbyFixtureStatus.PreMatch &&
                    f.StartDateTime < (DateTimeOffset.UtcNow - TimeSpan.FromHours(3)));
        }

        public async Task<IEnumerable<RugbyFixture>> GetFixturesNotIngestedYet()
        {
            var pastFixturesIdsNotScheduledYet =
                            (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).Where(s => s.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingCompleted &&
                            s.StartDateTime < DateTime.UtcNow - TimeSpan.FromHours(6)).Select(s => s.FixtureId).ToList();

            var fixtures = (_rugbyFixturesRepository.Where(f => pastFixturesIdsNotScheduledYet.Contains(f.Id)))
                                .OrderByDescending(f => f.StartDateTime);

            return fixtures;
        }

        public async Task<IEnumerable<RugbyFixture>> GetPastDaysFixtures(int numberOfDays)
        {
            var now = DateTime.UtcNow;

            var fixtures = (await _rugbyFixturesRepository.AllAsync())
                .Where(f => now - f.StartDateTime < TimeSpan.FromDays(numberOfDays)).ToList();

            return fixtures;
        }
    }
}