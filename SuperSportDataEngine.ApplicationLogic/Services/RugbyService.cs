using System.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class RugbyService : IRugbyService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        private readonly int _numberOfMinutesToCheckForInProgressFixtures;

        private ILoggingService _logger;

        public RugbyService(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            ILoggingService logger)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;

            _logger = logger;

            _numberOfMinutesToCheckForInProgressFixtures =
                int.Parse(ConfigurationManager.AppSettings["NumberOfMinutesToCheckForInProgressFixtures"] ?? "120");
        }

        public async Task<IEnumerable<RugbyTournament>> GetActiveTournaments()
        {
            return await Task.FromResult(
                _publicSportDataUnitOfWork
                    .RugbyTournaments.Where(c => c.IsEnabled).ToList());
        }

        public async Task<IEnumerable<RugbyTournament>> GetCurrentTournaments()
        {
            var tournamentsGuidsThatAreCurrent = await Task.FromResult(
                _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Where(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress).Select(s => s.TournamentId).ToList());

            return await Task.FromResult(
                _publicSportDataUnitOfWork.RugbyTournaments
                    .Where(t => t.IsEnabled && tournamentsGuidsThatAreCurrent.Contains(t.Id))
                    .Select(t => t).ToList());
        }

        public async Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid tournamentId)
        {
            var season =
                await Task.FromResult(_systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons
                    .FirstOrDefault(
                        s => s.TournamentId == tournamentId &&
                        (s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive)));

            return season?.SchedulerStateForManagerJobPolling ?? SchedulerStateForManagerJobPolling.Undefined;
        }

        public async Task<IEnumerable<RugbyTournament>> GetEndedTournaments()
        {
            var tournamentGuids =
                await Task.FromResult(_systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons
                    .Where(
                        season => season.RugbySeasonStatus == RugbySeasonStatus.Ended &&
                        season.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                    .Select(s => s.TournamentId));

            return await Task.FromResult(_publicSportDataUnitOfWork.RugbyTournaments.Where(tournament => tournamentGuids.Contains(tournament.Id)).Select(t => t));
        }

        public async Task<IEnumerable<RugbyTournament>> GetInactiveTournaments()
        {
            return await Task.FromResult(_publicSportDataUnitOfWork.RugbyTournaments.Where(t => t.IsEnabled == false).ToList());
        }

        public async Task<int> GetCurrentProviderSeasonIdForTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            var currentSeason =
                    await Task.FromResult(_publicSportDataUnitOfWork.RugbySeasons
                        .FirstOrDefault(season => season.RugbyTournament.Id == tournamentId && season.IsCurrent));

            return currentSeason?.ProviderSeasonId ?? DateTime.Now.Year;
        }

        public async Task<IEnumerable<RugbyFixture>> GetLiveFixturesForCurrentTournament(CancellationToken cancellationToken, Guid tournamentId)
        {
            var liveGames = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync())
                    .Where(IsFixtureLive)
                    .Where(fixture => fixture.RugbyTournament != null && fixture.RugbyTournament.Id == tournamentId)
                    .Where(f => !f.IsDisabledInbound);

            return liveGames;
        }

        private bool IsFixtureLive(RugbyFixture fixture)
        {
            var fixtureSchedule = _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.All().ToList()
                    .FirstOrDefault(s => s.FixtureId == fixture.Id);

            if (fixtureSchedule == null)
                return false;

            return fixtureSchedule.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.LivePolling ||
                   fixtureSchedule.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.PreLivePolling;
        }

        public async Task<int> GetLiveFixturesCount(CancellationToken cancellationToken)
        {
            var liveGames = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync())
                .Where(IsFixtureLive)
                .Where(f => !f.IsDisabledInbound);

            return liveGames.Count();
        }

        public async Task<IEnumerable<RugbyFixture>> GetCompletedFixtures()
        {
            var completedSchedules = _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Where(
                    f => f.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.SchedulingCompleted).Select(s => s.FixtureId);

            var fixturesCompleted = _publicSportDataUnitOfWork.RugbyFixtures.Where(f => completedSchedules.Contains(f.Id));

            return await Task.FromResult(fixturesCompleted.ToList());
        }

        public async Task<bool> HasFixtureEnded(long providerFixtureId)
        {
            var fixture =
                await Task.FromResult(
                    _publicSportDataUnitOfWork.RugbyFixtures.FirstOrDefault(f => f.ProviderFixtureId == providerFixtureId));

            if (fixture != null)
                return fixture.RugbyFixtureStatus == RugbyFixtureStatus.Result;

            // We can't find the fixture in the DB? But still running ingest code?
            // This is a bizzare condition but checking it nonetherless.
            return true;
        }

        public async Task<IEnumerable<RugbyTournament>> GetActiveTournamentsForMatchesInResultsState()
        {
            var tournamentsThatHaveFixturesInResultState = await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures
                        .Where(f => f.RugbyFixtureStatus == RugbyFixtureStatus.Result && f.RugbyTournament.IsEnabled)
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
            var disabledTournamentsIds = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).Where(t => t.IsEnabled == false).Select(t => t.Id);
            var itemsToDelete = (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.AllAsync()).Where(t => disabledTournamentsIds.Contains(t.TournamentId)).ToList();

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.DeleteRange(itemsToDelete);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task CleanupSchedulerTrackingRugbySeasonsTable()
        {
            var endedSeasons = (await _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.AllAsync())
                .Where(
                    s => s.RugbySeasonStatus == RugbySeasonStatus.Ended).ToList();

            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.DeleteRange(endedSeasons);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task CleanupSchedulerTrackingRugbyFixturesTable()
        {
            var nowMinus6Months = DateTimeOffset.UtcNow - TimeSpan.FromDays(180);
            var itemsToDelete = (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync())
                .Where(
                    f => (f.RugbyFixtureStatus == RugbyFixtureStatus.Result || f.SchedulerStateFixtures == SchedulerStateForRugbyFixturePolling.SchedulingCompleted) && f.StartDateTime < nowMinus6Months).ToList();

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.DeleteRange(itemsToDelete);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(Guid tournamentId, RugbyFixtureStatus fixtureStatus)
        {
            // The logic is now corrected.
            // We are serving out on the fixtures endpoint all fixtures that are 
            // upcoming and In Progress(this is based on a time period, 
            // only check fixtures that started two hours ago. This is configurable.)
            // The issue Darren was having was:
            // Fixture started at 1:45am SAST and ended just after 2:00am SAST
            // Due to UTC Dates, the fixtures endpoint at 2:01am SAST would not 
            // show the fixture In Progress since the days would have changed.

            var today = DateTime.UtcNow - TimeSpan.FromMinutes(120);

            var fixtures = await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures.Where(t => 
                        !t.IsDisabledOutbound && 
                        t.RugbyTournament.Id == tournamentId && 
                        t.RugbyFixtureStatus == fixtureStatus &&
                        t.RugbySeason != null &&
                        t.RugbySeason.IsCurrent).OrderByDescending(f => f.StartDateTime));

            if (fixtureStatus == RugbyFixtureStatus.Result)
                return fixtures;

            return fixtures.Where(t => t.StartDateTime >= today);
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
            // The logic is now corrected.
            // We are serving out on the fixtures endpoint all fixtures that are 
            // upcoming and In Progress(this is based on a time period, 
            // only check fixtures that started two hours ago. This is configurable.)
            // The issue Darren was having was:
            // Fixture started at 1:45am SAST and ended just after 2:00am SAST
            // Due to UTC Dates, the fixtures endpoint at 2:01am SAST would not 
            // show the fixture In Progress since the days would have changed.

            var today = DateTime.UtcNow - TimeSpan.FromMinutes(120);

            fixtures = _publicSportDataUnitOfWork.RugbyFixtures.Where(t =>
                    !t.IsDisabledOutbound &&
                    t.RugbyTournament.Id == tournament.Id &&
                    t.RugbyFixtureStatus != RugbyFixtureStatus.Result &&
                    t.StartDateTime >= today &&
                    t.RugbySeason != null &&
                    t.RugbySeason.IsCurrent).OrderBy(f => f.StartDateTime);

            if (!tournamentSlug.Equals("sevens")) return await Task.FromResult(fixtures.ToList());

            fixtures = fixtures.Where(f => 
                        f.RugbySeason != null && 
                        f.RoundNumber == (f.RugbySeason.CurrentRoundNumberCmsOverride ?? f.RugbySeason.CurrentRoundNumber));

            return await Task.FromResult(fixtures.ToList());
        }

        public async Task<Guid> GetTournamentId(string tournamentSlug)
        {
            var tournament = await GetTournamentBySlug(tournamentSlug);

            if (tournament == null)
            {
                return await Task.FromResult(Guid.Empty);
            }

            return await Task.FromResult(tournament.Id);
        }

        public async Task<RugbyTournament> GetTournamentBySlug(string tournamentSlug)
        {
            var tournament =
                await Task.FromResult(_publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(f =>
                    f.Slug.Equals(tournamentSlug, StringComparison.InvariantCultureIgnoreCase)));

            return tournament;
        }

        public async Task<IEnumerable<RugbyFixture>> GetRecentResultsFixtures(int maxCount)
        {
            var recentFixturesInResultsState = await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures.Where(
                x => !x.IsDisabledOutbound &&
                x.RugbyFixtureStatus == RugbyFixtureStatus.Result &&
                x.RugbyTournament.IsEnabled)
                .OrderByDescending(f => f.StartDateTime)
                .Take(maxCount));

            return recentFixturesInResultsState;
        }

        public async Task<IEnumerable<RugbyTournament>> GetTournamentsForJustEndedFixtures()
        {
            var rugbyFixtures =
                (await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures.Where(
                    x => !x.IsDisabledInbound &&
                         x.RugbyFixtureStatus == RugbyFixtureStatus.Result &&
                         x.RugbyTournament.IsEnabled &&
                         x.RugbyTournament.IsEnabled)))
                .ToList();

            var rugbyTournaments = new List<RugbyTournament>();

            foreach (var fixture in rugbyFixtures)
            {
                var utcNowDateTime = DateTimeOffset.UtcNow.DateTime;
                var isPlayedToday = fixture.StartDateTime.Year == utcNowDateTime.Year &&
                                    fixture.StartDateTime.Month == utcNowDateTime.Month &&
                                    fixture.StartDateTime.Day == utcNowDateTime.Day;

                if (!isPlayedToday) continue;

                const int gameTimeEstimateInMinutes = 95;
                var isMatchOver = utcNowDateTime >
                                  fixture.StartDateTime.DateTime + TimeSpan.FromMinutes(gameTimeEstimateInMinutes);
                if (!isMatchOver) continue;

                if (rugbyTournaments.Any(t => t.Id == fixture.RugbyTournament.Id)) continue;

                rugbyTournaments.Add(fixture.RugbyTournament);
            }

            return rugbyTournaments;
        }

        public async Task<IEnumerable<RugbyFixture>> GetTournamentResults(string tournamentSlug)
        {
            if (IsNationalTeamSlug(tournamentSlug))
            {
                return await GetNationalTeamResults();
            }

            var tournament = await GetTournamentBySlug(tournamentSlug);

            var fixturesInResultsState = Enumerable.Empty<RugbyFixture>();

            if (tournament == null)
            {
                return fixturesInResultsState;
            }

            fixturesInResultsState = await GetTournamentFixtures(tournament.Id, RugbyFixtureStatus.Result);

            if (!tournamentSlug.Equals("sevens")) return await Task.FromResult(fixturesInResultsState.ToList());

            var season =
                _publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(s =>
                    s.IsCurrent &&
                    s.RugbyTournament.Id == tournament.Id);

            fixturesInResultsState = fixturesInResultsState
                    .Where(f =>
                        season != null &&
                        f.RoundNumber == (season.CurrentRoundNumberCmsOverride ?? season.CurrentRoundNumber));

            return fixturesInResultsState;
        }

        public async Task<IEnumerable<RugbyFixture>> GetUpcomingFixtures()
        {
            var today = DateTime.UtcNow;

            var fixtures = _publicSportDataUnitOfWork.RugbyFixtures.Where(t =>
                !t.IsDisabledOutbound &&
                t.RugbyTournament.IsEnabled &&
                t.RugbyFixtureStatus != RugbyFixtureStatus.Result &&
                t.StartDateTime >= today)
                .OrderBy(f => f.StartDateTime);

            return await Task.FromResult(fixtures.ToList());
        }

        public bool IsNationalTeamSlug(string slug)
        {
            var result = slug.Equals("springboks", StringComparison.CurrentCultureIgnoreCase);

            return result;
        }

        public async Task<RugbySeason> GetCurrentRugbySeasonForTournament(string category)
        {
            return await Task.FromResult(_publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(
                s => s.RugbyTournament.Slug == category && s.IsCurrent));
        }

        private async Task<IEnumerable<RugbyFixture>> GetNationalTeamResults()
        {
            const string nationalTeamName = "South Africa";

            var fixtures = _publicSportDataUnitOfWork.RugbyFixtures.Where(f =>
                    ((f.TeamA != null && f.TeamA.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase)) ||
                    (f.TeamB != null && f.TeamB.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase))) &&
                    f.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                    .OrderByDescending(f => f.StartDateTime);

            return await Task.FromResult(fixtures.ToList());
        }

        private async Task<IEnumerable<RugbyFixture>> GetNationalTeamFixtures()
        {
            const string nationalTeamName = "South Africa";
            var today = DateTime.UtcNow;

            var fixtures = _publicSportDataUnitOfWork.RugbyFixtures.Where(f =>
                    ((f.TeamA != null && f.TeamA.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase)) ||
                     (f.TeamB != null && f.TeamB.Name.Equals(nationalTeamName, StringComparison.InvariantCultureIgnoreCase))) &&
                    f.RugbyFixtureStatus != RugbyFixtureStatus.Result &&
                    f.StartDateTime >= today)
                    .OrderBy(f => f.StartDateTime);

            return await Task.FromResult(fixtures.ToList());
        }

        public async Task<IEnumerable<RugbyGroupedLog>> GetGroupedLogs(string tournamentSlug)
        {
            var logs = Enumerable.Empty<RugbyGroupedLog>();

            if (IsNationalTeamSlug(tournamentSlug))
                return logs;

            var tournament = await GetTournamentBySlug(tournamentSlug);

            if (tournament != null && tournament.HasLogs)
            {
                logs = _publicSportDataUnitOfWork.RugbyGroupedLogs
                    .Where(t => 
                        t.RugbyTournament.IsEnabled &&
                        t.RugbyTournamentId == tournament.Id &&
                        t.RugbySeason.IsCurrent &&
                        t.RoundNumber == (t.RugbySeason.CurrentRoundNumberCmsOverride ?? t.RugbySeason.CurrentRoundNumber) &&
                        t.RugbyLogGroup.IsCoreGroup)
                    .OrderBy(g => g.RugbyLogGroup.GroupName).ThenBy(t => t.LogPosition);

                return await Task.FromResult(logs.ToList());
            }

            return await Task.FromResult(logs.ToList());
        }

        public async Task<IEnumerable<RugbyFlatLog>> GetFlatLogs(string tournamentSlug)
        {
            var flatLogs = Enumerable.Empty<RugbyFlatLog>();

            if (IsNationalTeamSlug(tournamentSlug))
                return flatLogs;

            var tournament = await GetTournamentBySlug(tournamentSlug);

            if (tournament != null && tournament.HasLogs)
            {
                flatLogs = _publicSportDataUnitOfWork.RugbyFlatLogs.Where(t => 
                        t.RugbyTournament.IsEnabled && 
                        t.RugbyTournamentId == tournament.Id && 
                        t.RugbySeason.IsCurrent &&
                        t.RoundNumber == (t.RugbySeason.CurrentRoundNumberCmsOverride ?? t.RugbySeason.CurrentRoundNumber))
                    .OrderBy(t => t.LogPosition);
            }

            return await Task.FromResult(flatLogs.ToList());
        }

        public async Task<List<RugbyFixture>> GetCurrentDayFixturesForActiveTournaments()
        {
            var minDateTime = DateTime.UtcNow.Date - TimeSpan.FromMinutes(_numberOfMinutesToCheckForInProgressFixtures);
            var tomorrow = DateTime.UtcNow.Date + TimeSpan.FromDays(1);

            var todayFixtures = _publicSportDataUnitOfWork.RugbyFixtures
               .Where(f => 
                    f.StartDateTime > minDateTime && 
                    f.StartDateTime < tomorrow &&
                    f.RugbyTournament.IsEnabled &&
                    !f.IsDisabledOutbound)
                .OrderBy(f => f.StartDateTime);

            return await Task.FromResult(todayFixtures.ToList());
        }

        public async Task<IEnumerable<RugbyFixture>> GetCurrentDayFixturesForTournament(string tournamentSlug)
        {
            var tournament = await GetTournamentBySlug(tournamentSlug);

            if (tournament is null) return Enumerable.Empty<RugbyFixture>();

            var minDateTime = DateTime.UtcNow.Date - TimeSpan.FromMinutes(_numberOfMinutesToCheckForInProgressFixtures);
            var tomorrow = DateTime.UtcNow.Date + TimeSpan.FromDays(1);

            var todayFixtures = _publicSportDataUnitOfWork.RugbyFixtures
                .Where(f => 
                    f.StartDateTime > minDateTime &&
                    f.StartDateTime < tomorrow &&
                    f.RugbyTournament.IsEnabled &&
                    f.RugbyTournament.Id == tournament.Id &&
                    !f.IsDisabledOutbound)
                .OrderBy(f => f.StartDateTime);

            return await Task.FromResult(todayFixtures.ToList());
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
            var scorersForFixture = await GetScorersForFixture(events, fixture.Id);

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

        private async Task<List<LegacyRugbyScorerEntity>> GetScorersForFixture(IEnumerable<RugbyMatchEvent> events, Guid fixtureId)
        {
            var teamScoringEvents = await GetScoringEventsForFixture(events, fixtureId);

            return teamScoringEvents.Select(GetLegacyRugbyScorerEntity)
            .ToList();
        }

        private LegacyRugbyScorerEntity GetLegacyRugbyScorerEntity(RugbyMatchEvent scoringEvent)
        {
            // If the player is null, this is a team scoring event.
            if (scoringEvent.RugbyPlayer1 == null)
            {
                return new LegacyRugbyScorerEntity
                {
                    CombinedName = "",
                    DisplayName = "",
                    EventId = scoringEvent.RugbyEventType.EventCode,
                    Name = "",
                    NickName = null,
                    PersonId = 0,
                    Surname = "",
                    Time = scoringEvent.GameTimeInMinutes.ToString(),
                    Type = scoringEvent.RugbyEventType.EventName,
                    RugbyTeamId = scoringEvent.RugbyTeamId
                };
            }

            return new LegacyRugbyScorerEntity
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
            };
        }

        private async Task<List<RugbyMatchEvent>> GetScoringEventsForFixture(IEnumerable<RugbyMatchEvent> events, Guid fixtureId)
        {
            return await Task.FromResult(events
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
            return _publicSportDataUnitOfWork.RugbyFixtures.FirstOrDefault(f => f.LegacyFixtureId == legacyMatchId);
        }

        private async Task<List<RugbyMatchEvent>> GetRugbyFixtureEvents(Guid fixtureId)
        {
            return await Task.FromResult(_publicSportDataUnitOfWork.RugbyMatchEvents.Where(s => s.RugbyFixture.Id == fixtureId).ToList());
        }

        private async Task<List<RugbyMatchStatistics>> GetMatchStatsForFixture(Guid fixtureId)
        {
            return await Task.FromResult( _publicSportDataUnitOfWork.RugbyMatchStatistics.Where(s => s.RugbyFixture.Id == fixtureId).ToList());
        }

        private async Task<List<RugbyPlayerLineup>> GetLineupForFixture(Guid fixtureId)
        {
            return await Task.FromResult(_publicSportDataUnitOfWork.RugbyPlayerLineups.Where(l => l.RugbyFixture.Id == fixtureId).ToList());
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
            return _publicSportDataUnitOfWork.RugbyCommentaries.Where(c => c.RugbyFixture.Id == fixtureId).ToList();
        }

        public async Task<IEnumerable<RugbyFixture>> GetPostponedFixtures()
        {
            var threeHoursAgo = (DateTimeOffset.UtcNow - TimeSpan.FromHours(3));

            return await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures.Where(f =>
                (f.RugbyFixtureStatus == RugbyFixtureStatus.PreMatch && f.StartDateTime < threeHoursAgo)
                || (f.RugbyFixtureStatus == RugbyFixtureStatus.Postponed)));
        }

        public async Task<IEnumerable<RugbyFixture>> GetFixturesNotIngestedYet()
        {
            var hoursBeforeNow = DateTime.UtcNow.AddHours(-6);

            var pastFixturesIdsNotScheduledYet =
                _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures
                    .Where(s => s.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingCompleted &&
                                s.StartDateTime < hoursBeforeNow).Select(s => s.FixtureId).ToList();

            var fixtures = (_publicSportDataUnitOfWork.RugbyFixtures.Where(f => pastFixturesIdsNotScheduledYet.Contains(f.Id)))
                                .OrderByDescending(f => f.StartDateTime);

            return await Task.FromResult(fixtures);
        }

        public async Task<IEnumerable<RugbyFixture>> GetPastDaysFixtures(int numberOfDays)
        {
            var fewDaysAgo = DateTime.UtcNow.Date.Subtract(TimeSpan.FromDays(numberOfDays));
            var today = DateTime.Today;

            var fixtures = _publicSportDataUnitOfWork.RugbyFixtures.Where(f => f.StartDateTime < today && f.StartDateTime >= fewDaysAgo);

            return await Task.FromResult(fixtures.ToList());
        }

        public async Task<IEnumerable<RugbyPlayerStatistics>> GetTournamentTryScorers(string tournamentSlug)
        {
            var tournamentId = await GetTournamentId(tournamentSlug);

            var players = _publicSportDataUnitOfWork.RugbyPlayerStatistics
                .Where(s => s.RugbyTournament.Id == tournamentId)
                .OrderByDescending(p => p.TriesScored)
                .ToList();

            if (!players.Any()) return players;

            var currentPlayerTries = players.First().TriesScored;
            var currentPlayerRank = 1;
            foreach (var player in players)
            {
                var shouldIncrementRank = player.TriesScored < currentPlayerTries;
                if (shouldIncrementRank)
                {
                    currentPlayerRank++;
                    player.Rank = currentPlayerRank;
                    currentPlayerTries = player.TriesScored;
                }

                player.Rank = currentPlayerRank;
            }

            return players;
        }

        public async Task<IEnumerable<RugbyPlayerStatistics>> GetTournamentPointsScorers(string tournamentSlug)
        {
            var tournamentId = await GetTournamentId(tournamentSlug);

            var players = _publicSportDataUnitOfWork.RugbyPlayerStatistics
                .Where(s => s.RugbyTournament.Id == tournamentId)
                .OrderByDescending(s => s.TotalPoints)
                .ToList();

            if (!players.Any()) return players;

            var currentPlayerPoints = players.First().TotalPoints;
            var currentPlayerRank = 1;
            foreach (var player in players)
            {
                var shouldIncrementRank = player.TotalPoints < currentPlayerPoints;
                if (shouldIncrementRank)
                {
                    currentPlayerPoints = player.TotalPoints;
                    currentPlayerRank++;
                    player.Rank = currentPlayerRank;
                }

                player.Rank = currentPlayerRank;
            }

            return players;
        }

        public async Task<bool> IsCategoryInvalid(string category)
        {
            return
               await GetTournamentId(category) == Guid.Empty &&
                !IsNationalTeamSlug(category);
        }
    }
}
