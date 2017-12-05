namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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
            return await Task.FromResult(_rugbyTournamentRepository.Where(c => c.IsEnabled).ToList());
        }

        public async Task<IEnumerable<RugbyTournament>> GetCurrentTournaments()
        {
            var tournamentsGuidsThatAreCurrent = await Task.FromResult(_schedulerTrackingRugbySeasonRepository.Where(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress).Select(s => s.TournamentId).ToList());
            return await Task.FromResult(_rugbyTournamentRepository.Where(t => t.IsEnabled && tournamentsGuidsThatAreCurrent.Contains(t.Id)).Select(t => t).ToList());
        }

        public async Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid tournamentId)
        {
            var season =
                await Task.FromResult(_schedulerTrackingRugbySeasonRepository
                    .FirstOrDefault(
                        s => s.TournamentId == tournamentId &&
                        (s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive)));

            return season?.SchedulerStateForManagerJobPolling ?? SchedulerStateForManagerJobPolling.Undefined;
        }

        public async Task<IEnumerable<RugbyTournament>> GetEndedTournaments()
        {
            var tournamentGuids =
                await Task.FromResult(_schedulerTrackingRugbySeasonRepository
                    .Where(
                        season => season.RugbySeasonStatus == RugbySeasonStatus.Ended &&
                        season.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                    .Select(s => s.TournamentId));

            return await Task.FromResult(_rugbyTournamentRepository.Where(tournament => tournamentGuids.Contains(tournament.Id)).Select(t => t));
        }

        public async Task<IEnumerable<RugbyTournament>> GetInactiveTournaments()
        {
            return await Task.FromResult(_rugbyTournamentRepository.Where(t => t.IsEnabled == false).ToList());
        }

        public async Task<int> GetCurrentProviderSeasonIdForTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            var currentSeason =
                    await Task.FromResult(_rugbySeasonRepository.FirstOrDefault(season => season.RugbyTournament.Id == tournamentId && season.IsCurrent));

            return currentSeason?.ProviderSeasonId ?? DateTime.Now.Year;
        }

        public async Task<IEnumerable<RugbyFixture>> GetLiveFixturesForCurrentTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            var liveGames = (await _rugbyFixturesRepository.AllAsync())
                    .Where(IsFixtureLive)
                    .Where(fixture => fixture.RugbyTournament != null && fixture.RugbyTournament.Id == tournamentId);

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

            return liveGames.Count();
        }

        public async Task<IEnumerable<RugbyFixture>> GetCompletedFixtures()
        {
            var completedSchedules = _schedulerTrackingRugbyFixtureRepository.Where(
                    f => f.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.SchedulingCompleted).Select(s => s.FixtureId);

            var fixturesCompleted = _rugbyFixturesRepository.Where(f => completedSchedules.Contains(f.Id));

            return await Task.FromResult(fixturesCompleted.ToList());
        }

        public async Task<bool> HasFixtureEnded(long providerFixtureId)
        {
            var fixture =
                await Task.FromResult(
                    _rugbyFixturesRepository.FirstOrDefault(f => f.ProviderFixtureId == providerFixtureId));

            if (fixture != null)
                return fixture.RugbyFixtureStatus == RugbyFixtureStatus.Result;

            // We can't find the fixture in the DB? But still running ingest code?
            // This is a bizzare condition but checking it nonetherless.
            return true;
        }

        public async Task<IEnumerable<RugbyTournament>> GetActiveTournamentsForMatchesInResultsState()
        {
            var tournamentsThatHaveFixturesInResultState = await Task.FromResult(_rugbyFixturesRepository
                        .Where(f => f.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                        .Select(f => f.RugbyTournament).ToList());

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
            return await Task.FromResult(_rugbyFixturesRepository.Where(t => t.RugbyTournament.Id == tournamentId && t.RugbyFixtureStatus == fixtureStatus).OrderByDescending(f => f.StartDateTime));
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

            fixtures = _rugbyFixturesRepository.Where(t => t.RugbyTournament.Id == tournament.Id && t.RugbyFixtureStatus != RugbyFixtureStatus.Result).OrderBy(f => f.StartDateTime);

            return await Task.FromResult(fixtures.ToList());
        }

        public async Task<Guid> GetTournamentId(string tournamentSlug)
        {
            var tournament = _rugbyTournamentRepository.FirstOrDefault(f => f.Slug == tournamentSlug);

            if (tournament == null)
                throw new Exception("Tournament slug does not exist.");

            return await Task.FromResult(tournament.Id);
        }

        public async Task<RugbyTournament> GetTournamentBySlug(string tournamentSlug)
        {
            return await Task.FromResult(_rugbyTournamentRepository.FirstOrDefault(f => f.Slug.Equals(tournamentSlug, StringComparison.InvariantCultureIgnoreCase)));
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
                fixturesInResultsState = await GetTournamentFixtures(tournament.Id, RugbyFixtureStatus.Result);
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

            var fixtures = _rugbyFixturesRepository.Where(f =>
                    ((f.TeamA != null && f.TeamA.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase)) ||
                    (f.TeamB != null && f.TeamB.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase))) &&
                    f.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                    .OrderByDescending(f => f.StartDateTime);

            return await Task.FromResult(fixtures.ToList());
        }

        private async Task<IEnumerable<RugbyFixture>> GetNationalTeamFixtures()
        {
            const string nationalTeamName = "South Africa";

            var fixtures = _rugbyFixturesRepository.Where(f =>
                    ((f.TeamA != null && f.TeamA.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase)) ||
                     (f.TeamB != null && f.TeamB.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase))) &&
                    f.RugbyFixtureStatus != RugbyFixtureStatus.Result)
                    .OrderBy(f => f.StartDateTime);

            return await Task.FromResult(fixtures.ToList());
        }

        public async Task<IEnumerable<RugbyGroupedLog>> GetGroupedLogs(string tournamentSlug)
        {
            var tournament = await GetTournamentBySlug(tournamentSlug);

            var logs = Enumerable.Empty<RugbyGroupedLog>();

            if (tournament != null && tournament.HasLogs)
            {
                logs = _rugbyGroupedLogsRepository
                    .Where(t => t.RugbyTournament.IsEnabled && t.RugbyTournamentId == tournament.Id)
                    .OrderBy(g => g.RugbyLogGroup.Id)
                    .ThenBy(t => t.LogPosition);

                return await Task.FromResult(logs.ToList());
            }

            return await Task.FromResult(logs.ToList());
        }

        public async Task<IEnumerable<RugbyFlatLog>> GetFlatLogs(string tournamentSlug)
        {
            var tournament = await GetTournamentBySlug(tournamentSlug);

            var flatLogs = Enumerable.Empty<RugbyFlatLog>();

            if (tournament != null && tournament.HasLogs)
            {
                flatLogs = _rugbyFlatLogsRepository.Where(t => t.RugbyTournament.IsEnabled && t.RugbyTournamentId == tournament.Id && t.RugbySeason.IsCurrent)
                    .OrderBy(t => t.LogPosition);
            }

            return await Task.FromResult(flatLogs.ToList());
        }

        public Task<List<RugbyFixture>> GetCurrentDayFixturesForActiveTournaments()
        {
            var todayFixtures = _rugbyFixturesRepository
                .Where(f => f.StartDateTime.UtcDateTime.Date == DateTime.UtcNow.Date && f.RugbyTournament.IsEnabled);

            return Task.FromResult(todayFixtures.OrderBy(f => f.StartDateTime).ToList());
        }

        public async Task<RugbyMatchDetailsEntity> GetMatchDetailsByLegacyMatchId(int legacyMatchId, bool omitDisabledFixtures)
        {
            var fixture = GetRugbyFixtureByLegacyMatchId(legacyMatchId);

            if (fixture is null)
            {
                return null;
            }

            if (omitDisabledFixtures)
            {
                if (fixture.IsDisabledOutbound)
                    return null;
            }

            var lineupForFixture = await GetLineupForFixture(fixture.Id);
            var statsForFixture = await GetMatchStatsForFixture(fixture.Id);
            var events = await GetRugbyFixtureEvents(fixture.Id);
            var scorersForFixture = await GetScorersForFixture(fixture.Id);

            events.AddRange(await GetCommentaryAsRugbyMatchEvents(fixture.Id));

            var matchDetails = new RugbyMatchDetailsEntity();

            matchDetails.TeamALineup = lineupForFixture.Where(l => l.RugbyTeamId == (fixture.TeamA?.Id ?? Guid.Empty)).ToList();
            matchDetails.TeamBLineup = lineupForFixture.Where(l => l.RugbyTeamId == (fixture.TeamB?.Id ?? Guid.Empty)).ToList();
            matchDetails.TeamAMatchStatistics = statsForFixture.FirstOrDefault(s => s.RugbyTeamId == (fixture.TeamA?.Id ?? Guid.Empty));
            matchDetails.TeamBMatchStatistics = statsForFixture.FirstOrDefault(s => s.RugbyTeamId == (fixture.TeamB?.Id ?? Guid.Empty));
            matchDetails.RugbyFixture = fixture;
            matchDetails.TeamsLineups = lineupForFixture.OrderBy(p => p.JerseyNumber).ToList();
            matchDetails.MatchEvents = events.OrderByDescending(e => e.GameTimeInSeconds).ThenByDescending(e => e.TimestampCreated).ToList();
            matchDetails.TeamAScorers = scorersForFixture.Where(s => s.RugbyTeamId == (fixture.TeamA?.Id ?? Guid.Empty)).ToList();
            matchDetails.TeamBScorers = scorersForFixture.Where(s => s.RugbyTeamId == (fixture.TeamB?.Id ?? Guid.Empty)).ToList();

            return matchDetails;
        }

        private async Task<List<LegacyRugbyScorerEntity>> GetScorersForFixture(Guid fixtureId)
        {
            var teamScoringEvents = await GetScoringEventsForFixture(fixtureId);

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
                Type = scoringEvent.RugbyEventType.EventName,
                RugbyTeamId = scoringEvent.RugbyTeamId
            })
            .ToList();
        }

        private async Task<List<RugbyMatchEvent>> GetScoringEventsForFixture(Guid fixtureId)
        {
            return await Task.FromResult(_rugbyMatchEventsRepository
                .Where(s => s.RugbyFixture.Id == fixtureId &&
                (s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.PenaltyTryFivePoints ||
                 s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.PenaltyTrySevenPoints ||
                 s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.DropGoalFromMark ||
                 s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Conversion ||
                 s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.DropGoal ||
                 s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Penalty ||
                 s.RugbyEventType.EventCode == LegacyRugbyScoringEventsConstants.Try))
                .ToList());
        }

        private RugbyFixture GetRugbyFixtureByLegacyMatchId(int legacyMatchId)
        {
            return _rugbyFixturesRepository.FirstOrDefault(f => f.LegacyFixtureId == legacyMatchId);
        }

        private async Task<List<RugbyMatchEvent>> GetRugbyFixtureEvents(Guid fixtureId)
        {
            return await Task.FromResult(_rugbyMatchEventsRepository.Where(s => s.RugbyFixture.Id == fixtureId).ToList());
        }

        private async Task<List<RugbyMatchStatistics>> GetMatchStatsForFixture(Guid fixtureId)
        {
            return await Task.FromResult( _rugbyMatchStatisticsRepository.Where(s => s.RugbyFixture.Id == fixtureId).ToList());
        }

        private async Task<List<RugbyPlayerLineup>> GetLineupForFixture(Guid fixtureId)
        {
            return await Task.FromResult(_rugbyPlayerLineupsRepository.Where(l => l.RugbyFixture.Id == fixtureId).ToList());
        }

        private async Task<List<LegacyRugbyMatchEventEntity>> GetCommentaryAsRugbyMatchEvents(Guid fixtureId)
        {
            var matchCommentaries = GetRugbyMatchCommentary(fixtureId);

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

            return await Task.FromResult(matchCommentariesAsEvents);
        }

        private List<RugbyCommentary> GetRugbyMatchCommentary(Guid fixtureId)
        {
            return _rugbyCommentaryRepository.Where(c => c.RugbyFixture.Id == fixtureId).ToList();
        }

        public async Task<IEnumerable<RugbyFixture>> GetPostponedFixtures()
        {
            return await Task.FromResult(_rugbyFixturesRepository.Where(f =>
                (f.RugbyFixtureStatus == RugbyFixtureStatus.PreMatch && f.StartDateTime < (DateTimeOffset.UtcNow - TimeSpan.FromHours(3)))
                || (f.RugbyFixtureStatus == RugbyFixtureStatus.Postponed)));
        }

        public async Task<IEnumerable<RugbyFixture>> GetFixturesNotIngestedYet()
        {
            var pastFixturesIdsNotScheduledYet =
                _schedulerTrackingRugbyFixtureRepository
                    .Where(s => s.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingCompleted &&
                                s.StartDateTime < DateTime.UtcNow.AddHours(-6)).Select(s => s.FixtureId).ToList();

            var fixtures = (_rugbyFixturesRepository.Where(f => pastFixturesIdsNotScheduledYet.Contains(f.Id)))
                                .OrderByDescending(f => f.StartDateTime);

            return await Task.FromResult(fixtures);
        }

        public async Task<IEnumerable<RugbyFixture>> GetPastDaysFixtures(int numberOfDays)
        {
            var now = DateTime.UtcNow;

            var fixtures = _rugbyFixturesRepository.Where(f => now - f.StartDateTime < TimeSpan.FromDays(numberOfDays));

            return await Task.FromResult(fixtures.ToList());
        }
    }
}