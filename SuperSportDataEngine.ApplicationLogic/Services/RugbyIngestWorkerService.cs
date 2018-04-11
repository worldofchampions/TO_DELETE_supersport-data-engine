namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using Boundaries.ApplicationLogic.Interfaces;
    using Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
    using Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs;
    using Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats;
    using Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
    using Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using Constants;
    using Constants.Providers;
    using Extensions;
    using Helpers;
    using Common.Logging;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly ILoggingService _logger;
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;
        private readonly IRugbyService _rugbyService;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly int _halfTimeMaximumDurationInMinutes;

        public RugbyIngestWorkerService(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            ILoggingService logger,
            IStatsProzoneRugbyIngestService statsProzoneIngestService,
            IMongoDbRugbyRepository mongoDbRepository,
            IRugbyService rugbyService)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _logger = logger;
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
            _rugbyService = rugbyService;

            _halfTimeMaximumDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["HalfTimeMaximumDurationInMinutes"] ?? "10");
        }

        public async Task IngestReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var entitiesResponse =
                await _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            if (entitiesResponse == null)
                throw new NullReferenceException("Provider request failed due to timeout.");

            await PersistVenuesInRepository(cancellationToken, entitiesResponse);
            await PersistTeamsInRepository(cancellationToken, entitiesResponse);
            await PersistTournamentsInRepository(cancellationToken, entitiesResponse);
            await PersistPlayerDataInRepository(cancellationToken, entitiesResponse);
            await PersistTournamentSeasonsInRepository(cancellationToken);
            await PersistTournamentCurrentAndUpcomingSeasonsInRepository(cancellationToken);

            _mongoDbRepository.SaveEntities(entitiesResponse);
        }

        private async Task PersistPlayerDataInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (entitiesResponse.Entities != null && entitiesResponse.Entities.players == null)
                return;

            var playersAlreadyInDb = await _publicSportDataUnitOfWork.RugbyPlayers.AllAsync();
            var players = playersAlreadyInDb as IList<RugbyPlayer> ?? playersAlreadyInDb.ToList();

            if (entitiesResponse.Entities != null)
            {
                foreach (var player in entitiesResponse.Entities.players)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var playerInDb = players.FirstOrDefault(p => p.ProviderPlayerId == player.id);

                    var newPlayer = new RugbyPlayer()
                    {
                        FirstName = null,
                        LastName = null,
                        FullName = player.name,
                        ProviderPlayerId = player.id,
                        DataProvider = DataProvider.StatsProzone
                    };

                    if (playerInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyPlayers.Add(newPlayer);
                    }
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistRugbyTournamentsInSchedulerTrackingRugbyTournamentTable(CancellationToken cancellationToken)
        {
            var activeTournaments = await _rugbyService.GetActiveTournaments();
            var trackingTournamentsAlreadyInDb = await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.AllAsync();
            var tournaments = trackingTournamentsAlreadyInDb as IList<SchedulerTrackingRugbyTournament> ?? trackingTournamentsAlreadyInDb.ToList();

            foreach (var tournament in activeTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var seasonInDb =
                    _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons
                        .Where(s => s.TournamentId == tournament.Id && s.RugbySeasonStatus == RugbySeasonStatus.InProgress).FirstOrDefault();

                if (seasonInDb != null)
                {
                    var tournamentInDb =
                        tournaments.FirstOrDefault(t => t.TournamentId == tournament.Id && t.SeasonId == seasonInDb.SeasonId);

                    var newTournament = new SchedulerTrackingRugbyTournament()
                    {
                        SeasonId = seasonInDb.SeasonId,
                        TournamentId = tournament.Id,
                        SchedulerStateLogs = SchedulerStateForRugbyLogPolling.NotRunning,
                        SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning
                    };

                    if (tournamentInDb == null)
                    {
                        _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Add(newTournament);
                    }
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistTeamsInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (entitiesResponse.Entities.teams == null)
                return;

            var teamsAlreadyInDb = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).ToList();

            foreach (var team in entitiesResponse.Entities.teams)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamInDb = teamsAlreadyInDb.FirstOrDefault(t => t.ProviderTeamId == team.id);

                if (teamInDb == null)
                {
                    var newTeam = new RugbyTeam()
                    {
                        ProviderTeamId = team.id,
                        Name = team.name,
                        LogoUrl = team.TeamLogoURL,
                        Abbreviation = team.TeamAbbrev,
                        DataProvider = DataProvider.StatsProzone
                    };
                    _publicSportDataUnitOfWork.RugbyTeams.Add(newTeam);
                }
                else
                {
                    teamInDb.Name = team.name;
                    teamInDb.ProviderTeamId = team.id;

                    _publicSportDataUnitOfWork.RugbyTeams.Update(teamInDb);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistVenuesInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (entitiesResponse.Entities != null && entitiesResponse.Entities.venues == null)
                return;

            var venuesAlreadyInDb = (await _publicSportDataUnitOfWork.RugbyVenues.AllAsync()).ToList();

            if (entitiesResponse.Entities != null)
            {
                foreach (var venue in entitiesResponse.Entities.venues)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    // Lookup in db
                    var venueInDb = venuesAlreadyInDb.FirstOrDefault(v => v.ProviderVenueId == venue.id);

                    if (venueInDb == null)
                    {
                        var newVenue = new RugbyVenue()
                        {
                            ProviderVenueId = venue.id,
                            Name = venue.name,
                            DataProvider = DataProvider.StatsProzone
                        };
                        _publicSportDataUnitOfWork.RugbyVenues.Add(newVenue);
                    }
                    else
                    {
                        venueInDb.Name = venue.name;
                        venueInDb.ProviderVenueId = venue.id;

                        _publicSportDataUnitOfWork.RugbyVenues.Update(venueInDb);
                    }
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistTournamentSeasonsInRepository(CancellationToken cancellationToken)
        {
            var activeTournaments = await _rugbyService.GetActiveTournaments();
            foreach (var tournament in activeTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var currentSeason = _publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(s =>
                        s.RugbyTournament.ProviderTournamentId == tournament.ProviderTournamentId &&
                        s.IsCurrent);

                if (currentSeason == null)
                    continue;

                await IngestSeason(cancellationToken, tournament, currentSeason.ProviderSeasonId);
            }
        }

        private async Task PersistTournamentCurrentAndUpcomingSeasonsInRepository(CancellationToken cancellationToken)
        {
            var activeTournaments = await _rugbyService.GetActiveTournaments();
            foreach (var tournament in activeTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var yearNow = DateTime.UtcNow.Year;
                await IngestSeason(cancellationToken, tournament, yearNow);
                await IngestSeason(cancellationToken, tournament, yearNow + 1);
            }
        }

        private async Task PersistPastTournamentSeasonsInRepository(CancellationToken cancellationToken)
        {
            var activeTournaments = await _rugbyService.GetActiveTournaments();
            foreach (var tournament in activeTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                await IngestPastSeason(cancellationToken, tournament, DateTime.Now.Year);
                await IngestPastSeason(cancellationToken, tournament, DateTime.Now.Year - 1);
                await IngestPastSeason(cancellationToken, tournament, DateTime.Now.Year - 2);
            }
        }

        private async Task IngestPastSeason(CancellationToken cancellationToken, RugbyTournament tournament, int seasonId)
        {
            await IngestSeason(cancellationToken, tournament, seasonId);

            var fixtures =
                    await _statsProzoneIngestService.IngestFixturesForTournament(
                        tournament, seasonId, cancellationToken);

            if (fixtures == null)
                throw new NullReferenceException("Provider request failed due to timeout.");

            await PersistFixturesData(cancellationToken, fixtures);

            await PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(cancellationToken, fixtures);
            await PersistRugbyTournamentsInSchedulerTrackingRugbyTournamentTable(cancellationToken);

            _mongoDbRepository.Save(fixtures);
        }

        private async Task IngestSeason(CancellationToken cancellationToken, RugbyTournament tournament, int year)
        {
            var season = await _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, year);
            if (season == null)
            {
                return;
            }

            var providerTournamentId = season.RugbySeasons.competitionId;

            if (season.RugbySeasons.season.Count == 0)
                return;

            var providerSeasonId = season.RugbySeasons.season.First().id;

            var isSeasonCurrentlyActive = season.RugbySeasons.season.First().currentSeason;
            var rounds = season.RugbySeasons.season.First().rounds;
            var activeRound = rounds.FirstOrDefault(r => r.currentRound);

            var currentRoundNumber = -1;

            if (activeRound != null)
            {
                currentRoundNumber = activeRound.roundNumber;
            }

            var seasonsInDb = _publicSportDataUnitOfWork.RugbySeasons.All().ToList();

            var seasonEntry =
                    seasonsInDb
                    .FirstOrDefault(s => s.RugbyTournament.ProviderTournamentId == providerTournamentId && s.ProviderSeasonId == providerSeasonId);

            var tournamentInDb = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == providerTournamentId);

            var newEntry = new RugbySeason()
            {
                ProviderSeasonId = providerSeasonId,
                RugbyTournament = tournamentInDb,
                IsCurrent = isSeasonCurrentlyActive,
                Name = season.RugbySeasons.season.First().name,
                DataProvider = DataProvider.StatsProzone,
            };

            if (currentRoundNumber != -1)
            {
                newEntry.CurrentRoundNumber = currentRoundNumber;
            }

            // Not in repo?
            if (seasonEntry == null)
            {
                _publicSportDataUnitOfWork.RugbySeasons.Add(newEntry);
            }
            else
            {
                seasonEntry.StartDateTime = newEntry.StartDateTime;
                seasonEntry.Name = newEntry.Name;

                if (currentRoundNumber != -1 &&
                    seasonEntry.CurrentRoundNumber <= currentRoundNumber)
                {
                    seasonEntry.CurrentRoundNumber = currentRoundNumber;
                }

                CheckIfAllFixturesForCurrentRoundHasEnded(seasonEntry);

                _publicSportDataUnitOfWork.RugbySeasons.Update(seasonEntry);
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async void CheckIfAllFixturesForCurrentRoundHasEnded(RugbySeason seasonEntry)
        {
            var roundNumber = seasonEntry.CurrentRoundNumber;
            var fixturesForRoundResponse = 
                    await _statsProzoneIngestService.IngestRoundFixturesForTournament(
                        seasonEntry.RugbyTournament.ProviderTournamentId, 
                        seasonEntry.ProviderSeasonId,
                        roundNumber);

            if (fixturesForRoundResponse == null)
                return;

            var roundFixtures = fixturesForRoundResponse.RoundFixtures.roundFixtures.FirstOrDefault();
            if (roundFixtures == null)
                return;

            var doesRoundHaveUnendedFixtures =
                    roundFixtures.gameFixtures
                        .Any(f => 
                            GetFixtureStatusFromProviderFixtureState(
                                null, f.gameStateName) != RugbyFixtureStatus.Result);

            // If the current round has all the fixtures as completed.
            // Increase the round number.
            if(!doesRoundHaveUnendedFixtures)
            {
                seasonEntry.CurrentRoundNumber++;
            }
        }

        public async Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await PersistTournamentSeasonsInRepository(cancellationToken);

            var activeTournaments =
                await _rugbyService.GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var activeSeasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var fixtures =
                    await _statsProzoneIngestService.IngestFixturesForTournament(
                        tournament, activeSeasonId, cancellationToken);

                if (fixtures == null)
                    continue;

                await PersistFixturesData(cancellationToken, fixtures);

                await PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(cancellationToken, fixtures);
                await PersistRugbyTournamentsInSchedulerTrackingRugbyTournamentTable(cancellationToken);

                _mongoDbRepository.Save(fixtures);
            }
        }

        private async Task PersistFlatLogs(CancellationToken cancellationToken, RugbyFlatLogsResponse flatLogsResponse)
        {
            var tournamentId = flatLogsResponse.RugbyFlatLogs.competitionId;
            var seasonId = flatLogsResponse.RugbyFlatLogs.seasonId;
            var roundNumber = flatLogsResponse.RugbyFlatLogs.roundNumber;

            var laddersAlreadyInDb = (_publicSportDataUnitOfWork.RugbyFlatLogs.Where(
                l => l.RugbyTournament.ProviderTournamentId == tournamentId &&
                l.RugbySeason.ProviderSeasonId == seasonId &&
                l.RoundNumber == roundNumber)).ToList();

            var itemsToRemove = laddersAlreadyInDb;
            if (flatLogsResponse.RugbyFlatLogs.ladderposition == null)
                return;

            foreach (var position in flatLogsResponse.RugbyFlatLogs.ladderposition)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var team = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).FirstOrDefault(t => t.ProviderTeamId == position.teamId);
                if (team == null) continue;

                var ladderEntryInDb =
                    laddersAlreadyInDb.FirstOrDefault(
                        l => l.RugbyTournament.ProviderTournamentId == tournamentId &&
                             l.RugbySeason.ProviderSeasonId == seasonId &&
                             l.RoundNumber == roundNumber &&
                             l.RugbyTeamId == team.Id);

                var tournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == tournamentId);
                var season = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).FirstOrDefault(s => tournament != null && (s.RugbyTournament.Id == tournament.Id && s.ProviderSeasonId == seasonId));

                if (season == null) continue;
                if (tournament == null) continue;

                var newLadderEntry = new RugbyFlatLog()
                {
                    RugbyTournamentId = tournament.Id,
                    RugbyTournament = tournament,
                    RoundNumber = roundNumber,
                    LogPosition = position.position,
                    BonusPoints = position.bonusPoints,
                    GamesDrawn = position.gamesPlayed - position.losses - position.wins,
                    GamesLost = position.losses,
                    GamesWon = position.wins,
                    GamesPlayed = position.gamesPlayed,
                    PointsAgainst = position.pointsAgainst,
                    PointsDifference = position.pointsDifference,
                    PointsFor = position.pointsFor,
                    TournamentPoints = position.competitionPoints,
                    RugbySeason = season,
                    RugbySeasonId = season.Id,
                    RugbyTeam = team,
                    RugbyTeamId = team.Id,
                    TriesAgainst = position.triesAgainst,
                    TriesFor = position.triesAgainst
                };

                if (ladderEntryInDb == null)
                {
                    _publicSportDataUnitOfWork.RugbyFlatLogs.Add(newLadderEntry);
                }
                else
                {
                    ladderEntryInDb.LogPosition = position.position;
                    ladderEntryInDb.BonusPoints = position.bonusPoints;
                    ladderEntryInDb.GamesDrawn = position.gamesPlayed - position.losses - position.wins;
                    ladderEntryInDb.GamesLost = position.losses;
                    ladderEntryInDb.GamesWon = position.wins;
                    ladderEntryInDb.GamesPlayed = position.gamesPlayed;
                    ladderEntryInDb.PointsAgainst = position.pointsAgainst;
                    ladderEntryInDb.PointsDifference = position.pointsDifference;
                    ladderEntryInDb.PointsFor = position.pointsFor;
                    ladderEntryInDb.TournamentPoints = position.competitionPoints;
                    ladderEntryInDb.TriesAgainst = position.triesAgainst;
                    ladderEntryInDb.TriesFor = position.triesAgainst;

                    _publicSportDataUnitOfWork.RugbyFlatLogs.Update(ladderEntryInDb);
                    itemsToRemove.Remove(ladderEntryInDb);
                }
            }

            _publicSportDataUnitOfWork.RugbyFlatLogs.DeleteRange(itemsToRemove);
            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistFixturesData(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, fixtures);
            await PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(fixtures);

            _mongoDbRepository.Save(fixtures);
        }

        private async Task PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(RugbyFixturesResponse fixtures)
        {
            var fixturesAlreadyInDb = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync()).ToList();

            foreach (var roundFixtures in fixtures.Fixtures.roundFixtures)
            {
                foreach (var fixture in roundFixtures.gameFixtures)
                {
                    var fixtureInDb = fixturesAlreadyInDb.FirstOrDefault(f => f.ProviderFixtureId == fixture.gameId);
                    if (fixtureInDb == null) continue;

                    {
                        var fixtureGuid = fixtureInDb.Id;
                        var tournamentGuid = fixtureInDb.RugbyTournament.Id;

                        var fixtureSchedule =
                            (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync())
                            .FirstOrDefault(
                                f => f.FixtureId == fixtureGuid && f.TournamentId == tournamentGuid);

                        if (fixtureSchedule == null)
                        {
                            var newFixtureSchedule = new SchedulerTrackingRugbyFixture()
                            {
                                FixtureId = fixtureGuid,
                                TournamentId = tournamentGuid,
                                StartDateTime = fixtureInDb.StartDateTime,
                                EndedDateTime = DateTimeOffset.MinValue,
                                RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixtureInDb, fixture.gameStateName),
                                SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted,
                                IsJobRunning = false
                            };

                            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(newFixtureSchedule);
                        }
                        else
                        {
                            // If the schedule already is in the system repo
                            // we need to update the status of the game.
                            var gameState = GetFixtureStatusFromProviderFixtureState(fixtureInDb, fixture.gameStateName);
                            fixtureSchedule.RugbyFixtureStatus = gameState;
                            fixtureSchedule.StartDateTime = fixtureInDb.StartDateTime;

                            if (fixtureSchedule.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingCompleted)
                            {
                                fixtureSchedule.SchedulerStateFixtures =
                                    FixturesStateHelper.GetSchedulerStateForFixture(DateTime.UtcNow, gameState, fixtureInDb.StartDateTime.DateTime);
                            }

                            if (HasFixtureEnded(fixtureInDb, fixture.gameStateName) &&
                                fixtureSchedule.EndedDateTime == DateTimeOffset.MinValue)
                            {
                                fixtureSchedule.EndedDateTime =
                                    fixtureSchedule.StartDateTime
                                        .AddSeconds(
                                            fixture.gameSeconds)
                                        .AddMinutes(_halfTimeMaximumDurationInMinutes); // This is for half-time break.
                            }

                            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Update(fixtureSchedule);
                        }
                    }
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private bool HasFixtureEnded(RugbyFixture rugbyFixture, string gameStateName)
        {
            var state = GetFixtureStatusFromProviderFixtureState(rugbyFixture, gameStateName);
            return state == RugbyFixtureStatus.Result;
        }

        private async Task PersistRugbyFixturesToPublicSportsRepository(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var tournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == fixtures.Fixtures.competitionId);

            var allFixtures = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync()).ToList();
            var allTeams = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).ToList();
            var allVenues = (await _publicSportDataUnitOfWork.RugbyVenues.AllAsync()).ToList();

            foreach (var roundFixture in fixtures.Fixtures.roundFixtures)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                foreach (var fixture in roundFixture.gameFixtures)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var fixtureId = fixture.gameId;

                    DateTimeOffset.TryParse(fixture.startTimeUTC, out DateTimeOffset startTime);
                    var teams = fixture.teams.ToArray();
                    // We need temporary variables here.
                    // Cannot use indexing in Linq Where clause.
                    var team0 = teams[0];
                    var team1 = teams[1];

                    var teamA = allTeams.FirstOrDefault(t => t.ProviderTeamId == team0.teamId);
                    var teamB = allTeams.FirstOrDefault(t => t.ProviderTeamId == team1.teamId);

                    // When either team is null i.e this fixture has missing information.
                    // Do not ingest this fixture.
                    // Ammended: Ingest this fixture.
                    // We have a TBC team in the DB for when a team is un-determined.

                    var isFixtureTbc = teamA == null || teamB == null;
                    var isFixturePartOfAFinal = IsProviderFixturePartOfFinal(roundFixture);

                    if (isFixtureTbc && !isFixturePartOfAFinal)
                        continue;

                    var venue = fixture.venueName == null ? null :
                        allVenues.FirstOrDefault(v => v.ProviderVenueId == fixture.venueId);

                    var newFixture = new RugbyFixture()
                    {
                        ProviderFixtureId = fixtureId,
                        StartDateTime = startTime,
                        RugbyVenue = venue,
                        RugbyTournament = tournament,
                        TeamA = teamA,
                        TeamB = teamB,
                        TeamAIsHomeTeam = team0.isHomeTeam,
                        TeamBIsHomeTeam = team1.isHomeTeam,
                        RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(null, fixture.gameStateName),
                        DataProvider = DataProvider.StatsProzone,
                        IsLiveScored = tournament != null && tournament.IsLiveScored,
                        TeamAScore = null,
                        TeamBScore = null,
                        RoundNumber = roundFixture.roundId,
                        RugbySeason =
                            _publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(s => 
                                    s.RugbyTournament.ProviderTournamentId == tournament.ProviderTournamentId && 
                                    s.ProviderSeasonId == fixtures.Fixtures.seasonId)
                    };

                    // Should we set the scores of the new fixture?
                    if (newFixture.RugbyFixtureStatus != RugbyFixtureStatus.PreMatch)
                    {
                        newFixture.TeamAScore = team0.teamFinalScore;
                        newFixture.TeamBScore = team1.teamFinalScore;
                    }

                    // Lookup in Db
                    var fixtureInDb = allFixtures.FirstOrDefault(f => f.ProviderFixtureId == fixtureId);

                    if (fixtureInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyFixtures.Add(newFixture);
                    }
                    else
                    {
                        if (!fixtureInDb.CmsOverrideModeIsActive)
                        {
                            fixtureInDb.StartDateTime = startTime;
                            fixtureInDb.RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixtureInDb, fixture.gameStateName);

                            // Only update the scores for games that are completed.
                            // Real-time scores will be updated separately in a method that runs more frequently.
                            if (fixtureInDb.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                            {
                                fixtureInDb.TeamAScore = team0.teamFinalScore;
                                fixtureInDb.TeamBScore = team1.teamFinalScore;
                            }
                        }

                        fixtureInDb.RugbyVenue = newFixture.RugbyVenue;
                        fixtureInDb.TeamA = newFixture.TeamA;
                        fixtureInDb.TeamB = newFixture.TeamB;
                        fixtureInDb.TeamAIsHomeTeam = newFixture.TeamAIsHomeTeam;
                        fixtureInDb.TeamBIsHomeTeam = newFixture.TeamBIsHomeTeam;
                        fixtureInDb.RugbyTournament = newFixture.RugbyTournament;
                        fixtureInDb.RoundNumber = roundFixture.roundId;
                        fixtureInDb.RugbySeason = newFixture.RugbySeason;

                        // Do not update the isLiveScored property here.
                        // It will be updated by the CMS.
                        _publicSportDataUnitOfWork.RugbyFixtures.Update(fixtureInDb);
                    }
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private bool IsProviderFixturePartOfFinal(Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.RoundFixture roundFixture)
        {
            if (roundFixture.roundName.Equals("Quarter Finals"))
                return true;

            if (roundFixture.roundName.Equals("Semi Finals"))
                return true;

            if (roundFixture.roundName.Equals("Final"))
                return true;

            return false;
        }

        private static RugbyFixtureStatus GetFixtureStatusFromProviderFixtureState(RugbyFixture rugbyFixture, string gameStateName)
        {
            if (rugbyFixture != null && rugbyFixture.CmsOverrideModeIsActive)
            {
                return rugbyFixture.RugbyFixtureStatus;
            }

            if (gameStateName.Equals(ProviderGameStateConstant.PreGame, StringComparison.InvariantCultureIgnoreCase))
                return RugbyFixtureStatus.PreMatch;

            if (gameStateName == "Final")
                return RugbyFixtureStatus.Result;

            if (gameStateName == "Game End")
                return RugbyFixtureStatus.Result;

            if (gameStateName.Equals(ProviderGameStateConstant.FullTime, StringComparison.InvariantCultureIgnoreCase))
                return RugbyFixtureStatus.FullTime;

            if (gameStateName == "Pre Match")
                return RugbyFixtureStatus.PreMatch;

            if (gameStateName == "First Half")
                return RugbyFixtureStatus.FirstHalf;

            if (gameStateName == "Half Time")
                return RugbyFixtureStatus.HalfTime;

            if (gameStateName == "Second Half")
                return RugbyFixtureStatus.SecondHalf;

            if (gameStateName == "Full Time")
                return RugbyFixtureStatus.FullTime;

            if (gameStateName == "Extra Time")
                return RugbyFixtureStatus.ExtraTime;

            return RugbyFixtureStatus.PreMatch;
        }

        private async Task PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            var season =
                (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync())
                .FirstOrDefault(
                    s => s.ProviderSeasonId == fixtures.Fixtures.seasonId && s.RugbyTournament.ProviderTournamentId == fixtures.Fixtures.competitionId);

            if (season == null)
                return;

            var seasonId = season.Id;
            var tournamentId = season.RugbyTournament.Id;
            DateTimeOffset.TryParse(fixtures.Fixtures.seasonStartDate, out var seasonStartDate);
            DateTimeOffset.TryParse(fixtures.Fixtures.seasonFinishDate, out var seasonEndDate);

            season.StartDateTime = seasonStartDate;
            season.EndDateTime = seasonEndDate;
            _publicSportDataUnitOfWork.RugbySeasons.Update(season);

            var dateOffsetNow = DateTimeOffset.Now;

            var seasonStatus = GetRugbySeasonStatus(seasonStartDate, dateOffsetNow, seasonEndDate);

            var seasonInDb = (await _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.AllAsync()).FirstOrDefault(s => s.SeasonId == seasonId && s.TournamentId == tournamentId);

            if (seasonInDb == null)
            {
                _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(
                    new SchedulerTrackingRugbySeason()
                    {
                        SeasonId = seasonId,
                        TournamentId = tournamentId,
                        SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning,
                        RugbySeasonStatus = seasonStatus
                    });
            }
            else
            {
                seasonInDb.RugbySeasonStatus = seasonStatus;
                _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Update(seasonInDb);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private RugbySeasonStatus GetRugbySeasonStatus(DateTimeOffset seasonStartDate, DateTimeOffset dateOffsetNow, DateTimeOffset seasonEndDate)
        {
            if (dateOffsetNow < seasonStartDate)
                return RugbySeasonStatus.NotActive;

            if (dateOffsetNow > seasonEndDate)
                return RugbySeasonStatus.Ended;

            return RugbySeasonStatus.InProgress;
        }

        public async Task IngestLogsForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var activeTournaments =
                (await _rugbyService.GetActiveTournaments()).ToList();

            await IngestLogsHelper(activeTournaments, cancellationToken);
        }

        public async Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var currentTournaments =
                (await _rugbyService.GetCurrentTournaments()).ToList();

            await IngestLogsHelper(currentTournaments, cancellationToken);
        }

        private async Task IngestLogsHelper(List<RugbyTournament> tournaments, CancellationToken cancellationToken)
        {
            var seasons = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).ToList();

            foreach (var tournament in tournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                int activeSeasonIdForTournament =
                        await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var season = seasons.FirstOrDefault(s =>
                                                 s.ProviderSeasonId == activeSeasonIdForTournament &&
                                                 s.RugbyTournament.ProviderTournamentId == tournament.ProviderTournamentId);

                if (season == null) continue;
                var numberOfRounds = season.CurrentRoundNumberCmsOverride ?? season.CurrentRoundNumber;

                var logType = season.RugbyLogType;

                if (logType == RugbyLogType.FlatLogs)
                {
                    RugbyFlatLogsResponse logs =
                        await _statsProzoneIngestService.IngestFlatLogsForTournament(
                            tournament.ProviderTournamentId, 
                            activeSeasonIdForTournament,
                            numberOfRounds);

                    if (logs == null)
                        continue;

                    await PersistFlatLogs(cancellationToken, logs);

                    _mongoDbRepository.Save(logs);
                }
                else
                {
                    var logs =
                        await _statsProzoneIngestService.IngestGroupedLogsForTournament(
                            tournament.ProviderTournamentId, 
                            activeSeasonIdForTournament, 
                            numberOfRounds);

                    if (logs == null)
                        continue;

                    await PersistGroupedLogs(cancellationToken, logs);

                    _mongoDbRepository.Save(logs);
                }
            }
        }

        private async Task IngestStandingsForSuperRugby(
            CancellationToken cancellationToken,
            int groupHierarchyLevel,
            RugbyGroupedLogsResponse logs,
            IEnumerable<Ladderposition> ladderPositions)
        {
            var tournamentId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;

            var rugbyTournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == tournamentId);
            if (rugbyTournament == null) return;

            var rugbySeason = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).FirstOrDefault(s => s.RugbyTournament.ProviderTournamentId == tournamentId && s.ProviderSeasonId == seasonId);

            foreach (var ladder in ladderPositions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamId = ladder.teamId;
                var rugbyTeam = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).FirstOrDefault(t => t.ProviderTeamId == teamId);

                if (rugbySeason == null) continue;
                if (rugbyTeam == null) continue;

                try
                {
                    var rugbyLogGroup = (await _publicSportDataUnitOfWork.RugbyLogGroups.AllAsync()).Single(g =>
                        g.RugbySeason.Id == rugbySeason.Id &&
                        g.GroupHierarchyLevel == groupHierarchyLevel &&
                        g.ProviderLogGroupId == ladder.group &&
                        g.ProviderGroupName == ladder.groupName);

                    // Does an entry in the db exist for this tournament-season-round-team?
                    var entryInDb = (await _publicSportDataUnitOfWork.RugbyGroupedLogs.AllAsync())
                                        .FirstOrDefault(g => g.RugbyLogGroup.ProviderLogGroupId == ladder.group &&
                                                             g.RugbyTournament.ProviderTournamentId == tournamentId &&
                                                             g.RugbySeason.ProviderSeasonId == seasonId &&
                                                             g.RoundNumber == logs.RugbyGroupedLogs.roundNumber &&
                                                             g.RugbyTeam.ProviderTeamId == teamId);

                    var newLogEntry = new RugbyGroupedLog()
                    {
                        LogPosition = ladder.position,
                        GamesPlayed = ladder.gamesPlayed,
                        GamesWon = ladder.wins,
                        GamesLost = ladder.losses,
                        GamesDrawn = ladder.draws ?? 0,
                        PointsFor = ladder.pointsFor,
                        PointsAgainst = ladder.pointsAgainst,
                        PointsDifference = ladder.pointsDifference,
                        TournamentPoints = ladder.competitionPoints,
                        BonusPoints = ladder.bonusPoints,
                        TriesFor = ladder.triesFor,
                        TriesAgainst = ladder.triesAgainst,
                        RugbyTeam = rugbyTeam,
                        RugbyLogGroup = rugbyLogGroup,
                        RoundNumber = logs.RugbyGroupedLogs.roundNumber,
                        RugbySeason = rugbySeason,
                        RugbySeasonId = rugbySeason.Id,
                        RugbyTeamId = rugbyTeam.Id,
                        RugbyTournament = rugbyTournament,
                        RugbyTournamentId = rugbyTournament.Id,
                        RugbyLogGroupId = rugbyLogGroup.Id
                    };

                    if (entryInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Add(newLogEntry);
                    }
                    else
                    {
                        entryInDb.LogPosition = ladder.position;
                        entryInDb.GamesPlayed = ladder.gamesPlayed;
                        entryInDb.GamesWon = ladder.wins;
                        entryInDb.GamesLost = ladder.losses;
                        entryInDb.GamesDrawn = ladder.draws ?? 0;
                        entryInDb.PointsFor = ladder.pointsFor;
                        entryInDb.PointsAgainst = ladder.pointsAgainst;
                        entryInDb.PointsDifference = ladder.pointsDifference;
                        entryInDb.TournamentPoints = ladder.competitionPoints;
                        entryInDb.BonusPoints = ladder.bonusPoints;
                        entryInDb.TriesFor = ladder.triesFor;
                        entryInDb.TriesAgainst = ladder.triesAgainst;

                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Update(entryInDb);
                    }
                }
                catch (Exception exception)
                {
                    var key = GetType().FullName + ".IngestStandingsForSuperRugby" + ". Season=" + seasonId;
                    var message = $"Error processing RugbyLogGroup for ladderId={ladder.group}, groupName={ladder.groupName}. Check that the RugbyLogGroup entry is configured in the DB.";
                    await _logger.Error(key, exception, $"{message}{Environment.NewLine}{key}{Environment.NewLine}{exception}");
                    continue;
                }
            }
        }

        private async Task IngestStandingsForChampionsCup(
            CancellationToken cancellationToken,
            int groupHierarchyLevel,
            RugbyGroupedLogsResponse logs,
            IEnumerable<Ladderposition> ladderPositions)
        {
            var tournamentId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;

            var rugbyTournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == tournamentId);
            if (rugbyTournament == null)
                return;

            var rugbySeason = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).FirstOrDefault(s => s.RugbyTournament.ProviderTournamentId == tournamentId && s.ProviderSeasonId == seasonId);

            foreach (var ladder in ladderPositions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamId = ladder.teamId;
                var rugbyTeam = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).FirstOrDefault(t => t.ProviderTeamId == teamId);

                if (rugbySeason == null) continue;
                if (rugbyTeam == null) continue;

                try
                {
                    var rugbyLogGroup = (await _publicSportDataUnitOfWork.RugbyLogGroups.AllAsync()).Single(g =>
                        g.RugbySeason.Id == rugbySeason.Id &&
                        g.GroupHierarchyLevel == groupHierarchyLevel &&
                        g.ProviderLogGroupId == ladder.group &&
                        g.ProviderGroupName == ladder.groupName);

                    // Does an entry in the db exist for this tournament-season-round-team?
                    var entryInDb = (await _publicSportDataUnitOfWork.RugbyGroupedLogs.AllAsync())
                                        .FirstOrDefault(g => g.RugbyLogGroup.ProviderLogGroupId == ladder.group &&
                                                             g.RugbyTournament.ProviderTournamentId == tournamentId &&
                                                             g.RugbySeason.ProviderSeasonId == seasonId &&
                                                             g.RoundNumber == logs.RugbyGroupedLogs.roundNumber &&
                                                             g.RugbyTeam.ProviderTeamId == teamId);

                    var newLogEntry = new RugbyGroupedLog()
                    {
                        LogPosition = ladder.position,
                        GamesPlayed = ladder.gamesPlayed,
                        GamesWon = ladder.wins,
                        GamesLost = ladder.losses,
                        GamesDrawn = ladder.draws ?? 0,
                        PointsFor = ladder.pointsFor,
                        PointsAgainst = ladder.pointsAgainst,
                        PointsDifference = ladder.pointsDifference,
                        TournamentPoints = ladder.competitionPoints,
                        BonusPoints = ladder.bonusPoints,
                        TriesFor = ladder.triesFor,
                        TriesAgainst = ladder.triesAgainst,
                        RugbyTeam = rugbyTeam,
                        RugbyLogGroup = rugbyLogGroup,
                        RoundNumber = logs.RugbyGroupedLogs.roundNumber,
                        RugbySeason = rugbySeason,
                        RugbySeasonId = rugbySeason.Id,
                        RugbyTeamId = rugbyTeam.Id,
                        RugbyTournament = rugbyTournament,
                        RugbyTournamentId = rugbyTournament.Id,
                        RugbyLogGroupId = rugbyLogGroup.Id
                    };

                    if (entryInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Add(newLogEntry);
                    }
                    else
                    {
                        entryInDb.LogPosition = ladder.position;
                        entryInDb.GamesPlayed = ladder.gamesPlayed;
                        entryInDb.GamesWon = ladder.wins;
                        entryInDb.GamesLost = ladder.losses;
                        entryInDb.GamesDrawn = ladder.draws ?? 0;
                        entryInDb.PointsFor = ladder.pointsFor;
                        entryInDb.PointsAgainst = ladder.pointsAgainst;
                        entryInDb.PointsDifference = ladder.pointsDifference;
                        entryInDb.TournamentPoints = ladder.competitionPoints;
                        entryInDb.BonusPoints = ladder.bonusPoints;
                        entryInDb.TriesFor = ladder.triesFor;
                        entryInDb.TriesAgainst = ladder.triesAgainst;

                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Update(entryInDb);
                    }
                }
                catch (Exception exception)
                {
                    var key = GetType().FullName + ".IngestStandingsForChampionsCup" + ". Season=" + seasonId;
                    var message = $"Error processing RugbyLogGroup for ladderId={ladder.group}, groupName={ladder.groupName}. Check that the RugbyLogGroup entry is configured in the DB.";
                    await _logger.Error(key, exception, $"{message}{Environment.NewLine}{key}{Environment.NewLine}{exception}");
                }
            }
        }

        private async Task IngestStandingsForPro14(
            CancellationToken cancellationToken,
            int groupHierarchyLevel,
            RugbyGroupedLogsResponse logs,
            IEnumerable<Ladderposition> ladderPositions)
        {
            var tournamentId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;

            var rugbyTournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == tournamentId);
            if (rugbyTournament == null)
                return;

            var rugbySeason = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).FirstOrDefault(s => s.RugbyTournament.ProviderTournamentId == tournamentId && s.ProviderSeasonId == seasonId);

            foreach (var ladder in ladderPositions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamId = ladder.teamId;
                var rugbyTeam = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).FirstOrDefault(t => t.ProviderTeamId == teamId);

                if (rugbySeason == null) continue;
                if (rugbyTeam == null) continue;

                try
                {
                    var rugbyLogGroup = (await _publicSportDataUnitOfWork.RugbyLogGroups.AllAsync()).Single(g =>
                        g.RugbySeason.Id == rugbySeason.Id &&
                        g.GroupHierarchyLevel == groupHierarchyLevel &&
                        g.ProviderLogGroupId == ladder.group &&
                        g.ProviderGroupName == ladder.groupName);

                    // Does an entry in the db exist for this tournament-season-round-team?
                    var entryInDb = (await _publicSportDataUnitOfWork.RugbyGroupedLogs.AllAsync())
                                        .FirstOrDefault(g => g.RugbyLogGroup.ProviderLogGroupId == ladder.group &&
                                                             g.RugbyTournament.ProviderTournamentId == tournamentId &&
                                                             g.RugbySeason.ProviderSeasonId == seasonId &&
                                                             g.RoundNumber == logs.RugbyGroupedLogs.roundNumber &&
                                                             g.RugbyTeam.ProviderTeamId == teamId);

                    var newLogEntry = new RugbyGroupedLog()
                    {
                        LogPosition = ladder.position,
                        GamesPlayed = ladder.gamesPlayed,
                        GamesWon = ladder.wins,
                        GamesLost = ladder.losses,
                        GamesDrawn = ladder.draws ?? 0,
                        PointsFor = ladder.pointsFor,
                        PointsAgainst = ladder.pointsAgainst,
                        PointsDifference = ladder.pointsDifference,
                        TournamentPoints = ladder.competitionPoints,
                        BonusPoints = ladder.bonusPoints,
                        TriesFor = ladder.triesFor,
                        TriesAgainst = ladder.triesAgainst,
                        RugbyTeam = rugbyTeam,
                        RugbyLogGroup = rugbyLogGroup,
                        RoundNumber = logs.RugbyGroupedLogs.roundNumber,
                        RugbySeason = rugbySeason,
                        RugbySeasonId = rugbySeason.Id,
                        RugbyTeamId = rugbyTeam.Id,
                        RugbyTournament = rugbyTournament,
                        RugbyTournamentId = rugbyTournament.Id,
                        RugbyLogGroupId = rugbyLogGroup.Id
                    };

                    if (entryInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Add(newLogEntry);
                    }
                    else
                    {
                        entryInDb.LogPosition = ladder.position;
                        entryInDb.GamesPlayed = ladder.gamesPlayed;
                        entryInDb.GamesWon = ladder.wins;
                        entryInDb.GamesLost = ladder.losses;
                        entryInDb.GamesDrawn = ladder.draws ?? 0;
                        entryInDb.PointsFor = ladder.pointsFor;
                        entryInDb.PointsAgainst = ladder.pointsAgainst;
                        entryInDb.PointsDifference = ladder.pointsDifference;
                        entryInDb.TournamentPoints = ladder.competitionPoints;
                        entryInDb.BonusPoints = ladder.bonusPoints;
                        entryInDb.TriesFor = ladder.triesFor;
                        entryInDb.TriesAgainst = ladder.triesAgainst;

                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Update(entryInDb);
                    }
                }
                catch (Exception exception)
                {
                    var key = GetType().FullName + ".IngestStandingsForPro14" + ". Season=" + seasonId;
                    var message = $"Error processing RugbyLogGroup for ladderId={ladder.group}, groupName={ladder.groupName}. Check that the RugbyLogGroup entry is configured in the DB.";
                    await _logger.Error(key, exception, $"{message}{Environment.NewLine}{key}{Environment.NewLine}{exception}");
                }
            }
        }

        private async Task IngestStandingsForMitreCup(
            CancellationToken cancellationToken,
            int groupHierarchyLevel,
            RugbyGroupedLogsResponse logs,
            IEnumerable<Ladderposition> ladderPositions)
        {
            var tournamentId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;

            var rugbyTournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == tournamentId);
            if (rugbyTournament == null)
                return;

            var rugbySeason = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).FirstOrDefault(s => s.RugbyTournament.ProviderTournamentId == tournamentId && s.ProviderSeasonId == seasonId);

            foreach (var ladder in ladderPositions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamId = ladder.teamId;
                var rugbyTeam = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).FirstOrDefault(t => t.ProviderTeamId == teamId);

                if (rugbySeason == null) continue;
                if (rugbyTeam == null) continue;

                try
                {
                    var rugbyLogGroup = (await _publicSportDataUnitOfWork.RugbyLogGroups.AllAsync()).Single(g =>
                        g.RugbySeason.Id == rugbySeason.Id &&
                        g.GroupHierarchyLevel == groupHierarchyLevel &&
                        g.ProviderLogGroupId == ladder.group &&
                        g.ProviderGroupName == ladder.groupName);

                    // Does an entry in the db exist for this tournament-season-round-team?
                    var entryInDb = (await _publicSportDataUnitOfWork.RugbyGroupedLogs.AllAsync())
                                        .FirstOrDefault(g => g.RugbyLogGroup.ProviderLogGroupId == ladder.group &&
                                                             g.RugbyTournament.ProviderTournamentId == tournamentId &&
                                                             g.RugbySeason.ProviderSeasonId == seasonId &&
                                                             g.RoundNumber == logs.RugbyGroupedLogs.roundNumber &&
                                                             g.RugbyTeam.ProviderTeamId == teamId);

                    var newLogEntry = new RugbyGroupedLog()
                    {
                        LogPosition = ladder.position,
                        GamesPlayed = ladder.gamesPlayed,
                        GamesWon = ladder.wins,
                        GamesLost = ladder.losses,
                        GamesDrawn = ladder.draws ?? 0,
                        PointsFor = ladder.pointsFor,
                        PointsAgainst = ladder.pointsAgainst,
                        PointsDifference = ladder.pointsDifference,
                        TournamentPoints = ladder.competitionPoints,
                        BonusPoints = ladder.bonusPoints,
                        TriesFor = ladder.triesFor,
                        TriesAgainst = ladder.triesAgainst,
                        RugbyTeam = rugbyTeam,
                        RugbyLogGroup = rugbyLogGroup,
                        RoundNumber = logs.RugbyGroupedLogs.roundNumber,
                        RugbySeason = rugbySeason,
                        RugbySeasonId = rugbySeason.Id,
                        RugbyTeamId = rugbyTeam.Id,
                        RugbyTournament = rugbyTournament,
                        RugbyTournamentId = rugbyTournament.Id,
                        RugbyLogGroupId = rugbyLogGroup.Id
                    };

                    if (entryInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Add(newLogEntry);
                    }
                    else
                    {
                        entryInDb.LogPosition = ladder.position;
                        entryInDb.GamesPlayed = ladder.gamesPlayed;
                        entryInDb.GamesWon = ladder.wins;
                        entryInDb.GamesLost = ladder.losses;
                        entryInDb.GamesDrawn = ladder.draws ?? 0;
                        entryInDb.PointsFor = ladder.pointsFor;
                        entryInDb.PointsAgainst = ladder.pointsAgainst;
                        entryInDb.PointsDifference = ladder.pointsDifference;
                        entryInDb.TournamentPoints = ladder.competitionPoints;
                        entryInDb.BonusPoints = ladder.bonusPoints;
                        entryInDb.TriesFor = ladder.triesFor;
                        entryInDb.TriesAgainst = ladder.triesAgainst;

                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Update(entryInDb);
                    }
                }
                catch (Exception exception)
                {
                    var key = GetType().FullName + ".IngestStandingsForMitreCup" + ". Season=" + seasonId;
                    var message = $"Error processing RugbyLogGroup for ladderId={ladder.group}, groupName={ladder.groupName}. Check that the RugbyLogGroup entry is configured in the DB.";
                    await _logger.Error(key, exception, $"{message}{Environment.NewLine}{key}{Environment.NewLine}{exception}");
                }
            }
        }

        private async Task IngestStandingsForSevens2018(
            CancellationToken cancellationToken,
            int groupHierarchyLevel,
            RugbyGroupedLogsResponse logs,
            IEnumerable<Ladderposition> ladderPositions)
        {
            var tournamentId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;

            var rugbyTournament = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync()).FirstOrDefault(t => t.ProviderTournamentId == tournamentId);
            if (rugbyTournament == null)
                return;

            var rugbySeason = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync()).FirstOrDefault(s => s.RugbyTournament.ProviderTournamentId == tournamentId && s.ProviderSeasonId == seasonId);

            foreach (var ladder in ladderPositions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamId = ladder.teamId;
                var rugbyTeam = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).FirstOrDefault(t => t.ProviderTeamId == teamId);

                if (rugbySeason == null) continue;
                if (rugbyTeam == null) continue;

                var roundNumber = 
                    rugbySeason.CurrentRoundNumberCmsOverride ?? rugbySeason.CurrentRoundNumber;

                if (ladder.roundNumber != roundNumber)
                    continue;

                try
                {
                    var rugbyLogGroup = (await _publicSportDataUnitOfWork.RugbyLogGroups.AllAsync()).Single(g =>
                        g.RugbySeason.Id == rugbySeason.Id &&
                        g.GroupHierarchyLevel == groupHierarchyLevel &&
                        g.ProviderLogGroupId == ladder.group &&
                        g.ProviderGroupName == ladder.groupName);

                    // Does an entry in the db exist for this tournament-season-round-team?
                    var entryInDb = (await _publicSportDataUnitOfWork.RugbyGroupedLogs.AllAsync())
                                        .FirstOrDefault(g => g.RugbyLogGroup.ProviderLogGroupId == ladder.group &&
                                                             g.RugbyTournament.ProviderTournamentId == tournamentId &&
                                                             g.RugbySeason.ProviderSeasonId == seasonId &&
                                                             g.RoundNumber == ladder.roundNumber &&
                                                             g.RugbyTeam.ProviderTeamId == teamId);

                    var newLogEntry = new RugbyGroupedLog()
                    {
                        LogPosition = ladder.position,
                        GamesPlayed = ladder.gamesPlayed,
                        GamesWon = ladder.wins,
                        GamesLost = ladder.losses,
                        GamesDrawn = ladder.draws ?? 0,
                        PointsFor = ladder.pointsFor,
                        PointsAgainst = ladder.pointsAgainst,
                        PointsDifference = ladder.pointsDifference,
                        TournamentPoints = ladder.competitionPoints,
                        BonusPoints = ladder.bonusPoints,
                        TriesFor = ladder.triesFor,
                        TriesAgainst = ladder.triesAgainst,
                        RugbyTeam = rugbyTeam,
                        RugbyLogGroup = rugbyLogGroup,
                        RoundNumber = ladder.roundNumber?? 0,
                        RugbySeason = rugbySeason,
                        RugbySeasonId = rugbySeason.Id,
                        RugbyTeamId = rugbyTeam.Id,
                        RugbyTournament = rugbyTournament,
                        RugbyTournamentId = rugbyTournament.Id,
                        RugbyLogGroupId = rugbyLogGroup.Id
                    };

                    if (entryInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Add(newLogEntry);
                    }
                    else
                    {
                        entryInDb.LogPosition = ladder.position;
                        entryInDb.GamesPlayed = ladder.gamesPlayed;
                        entryInDb.GamesWon = ladder.wins;
                        entryInDb.GamesLost = ladder.losses;
                        entryInDb.GamesDrawn = ladder.draws ?? 0;
                        entryInDb.PointsFor = ladder.pointsFor;
                        entryInDb.PointsAgainst = ladder.pointsAgainst;
                        entryInDb.PointsDifference = ladder.pointsDifference;
                        entryInDb.TournamentPoints = ladder.competitionPoints;
                        entryInDb.BonusPoints = ladder.bonusPoints;
                        entryInDb.TriesFor = ladder.triesFor;
                        entryInDb.TriesAgainst = ladder.triesAgainst;

                        _publicSportDataUnitOfWork.RugbyGroupedLogs.Update(entryInDb);
                    }
                }
                catch (Exception exception)
                {
                    var key = GetType().FullName + ".IngestStandingsForSevens2018" + ". Season=" + seasonId;
                    var message = $"Error processing RugbyLogGroup for ladderId={ladder.group}, groupName={ladder.groupName} roundNumber={ladder.roundNumber}. Check that the RugbyLogGroup entry is configured in the DB.";
                    await _logger.Warn(key, exception, $"{message}{Environment.NewLine}{key}{Environment.NewLine}{exception}");
                }
            }
        }

        private async Task PersistGroupedLogs(CancellationToken cancellationToken, RugbyGroupedLogsResponse logs)
        {
            var competitionId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;

            if (competitionId == RugbyStatsProzoneConstants.ProviderTournamentIdChampionsCup)
            {
                if (logs.RugbyGroupedLogs.overallStandings == null &&
                    logs.RugbyGroupedLogs.groupStandings == null &&
                    logs.RugbyGroupedLogs.secondaryGroupStandings == null)
                {
                    await IngestStandingsForChampionsCup(cancellationToken, 1, logs, logs.RugbyGroupedLogs.ladderposition);
                }
            }

            if (competitionId == RugbyStatsProzoneConstants.ProviderTournamentIdMitreCup)
            {
                if (logs.RugbyGroupedLogs.overallStandings == null &&
                    logs.RugbyGroupedLogs.groupStandings == null &&
                    logs.RugbyGroupedLogs.secondaryGroupStandings == null)
                {
                    if (seasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2017)
                        await IngestStandingsForMitreCup(cancellationToken, 1, logs, logs.RugbyGroupedLogs.ladderposition);
                }
            }

            if (competitionId == RugbyStatsProzoneConstants.ProviderTournamentIdPro14)
            {
                if (logs.RugbyGroupedLogs.overallStandings == null &&
                    logs.RugbyGroupedLogs.groupStandings == null &&
                    logs.RugbyGroupedLogs.secondaryGroupStandings == null)
                {
                    await IngestStandingsForPro14(cancellationToken, 1, logs, logs.RugbyGroupedLogs.ladderposition);
                }
            }

            if (competitionId == RugbyStatsProzoneConstants.ProviderTournamentIdSevensRugby)
            {
                if (seasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018)
                {
                    if (logs.RugbyGroupedLogs.secondaryGroupStandings != null)
                        await IngestStandingsForSevens2018(cancellationToken, 1, logs, logs.RugbyGroupedLogs.secondaryGroupStandings.ladderposition);

                    if (logs.RugbyGroupedLogs.groupStandings != null)
                        await IngestStandingsForSevens2018(cancellationToken, 2, logs, logs.RugbyGroupedLogs.groupStandings.ladderposition);
                }
            }

            if (competitionId == RugbyStatsProzoneConstants.ProviderTournamentIdSuperRugby)
            {
                // Structure for 2017 season.
                if (seasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2017)
                {
                    // "OverallStandings" are GroupHierarchyLevel: 0.
                    if (logs.RugbyGroupedLogs.overallStandings != null)
                        await IngestStandingsForSuperRugby(cancellationToken, 0, logs, logs.RugbyGroupedLogs.overallStandings.ladderposition);

                    // "SecondaryGroupStandings" are GroupHierarchyLevel: 1.
                    if (logs.RugbyGroupedLogs.secondaryGroupStandings != null)
                        await IngestStandingsForSuperRugby(cancellationToken, 1, logs, logs.RugbyGroupedLogs.secondaryGroupStandings.ladderposition);

                    // "GroupStandings" are GroupHierarchyLevel: 2.
                    if (logs.RugbyGroupedLogs.groupStandings != null)
                        await IngestStandingsForSuperRugby(cancellationToken, 2, logs, logs.RugbyGroupedLogs.groupStandings.ladderposition);
                }
                else
                {
                    // Structure for 2018 season (and onwards...!).
                    // Should this structure change in the future, apply further explicit checks for relevant seasons:
                    // e.g. "if (seasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018)" etc.

                    // "OverallStandings" are GroupHierarchyLevel: 0.
                    if (logs.RugbyGroupedLogs.overallStandings != null)
                        await IngestStandingsForSuperRugby(cancellationToken, 0, logs, logs.RugbyGroupedLogs.overallStandings.ladderposition);

                    // "GroupStandings" are GroupHierarchyLevel: 1.
                    if (logs.RugbyGroupedLogs.groupStandings != null)
                        await IngestStandingsForSuperRugby(cancellationToken, 1, logs, logs.RugbyGroupedLogs.groupStandings.ladderposition);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistTournamentsInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (entitiesResponse.Entities != null && entitiesResponse.Entities.competitions == null)
                return;

            if (entitiesResponse.Entities != null)
            {
                foreach (var competition in entitiesResponse.Entities.competitions)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var entry = (await _publicSportDataUnitOfWork.RugbyTournaments.AllAsync())
                        .FirstOrDefault(c => c.ProviderTournamentId == competition.id);

                    // We are disregarding competition 401.
                    // The provider has another listing for "World Rugby Seven Series"
                    // under the ID 831. We are using that instead.
                    if (competition.id == 401)
                        continue;

                    var newEntry = new RugbyTournament
                    {
                        ProviderTournamentId = competition.id,
                        Name = competition.name,
                        IsEnabled = entry != null,
                        LogoUrl = competition.CompetitionLogoURL,
                        Abbreviation = competition.CompetitionAbbrev,
                        Slug = GetSlug(competition.name),
                        DataProvider = DataProvider.StatsProzone,
                        IsLiveScored = false
                    };

                    if (entry == null)
                    {
                        _publicSportDataUnitOfWork.RugbyTournaments.Add(newEntry);
                    }
                    else
                    {
                        entry.ProviderTournamentId = newEntry.ProviderTournamentId;
                        entry.Name = newEntry.Name;
                        entry.LogoUrl = newEntry.LogoUrl;
                        entry.Abbreviation = newEntry.Abbreviation;
                        // Do not update the IsLiveScored property here.
                        // It will be updated by the CMS.

                        _publicSportDataUnitOfWork.RugbyTournaments.Update(entry);
                    }
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private string GetSlug(string name)
        {
            var lowerCase = name.ToLower();
            var lowerCaseWithSpecialCharactersRemoved =
                    RemoveSpecialCharacters(lowerCase);

            var slug = lowerCaseWithSpecialCharactersRemoved.Replace(' ', '-');
            return slug;
        }

        private static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_ ]+", "", RegexOptions.Compiled);
        }

        public async Task IngestResultsForAllFixtures(CancellationToken cancellationToken)
        {
            var activeTournaments = (await _rugbyService.GetActiveTournaments());

            foreach (var tournament in activeTournaments)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);
                var results = await _statsProzoneIngestService.IngestFixturesForTournament(tournament, seasonId, cancellationToken);
                if (results == null)
                    continue;

                await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, results);
            }
        }

        public async Task IngestFixturesForTournamentSeason(CancellationToken cancellationToken, int tournamentId, int seasonId)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await PersistTournamentSeasonsInRepository(cancellationToken);

            var fixtures =
                await _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                    tournamentId, seasonId, cancellationToken);

            if (fixtures == null)
                return;

            await PersistFixturesData(cancellationToken, fixtures);
        }

        public async Task IngestPastSeasonsForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await PersistPastTournamentSeasonsInRepository(cancellationToken);
        }

        public async Task IngestResultsForCurrentDayFixtures(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var currentDayFixtures = await _rugbyService.GetCurrentDayFixturesForActiveTournaments();

            foreach (var fixture in currentDayFixtures)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var tournamentId = fixture.RugbyTournament.ProviderTournamentId;

                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, fixture.RugbyTournament.Id);

                var results = await _statsProzoneIngestService.IngestFixturesForTournamentSeason(tournamentId, seasonId, cancellationToken);
                if (results == null)
                    continue;

                await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, results);
            }
        }

        public async Task IngestResultsForFixturesInResultsState(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixtures = await _rugbyService.GetCurrentDayFixturesForActiveTournaments();

            foreach (var fixture in fixtures)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var tournamentId = fixture.RugbyTournament.ProviderTournamentId;

                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, fixture.RugbyTournament.Id);

                var results = await _statsProzoneIngestService.IngestFixturesForTournamentSeason(tournamentId, seasonId, cancellationToken);
                if (results == null)
                    continue;

                await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, results);
            }
        }

        public async Task IngestOneMonthsFixturesForTournament(CancellationToken cancellationToken, int providerTournamentId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var tournament = await GetEnabledTournamentForId(providerTournamentId);

            if (tournament != null)
            {
                var fixtures =
                        await _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                            tournament.ProviderTournamentId,
                            await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id),
                            cancellationToken);

                if (fixtures == null)
                    throw new NullReferenceException("Provider request failed due to timeout.");

                var upcomingFixtures = RemoveFixturesThatHaveBeenCompleted(fixtures);
                RugbyFixturesResponse oneMonthsfixtures = RemoveFixturesMoreThanAMonthFromNow(upcomingFixtures);

                await PersistFixturesData(cancellationToken, oneMonthsfixtures);

                _mongoDbRepository.Save(fixtures);
            }
        }

        private async Task<RugbyTournament> GetEnabledTournamentForId(int id)
        {
            var tournament = (await _rugbyService.GetCurrentTournaments()).FirstOrDefault(t => t.ProviderTournamentId == id);

            return tournament;
        }

        private RugbyFixturesResponse RemoveFixturesThatHaveBeenCompleted(RugbyFixturesResponse fixtures)
        {
            foreach (var round in fixtures.Fixtures.roundFixtures)
            {
                round.gameFixtures
                    .RemoveAll(
                        f => GetFixtureStatusFromProviderFixtureState(null, f.gameStateName) == RugbyFixtureStatus.Result);
            }

            return fixtures;
        }

        private RugbyFixturesResponse RemoveFixturesMoreThanAMonthFromNow(RugbyFixturesResponse fixtures)
        {
            DateTimeOffset nowPlusOneMonth = DateTimeOffset.UtcNow + TimeSpan.FromDays(31);
            foreach (var round in fixtures.Fixtures.roundFixtures)
            {
                round.gameFixtures.RemoveAll(f =>
                {
                    DateTimeOffset.TryParse(f.startTimeUTC, out DateTimeOffset startTime);
                    if (startTime > nowPlusOneMonth)
                        return true;

                    return false;
                });
            }

            return fixtures;
        }

        public async Task IngestLiveMatchData(CancellationToken cancellationToken, long providerFixtureId)
        {
            var fixtureInDb = _publicSportDataUnitOfWork.RugbyFixtures.FirstOrDefault(f => f.ProviderFixtureId == providerFixtureId);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (fixtureInDb == null)
                    return;

                var matchStatsResponse =
                    await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, providerFixtureId);

                if (matchStatsResponse == null)
                    continue;

                var eventsFlowResponse =
                    await _statsProzoneIngestService.IngestEventsFlow(cancellationToken, providerFixtureId);

                if (eventsFlowResponse == null)
                    continue;

                await IngestLineUpsForFixtures(cancellationToken, new List<RugbyFixture>() { fixtureInDb });
                await IngestGameTime(cancellationToken, matchStatsResponse, fixtureInDb);

                var playersForFixture = _publicSportDataUnitOfWork.RugbyPlayerLineups.Where(l => l.RugbyFixture.ProviderFixtureId == fixtureInDb.ProviderFixtureId).Select(l => l.RugbyPlayer).ToList();
                await IngestCommentary(cancellationToken, eventsFlowResponse.RugbyEventsFlow.commentaryFlow, fixtureInDb, playersForFixture);

                await IngestMatchStatisticsData(cancellationToken, matchStatsResponse, providerFixtureId);
                await IngestScoreData(cancellationToken, matchStatsResponse);
                await IngestFixtureStatusData(cancellationToken, matchStatsResponse, fixtureInDb);
                await UpdateSchedulerTrackingFixturesTable(fixtureInDb.Id, matchStatsResponse.RugbyMatchStats.gameState);

                await IngestEvents(cancellationToken, eventsFlowResponse, fixtureInDb);

                _mongoDbRepository.Save(matchStatsResponse);
                _mongoDbRepository.Save(eventsFlowResponse);

                //// Check if should stop looping?
                var matchState = GetFixtureStatusFromProviderFixtureState(fixtureInDb, matchStatsResponse.RugbyMatchStats.gameState);
                var schedulerState = FixturesStateHelper.GetSchedulerStateForFixture(DateTime.UtcNow, matchState, fixtureInDb.StartDateTime.DateTime);

                if (schedulerState == SchedulerStateForRugbyFixturePolling.SchedulingCompleted ||
                    schedulerState == SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted ||
                    schedulerState == SchedulerStateForRugbyFixturePolling.ResultOnlyPolling)
                    break;

                Thread.Sleep(5_000);
            }
        }

        private async Task IngestGameTime(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse, RugbyFixture rugbyFixture)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (matchStatsResponse.RugbyMatchStats?.gameInfo == null)
                return;

            rugbyFixture.GameTimeInSeconds = matchStatsResponse.RugbyMatchStats.gameInfo.gameSeconds;
            _publicSportDataUnitOfWork.RugbyFixtures.Update(rugbyFixture);

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task UpdateSchedulerTrackingFixturesTable(Guid fixtureId, string fixtureGameState)
        {
            var schedule = (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync())
                                .FirstOrDefault(s => s.FixtureId == fixtureId);

            var fixtureInDb = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync()).FirstOrDefault(f => f.Id == fixtureId);

            if (schedule == null)
            {
                return;
            }

            var fixtureState = GetFixtureStatusFromProviderFixtureState(fixtureInDb, fixtureGameState);
            schedule.RugbyFixtureStatus = fixtureState;
            schedule.SchedulerStateFixtures =
                FixturesStateHelper.GetSchedulerStateForFixture(DateTime.UtcNow, fixtureState, schedule.StartDateTime.DateTime);
            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Update(schedule);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task IngestEvents(CancellationToken cancellationToken, RugbyEventsFlowResponse eventsFlowResponse, RugbyFixture rugbyFixture)
        {
            var eventsToRemove = _publicSportDataUnitOfWork.RugbyMatchEvents.Where(e => e.RugbyFixture.Id == rugbyFixture.Id).ToList();

            IngestScoreEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.scoreFlow, rugbyFixture, ref eventsToRemove);
            IngestPenaltyEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.penaltyFlow, rugbyFixture, ref eventsToRemove);
            IngestErrorEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.errorFlow, rugbyFixture, ref eventsToRemove);
            IngestInterchangeEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.interchangeFlow, rugbyFixture, ref eventsToRemove);

            var count = eventsToRemove.Count;
            if (count > 0)
            {
                _publicSportDataUnitOfWork.RugbyMatchEvents.DeleteRange(eventsToRemove);
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private void IngestInterchangeEvents(CancellationToken cancellationToken, InterchangeFlow interchangeFlow, RugbyFixture rugbyFixture, ref List<RugbyMatchEvent> eventsToRemove)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var substitutionIn =
                _publicSportDataUnitOfWork.RugbyEventTypeProviderMappings.FirstOrDefault(e => e.ProviderEventName == "Substitution-In");

            var events = _publicSportDataUnitOfWork.RugbyMatchEvents.Where(e => 
                                e.RugbyFixture.Id == rugbyFixture.Id && 
                                e.RugbyEventTypeId == substitutionIn.RugbyEventTypeId).ToList();

            foreach (var team in interchangeFlow.teams)
            {
                var teamObject = rugbyFixture.TeamA.ProviderTeamId == team.teamId
                    ? rugbyFixture.TeamA
                    : rugbyFixture.TeamB;

                var playersForTeam = _publicSportDataUnitOfWork.RugbyPlayerLineups.Where(l => l.RugbyFixtureId == rugbyFixture.Id).Select(l => l.RugbyPlayer).ToList();

                var localInterchanges = new List<RugbyMatchEvent>();

                var interchanges = team.interchanges.Distinct(new InterchangeCompare());
                foreach (var interchange in interchanges)
                {
                    var eventInDb = events.FirstOrDefault(e =>
                        interchange?.off != null &&
                        interchange.on != null &&
                        e.RugbyEventTypeId == substitutionIn.RugbyEventTypeId &&
                        e.RugbyPlayer1 != null &&
                        e.RugbyPlayer1.ProviderPlayerId == interchange.off.playerId &&
                        e.RugbyPlayer2 != null &&
                        e.RugbyPlayer2.ProviderPlayerId == interchange.on.playerId);

                    // Check if we just added this event 
                    // but never call save yet.
                    if (eventInDb == null)
                    {
                        var locallyAddedEvent = 
                                localInterchanges.FirstOrDefault(e =>
                                    interchange?.off != null &&
                                    interchange.on != null &&
                                    e.RugbyEventTypeId == substitutionIn.RugbyEventTypeId &&
                                    e.RugbyPlayer1 != null &&
                                    e.RugbyPlayer1.ProviderPlayerId == interchange.off.playerId &&
                                    e.RugbyPlayer2 != null &&
                                    e.RugbyPlayer2.ProviderPlayerId == interchange.on.playerId);

                        if (locallyAddedEvent != null)
                            continue;
                    }

                    var newEvent = new RugbyMatchEvent()
                    {
                        EventValue = substitutionIn.ProviderEventTypeId,
                        GameTimeInSeconds = interchange.gameSeconds,
                        GameTimeInMinutes = (int)Math.Round(interchange.gameSeconds / 60.0),
                        RugbyFixture = rugbyFixture,
                        RugbyFixtureId = rugbyFixture.Id,
                        RugbyPlayer1 = playersForTeam.FirstOrDefault(p => p.ProviderPlayerId == interchange.off.playerId),
                        RugbyPlayer2 = playersForTeam.FirstOrDefault(p => p.ProviderPlayerId == interchange.on.playerId),
                        RugbyTeam = teamObject,
                        RugbyTeamId = teamObject.Id,
                        RugbyEventTypeId = substitutionIn.RugbyEventTypeId
                    };

                    if (eventInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyMatchEvents.Add(newEvent);
                        localInterchanges.Add(newEvent);
                    }
                    else
                    {
                        _publicSportDataUnitOfWork.RugbyMatchEvents.Update(eventInDb);
                        eventsToRemove.Remove(eventInDb);
                    }
                }
            }
        }

        private void IngestErrorEvents(CancellationToken cancellationToken, ErrorFlow errorFlow, RugbyFixture rugbyFixture, ref List<RugbyMatchEvent> eventsToRemove)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (errorFlow?.errorEvent?.statErrorEvent == null)
                return;

            var events = _publicSportDataUnitOfWork.RugbyMatchEvents.Where(e => e.RugbyFixture.Id == rugbyFixture.Id).ToList();
            var eventTypeProviderMappings = (_publicSportDataUnitOfWork.RugbyEventTypeProviderMappings.All()).ToList();

            foreach (var error in errorFlow.errorEvent.statErrorEvent)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var eventTypeMapping = eventTypeProviderMappings.FirstOrDefault(m => m.ProviderEventTypeId == error.statId);

                if (eventTypeMapping?.RugbyEventType == null)
                    return;

                var teamInDb = rugbyFixture.TeamA != null && rugbyFixture.TeamA.ProviderTeamId == error.teamId ? rugbyFixture.TeamA : rugbyFixture.TeamB;
                if (teamInDb == null || teamInDb.ProviderTeamId == 0) continue;

                var newEvent = new RugbyMatchEvent()
                {
                    EventValue = error.statValue,
                    GameTimeInSeconds = error.gameSeconds,
                    GameTimeInMinutes = (int)Math.Round(error.gameSeconds / 60.0),
                    RugbyFixture = rugbyFixture,
                    RugbyFixtureId = rugbyFixture.Id,
                    RugbyPlayer1 = null,
                    RugbyPlayer2 = null,
                    RugbyTeam = teamInDb,
                    RugbyTeamId = teamInDb.Id,
                    RugbyEventTypeId = eventTypeMapping.RugbyEventTypeId
                };

                // Try to do a lookup for an event. All the properties checked here might
                // change by the provider and end up with a duplicate entry in the db
                // One with the correct event data and the other with incorrect data.
                // This is because there isn't a unique id provided for the event by the provider.
                var eventInDb = events.FirstOrDefault(e =>
                    e.RugbyFixtureId == newEvent.RugbyFixtureId &&
                    e.RugbyTeamId == newEvent.RugbyTeamId &&
                    e.RugbyEventTypeId == newEvent.RugbyEventTypeId &&
                    e.GameTimeInSeconds == newEvent.GameTimeInSeconds);

                if (eventInDb == null)
                {
                    _publicSportDataUnitOfWork.RugbyMatchEvents.Add(newEvent);
                }
                else
                {
                    _publicSportDataUnitOfWork.RugbyMatchEvents.Update(eventInDb);
                    eventsToRemove.Remove(eventInDb);
                }
            }
        }

        private void IngestPenaltyEvents(CancellationToken cancellationToken, PenaltyFlow penaltyFlow, RugbyFixture rugbyFixture, ref List<RugbyMatchEvent> eventsToRemove)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var penalties = penaltyFlow?.penaltyEvent?.statPenaltyEvent;
            if (penalties == null)
                return;

            var events = _publicSportDataUnitOfWork.RugbyMatchEvents.Where(e => e.RugbyFixture.Id == rugbyFixture.Id).ToList();
            var eventTypeProviderMappings = (_publicSportDataUnitOfWork.RugbyEventTypeProviderMappings.All()).ToList();

            foreach (var penaltyEvent in penalties)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                // Can we find a mapping for this event in the db?
                var eventTypeMapping = eventTypeProviderMappings.FirstOrDefault(m => m.ProviderEventTypeId == penaltyEvent.statId);

                if (eventTypeMapping?.RugbyEventType == null)
                    return;

                var teamInDb = rugbyFixture.TeamA != null && rugbyFixture.TeamA.ProviderTeamId == penaltyEvent.teamId ? rugbyFixture.TeamA : rugbyFixture.TeamB;
                if (teamInDb == null || teamInDb.ProviderTeamId == 0) continue;

                var newEvent = new RugbyMatchEvent()
                {
                    EventValue = penaltyEvent.statValue,
                    GameTimeInSeconds = penaltyEvent.gameSeconds,
                    GameTimeInMinutes = (int)Math.Round(penaltyEvent.gameSeconds / 60.0),
                    RugbyFixture = rugbyFixture,
                    RugbyFixtureId = rugbyFixture.Id,
                    RugbyPlayer1 = null,
                    RugbyPlayer2 = null,
                    RugbyTeam = teamInDb,
                    RugbyTeamId = teamInDb.Id,
                    RugbyEventTypeId = eventTypeMapping.RugbyEventTypeId
                };

                // Try to do a lookup for an event. All the properties checked here might
                // change by the provider and end up with a duplicate entry in the db
                // One with the correct event data and the other with incorrect data.
                // This is because there isn't a unique id provided for the event by the provider.
                var eventInDb = events.FirstOrDefault(e =>
                    e.RugbyFixtureId == newEvent.RugbyFixtureId &&
                    e.RugbyTeamId == newEvent.RugbyTeamId &&
                    e.RugbyEventTypeId == newEvent.RugbyEventTypeId &&
                    e.GameTimeInSeconds == newEvent.GameTimeInSeconds);

                if (eventInDb == null)
                {
                    _publicSportDataUnitOfWork.RugbyMatchEvents.Add(newEvent);
                }
                else
                {
                    _publicSportDataUnitOfWork.RugbyMatchEvents.Update(eventInDb);
                    eventsToRemove.Remove(eventInDb);
                }
            }
        }

        private void IngestScoreEvents(CancellationToken cancellationToken, ScoreFlow scoreFlow, RugbyFixture rugbyFixture, ref List<RugbyMatchEvent> eventsToRemove)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var teams = scoreFlow?.scoreEvent?.teams[0];
            if (teams == null)
                return;

            var events = _publicSportDataUnitOfWork.RugbyMatchEvents.Where(e => e.RugbyFixture.Id == rugbyFixture.Id).ToList();
            var eventTypeProviderMappings = (_publicSportDataUnitOfWork.RugbyEventTypeProviderMappings.All()).ToList();

            var players = _publicSportDataUnitOfWork.RugbyPlayerLineups.Where(l => l.RugbyFixture.Id == rugbyFixture.Id).Select(l => l.RugbyPlayer).ToList();

            foreach (var team in teams.team)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamInDb = rugbyFixture.TeamA != null && rugbyFixture.TeamA.ProviderTeamId == team.teamId ? rugbyFixture.TeamA : rugbyFixture.TeamB;
                if (teamInDb == null || teamInDb.ProviderTeamId == 0) continue;

                var scoreEvents = team.statScoringEvent;
                if (scoreEvents == null)
                    continue;

                foreach (var scoreEvent in scoreEvents)
                {
                    // Can we find a mapping for this event in the db?
                    var eventTypeMapping = eventTypeProviderMappings.FirstOrDefault(m => m.ProviderEventTypeId == scoreEvent.statId);

                    if (eventTypeMapping?.RugbyEventType == null)
                        return;

                    var player = players.FirstOrDefault(p => p.ProviderPlayerId == scoreEvent.playerId);

                    var newEvent = new RugbyMatchEvent()
                    {
                        EventValue = (float)scoreEvent.statValue,
                        GameTimeInSeconds = scoreEvent.gameSeconds,
                        GameTimeInMinutes = (int)Math.Round(scoreEvent.gameSeconds / 60.0),
                        RugbyFixture = rugbyFixture,
                        RugbyFixtureId = rugbyFixture.Id,
                        RugbyPlayer1 = player,
                        RugbyPlayer2 = null,
                        RugbyTeam = teamInDb,
                        RugbyTeamId = teamInDb.Id,
                        RugbyEventTypeId = eventTypeMapping.RugbyEventType.Id
                    };

                    // Try to do a lookup for an event. All the properties checked here might
                    // change by the provider and end up with a duplicate entry in the db
                    // One with the correct event data and the other with incorrect data.
                    // This is because there isn't a unique id provided for the event by the provider.
                    var eventInDb = events.FirstOrDefault(e =>
                        e.RugbyFixtureId == newEvent.RugbyFixtureId &&
                        e.RugbyTeamId == newEvent.RugbyTeamId &&
                        e.RugbyEventTypeId == newEvent.RugbyEventTypeId &&
                        e.GameTimeInSeconds == newEvent.GameTimeInSeconds);

                    if (eventInDb == null)
                    {
                        _publicSportDataUnitOfWork.RugbyMatchEvents.Add(newEvent);
                    }
                    else
                    {
                        _publicSportDataUnitOfWork.RugbyMatchEvents.Update(eventInDb);
                        eventsToRemove.Remove(eventInDb);
                    }
                }
            }
        }

        private async Task IngestFixtureStatusData(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse, RugbyFixture rugbyFixture)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixtureState = GetFixtureStatusFromProviderFixtureState(rugbyFixture, matchStatsResponse.RugbyMatchStats.gameState);

            if (rugbyFixture != null)
            {
                if (!rugbyFixture.CmsOverrideModeIsActive)
                {
                    rugbyFixture.RugbyFixtureStatus = fixtureState;
                    _publicSportDataUnitOfWork.RugbyFixtures.Update(rugbyFixture);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task IngestScoreData(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var allFixtures = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync());

            var scores = GetScoresForFixture(cancellationToken, matchStatsResponse);
            var fixtureId = matchStatsResponse.RugbyMatchStats.gameId;

            var fixtureInDb = allFixtures.FirstOrDefault(f => f.ProviderFixtureId == fixtureId);

            if (fixtureInDb == null)
            {
                // Is this even possible?
                // To be ingesting scores for a fixture that doesnt
                // exist in the DB.
            }
            else
            {
                if (!fixtureInDb.CmsOverrideModeIsActive)
                {
                    fixtureInDb.TeamAScore = scores.teamAScore;
                    fixtureInDb.TeamBScore = scores.teamBScore;
                    _publicSportDataUnitOfWork.RugbyFixtures.Update(fixtureInDb);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private (int teamAScore, int teamBScore) GetScoresForFixture(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return (0, 0);

            if (matchStatsResponse.RugbyMatchStats == null)
                return (0, 0);

            var teams = matchStatsResponse.RugbyMatchStats.teams;
            var teamAScore = teams.teamsMatch[0].score.points;
            var teamBScore = teams.teamsMatch[1].score.points;

            return (teamAScore, teamBScore);
        }

        private async Task IngestMatchStatisticsData(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixture = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync()).FirstOrDefault(f => f.ProviderFixtureId == providerFixtureId);
            var teamsInRepo = (await _publicSportDataUnitOfWork.RugbyTeams.AllAsync()).ToList();
            var matchStatistics = (await _publicSportDataUnitOfWork.RugbyMatchStatistics.AllAsync()).ToList();

            var teamsFromProvider = matchStatsResponse.RugbyMatchStats.teams;

            foreach (var teamMatch in teamsFromProvider.teamsMatch)
            {
                var team = teamsInRepo.FirstOrDefault(t => t.ProviderTeamId == teamMatch.teamId);
                if (team == null) continue;

                var stats = teamMatch.teamStats.matchStats.matchStat;
                var statsInDb = matchStatistics.FirstOrDefault(s => (fixture != null && (s.RugbyFixtureId == fixture.Id && s.RugbyTeamId == team.Id)));

                var statsMap = MakeStatisticsMap(stats);
                if (fixture == null) continue;

                var newStats = new RugbyMatchStatistics()
                {
                    RugbyFixture = fixture,
                    RugbyFixtureId = fixture.Id,
                    RugbyTeam = team,
                    RugbyTeamId = team.Id,
                    YellowCards = (int)statsMap.GetValueOrDefault(2),
                    CleanBreaks = (int)statsMap.GetValueOrDefault(7),
                    ConversionAttempts = (int)statsMap.GetValueOrDefault(2041),
                    Conversions = (int)statsMap.GetValueOrDefault(9),
                    ConversionsMissed = (int)statsMap.GetValueOrDefault(10),
                    DefendersBeaten = (int)statsMap.GetValueOrDefault(8),
                    DropGoalAttempts = (int)statsMap.GetValueOrDefault(2043),
                    DropGoals = (int)statsMap.GetValueOrDefault(13),
                    DropGoalsMissed = (int)statsMap.GetValueOrDefault(14),
                    LineOutsLost = (int)statsMap.GetValueOrDefault(20),
                    LineOutsWon = (int)statsMap.GetValueOrDefault(19),
                    Offloads = (int)statsMap.GetValueOrDefault(46),
                    Passes = (int)statsMap.GetValueOrDefault(2012),
                    Penalties = (int)statsMap.GetValueOrDefault(11),
                    PenaltiesConceded = (int)statsMap.GetValueOrDefault(2079),
                    PenaltiesMissed = (int)statsMap.GetValueOrDefault(12),
                    PenaltyAttempts = (int)statsMap.GetValueOrDefault(2038),
                    PenaltyTries = (int)statsMap.GetValueOrDefault(10530),
                    Possession = (int)statsMap.GetValueOrDefault(42),
                    RedCards = (int)statsMap.GetValueOrDefault(3),
                    ScrumsLost = (int)statsMap.GetValueOrDefault(55),
                    ScrumsWon = (int)statsMap.GetValueOrDefault(53),
                    Tackles = (int)statsMap.GetValueOrDefault(2055),
                    TacklesMissed = (int)statsMap.GetValueOrDefault(71),
                    Territory = (int)statsMap.GetValueOrDefault(10000),
                    Tries = (int)statsMap.GetValueOrDefault(2035)
                };

                if (statsInDb == null)
                {
                    _publicSportDataUnitOfWork.RugbyMatchStatistics.Add(newStats);
                }
                else
                {
                    statsInDb.YellowCards = newStats.YellowCards;
                    statsInDb.CarriesCrossedGainLine = newStats.CarriesCrossedGainLine;
                    statsInDb.CleanBreaks = newStats.CleanBreaks;
                    statsInDb.ConversionAttempts = newStats.ConversionAttempts;
                    statsInDb.Conversions = newStats.Conversions;
                    statsInDb.ConversionsMissed = newStats.ConversionsMissed;
                    statsInDb.DefendersBeaten = newStats.DefendersBeaten;
                    statsInDb.DropGoalAttempts = newStats.DropGoalAttempts;
                    statsInDb.DropGoals = newStats.DropGoals;
                    statsInDb.DropGoalsMissed = newStats.DropGoalsMissed;
                    statsInDb.LineOutsLost = newStats.LineOutsLost;
                    statsInDb.LineOutsWon = newStats.LineOutsWon;
                    statsInDb.Offloads = newStats.Offloads;
                    statsInDb.Passes = newStats.Passes;
                    statsInDb.Penalties = newStats.Penalties;
                    statsInDb.PenaltiesConceded = newStats.PenaltiesConceded;
                    statsInDb.PenaltiesMissed = newStats.PenaltiesMissed;
                    statsInDb.PenaltyAttempts = newStats.PenaltyAttempts;
                    statsInDb.PenaltyTries = newStats.PenaltyTries;
                    statsInDb.Possession = newStats.Possession;
                    statsInDb.RedCards = newStats.RedCards;
                    statsInDb.ScrumsLost = newStats.ScrumsLost;
                    statsInDb.ScrumsWon = newStats.ScrumsWon;
                    statsInDb.Tackles = newStats.Tackles;
                    statsInDb.TacklesMissed = newStats.TacklesMissed;
                    statsInDb.Territory = newStats.Territory;
                    statsInDb.Tries = newStats.Tries;

                    _publicSportDataUnitOfWork.RugbyMatchStatistics.Update(statsInDb);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private IDictionary<int, double> MakeStatisticsMap(IList<MatchStat> matchStats)
        {
            return matchStats.ToDictionary(stat => stat.StatTypeID, stat => stat.StatValue);
        }

        private async Task IngestCommentary(CancellationToken cancellationToken, CommentaryFlow commentary, RugbyFixture fixture, IList<RugbyPlayer> players)
        {
            if (commentary?.commentaryEvent == null)
                return;

            var commentaryForThisfixture = _publicSportDataUnitOfWork.RugbyCommentaries
                                                .Where(c => c.RugbyFixture.ProviderFixtureId == fixture.ProviderFixtureId).ToList();

            var commentariesThatShouldBeRemovedFromDb = commentaryForThisfixture;

            foreach (var comment in commentary.commentaryEvent)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var commentText = comment.commentary;
                var commentTimeInSeconds = comment.gameSeconds;
                var commentaryTimeInMinutes = (int)Math.Round(commentTimeInSeconds / 60.0);
                var gameTimeDisplayHoursMinutesSeconds = comment.gameTime;
                var gameTimeDisplayMinutesSeconds = comment.GameMinutes;

                var team = fixture.TeamA != null && fixture.TeamA.ProviderTeamId == comment.teamId ? fixture.TeamA : fixture.TeamB;

                var player = players.FirstOrDefault(p => p.ProviderPlayerId == comment.playerId);

                var dbCommentary = commentaryForThisfixture.FirstOrDefault(c =>
                                                c.GameTimeRawSeconds == commentTimeInSeconds &&
                                                c.CommentaryText == commentText &&
                                                c.RugbyFixture.Id == fixture.Id &&
                                                c.RugbyPlayer == player &&
                                                c.RugbyTeam == team);

                var newCommentary = new RugbyCommentary()
                {
                    CommentaryText = commentText,
                    GameTimeDisplayHoursMinutesSeconds = gameTimeDisplayHoursMinutesSeconds,
                    GameTimeDisplayMinutesSeconds = gameTimeDisplayMinutesSeconds,
                    GameTimeRawMinutes = commentaryTimeInMinutes,
                    GameTimeRawSeconds = commentTimeInSeconds,
                    RugbyFixture = fixture,
                    RugbyPlayer = player,
                    RugbyTeam = team
                };

                if (dbCommentary == null)
                {
                    _publicSportDataUnitOfWork.RugbyCommentaries.Add(newCommentary);
                    // Add the commentary to the local list.
                }
                else
                {
                    commentariesThatShouldBeRemovedFromDb.Remove(dbCommentary);
                }
            }

            if (commentariesThatShouldBeRemovedFromDb.Count > 0)
            {
                _publicSportDataUnitOfWork.RugbyCommentaries.DeleteRange(commentariesThatShouldBeRemovedFromDb);
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task IngestLineupsForUpcomingGames(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var now = DateTime.UtcNow;
            var numberOfDays = DateTime.UtcNow + TimeSpan.FromDays(int.Parse(ConfigurationManager.AppSettings["NumberOfDaysForUpcomingLineups"]));

            var gamesInTheNext2Days =
                    (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync())
                        .Where(
                            fixture => fixture.RugbyTournament != null &&
                                       fixture.RugbyTournament.IsEnabled &&
                                       fixture.StartDateTime >= now &&
                                       fixture.StartDateTime <= numberOfDays).ToList();

            await IngestLineUpsForFixtures(cancellationToken, gamesInTheNext2Days);
        }

        private async Task IngestLineUpsForFixtures(CancellationToken cancellationToken, IEnumerable<RugbyFixture> rugbyFixtures)
        {
            foreach (var fixture in rugbyFixtures)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var fixtureId = fixture.ProviderFixtureId;

                var matchStatsResponse =
                    await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, fixtureId);

                if (matchStatsResponse == null)
                    continue;

                try
                {
                    await IngestPlayerLineups(cancellationToken, matchStatsResponse, fixture);
                }
                catch (Exception e)
                {
                }
            }
        }

        private async Task IngestPlayerLineups(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse, RugbyFixture fixture)
        {
            if (matchStatsResponse?.RugbyMatchStats?.teams?.teamsMatch == null)
                return;

            // Do we have provider info?
            if (matchStatsResponse.RugbyMatchStats.teams.teamsMatch.Count == 0)
                return;

            var lineupsInDb = _publicSportDataUnitOfWork.RugbyPlayerLineups.Where(l => l.RugbyFixture.ProviderFixtureId == fixture.ProviderFixtureId).ToList();
          
            var lineupsToRemoveFromDb = lineupsInDb;

            var localLineups = new List<RugbyPlayerLineup>();

            foreach (var squad in matchStatsResponse.RugbyMatchStats.teams.teamsMatch)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var lineup = squad.teamLineup;

                if (lineup?.teamPlayer == null)
                    continue;

                var players = lineup.teamPlayer.ToList().Distinct(new TeamPlayerComparer()).ToList();

                var playersForFixture = (await _publicSportDataUnitOfWork.RugbyPlayers.AllAsync()).Where(p => players.Any(player => player.playerId.Equals(p.ProviderPlayerId))).ToList();

                var teamId = squad.teamId;
                var dbTeam = _publicSportDataUnitOfWork.RugbyTeams.FirstOrDefault(p => p.ProviderTeamId == teamId);

                if (dbTeam == null || dbTeam.ProviderTeamId == 0)
                    continue;

                foreach (var player in players)
                {
                    var playerId = player.playerId;
                    var dbPlayer = playersForFixture.FirstOrDefault(p => p.ProviderPlayerId == playerId);

                    if (dbPlayer == null)
                        break;

                    if (dbPlayer.FirstName == null && dbPlayer.LastName == null)
                    {
                        dbPlayer.FirstName = player.playerFirstName;
                        dbPlayer.LastName = player.playerLastName;
                        _publicSportDataUnitOfWork.RugbyPlayers.Update(dbPlayer);
                    }

                    var shirtNumber = player.shirtNum;
                    var positionName = player.playerPosition;

                    var isCaptain = player.isCaptain ?? false;
                    var isSubstitute = player.shirtNum >= 16;

                    var dbEntry =
                            lineupsInDb
                                .FirstOrDefault(l =>
                                    l.RugbyPlayerId == dbPlayer.Id &&
                                    l.RugbyFixtureId == fixture.Id &&
                                    l.RugbyTeamId == dbTeam.Id);

                    // Check if we just added this lineup 
                    // but never call save yet.
                    if (dbEntry == null)
                    {
                        var localEntry =
                            localLineups
                                .FirstOrDefault(l =>
                                    l.RugbyPlayerId == dbPlayer.Id &&
                                    l.RugbyFixtureId == fixture.Id &&
                                    l.RugbyTeamId == dbTeam.Id);

                        if (localEntry != null)
                            break;
                    }

                    if (dbEntry == null)
                    {
                        var newEntry = new RugbyPlayerLineup()
                        {
                            RugbyPlayerId = dbPlayer.Id,
                            RugbyFixtureId = fixture.Id,
                            RugbyTeamId = dbTeam.Id,
                            RugbyFixture = fixture,
                            RugbyTeam = dbTeam,
                            RugbyPlayer = dbPlayer,
                            IsCaptain = isCaptain,
                            IsSubstitute = isSubstitute,
                            PositionName = positionName,
                            JerseyNumber = shirtNumber
                        };

                        _publicSportDataUnitOfWork.RugbyPlayerLineups.Add(newEntry);
                        localLineups.Add(newEntry);
                    }
                    else
                    {
                        dbEntry.IsSubstitute = isSubstitute;
                        dbEntry.PositionName = positionName;
                        dbEntry.JerseyNumber = shirtNumber;

                        _publicSportDataUnitOfWork.RugbyPlayerLineups.Update(dbEntry);

                        // Should this lineup entry remain in the db?
                        lineupsToRemoveFromDb.Remove(dbEntry);
                    }
                }
            }

            if (lineupsToRemoveFromDb.Count > 0)
            {
                _publicSportDataUnitOfWork.RugbyPlayerLineups.DeleteRange(lineupsToRemoveFromDb);
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task IngestLogsForTournamentSeason(CancellationToken cancellationToken, int providerTournamentId, int seasonId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var seasons = (await _publicSportDataUnitOfWork.RugbySeasons.AllAsync());

            var season =
                    seasons
                        .Where(s =>
                            s.ProviderSeasonId == seasonId &&
                            s.RugbyTournament.ProviderTournamentId == providerTournamentId).ToList();

            if (!season.Any())
                return;

            var numberOfRounds = season.First().CurrentRoundNumberCmsOverride ?? season.First().CurrentRoundNumber;

            var logType = season.First().RugbyLogType;

            if (logType == RugbyLogType.FlatLogs)
            {
                var logs = 
                    await _statsProzoneIngestService.IngestFlatLogsForTournament(
                        providerTournamentId, seasonId, numberOfRounds);

                if (logs == null)
                    return;

                await PersistFlatLogs(cancellationToken, logs);

                _mongoDbRepository.Save(logs);
            }
            else
            {
                var logs = 
                    await _statsProzoneIngestService.IngestGroupedLogsForTournament(
                        providerTournamentId, seasonId, numberOfRounds);

                if (logs == null)
                    return;

                await PersistGroupedLogs(cancellationToken, logs);

                _mongoDbRepository.Save(logs);
            }
        }

        public async Task IngestLineupsForPastGames(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var now = DateTime.UtcNow;
            var numberOfPastDays = DateTime.UtcNow - TimeSpan.FromDays(int.Parse(ConfigurationManager.AppSettings["NumberOfDaysForPastLineups"]));

            var gamesInPastDay =
                    (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync())
                        .Where(
                            fixture => fixture.RugbyTournament != null &&
                                       fixture.RugbyTournament.IsEnabled &&
                                       fixture.StartDateTime < now &&
                                       fixture.StartDateTime >= numberOfPastDays &&
                                       (fixture.RugbyFixtureStatus == RugbyFixtureStatus.Result));

            await IngestLineUpsForFixtures(cancellationToken, gamesInPastDay);
        }

        public async Task IngestLiveMatchDataForPastFixtures(CancellationToken cancellationToken)
        {
            var pastFixtures = (await _rugbyService.GetFixturesNotIngestedYet()).ToList();

            await IngestPastLiveData(cancellationToken, pastFixtures);
        }

        public async Task IngestLiveMatchDataForPastFewDaysFixtures(CancellationToken cancellationToken)
        {
            var pastFixtures = (await _rugbyService.GetPastDaysFixtures(int.Parse(ConfigurationManager.AppSettings["NumberOfDaysForPastFixtures"]))).ToList();

            await IngestPastLiveData(cancellationToken, pastFixtures);
        }

        private async Task IngestPastLiveData(CancellationToken cancellationToken, List<RugbyFixture> pastFixtures)
        {
            foreach (var fixture in pastFixtures)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    Stopwatch provider = Stopwatch.StartNew();

                    var providerFixtureId = fixture.ProviderFixtureId;

                    var matchStatsResponse =
                        await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, providerFixtureId);

                    if (matchStatsResponse == null)
                        continue;

                    var eventsFlowResponse =
                        await _statsProzoneIngestService.IngestEventsFlow(cancellationToken, providerFixtureId);

                    if (eventsFlowResponse == null)
                        continue;

                    provider.Stop();

                    Stopwatch s = Stopwatch.StartNew();

                    await IngestLineUpsForFixtures(cancellationToken, new List<RugbyFixture>() { fixture });

                    var playersForFixture = _publicSportDataUnitOfWork.RugbyPlayerLineups.Where(l => l.RugbyFixture.ProviderFixtureId == fixture.ProviderFixtureId).Select(l => l.RugbyPlayer).ToList();
                    await IngestGameTime(cancellationToken, matchStatsResponse, fixture);
                    await IngestCommentary(cancellationToken, eventsFlowResponse.RugbyEventsFlow.commentaryFlow, fixture, playersForFixture);
                    await IngestMatchStatisticsData(cancellationToken, matchStatsResponse, providerFixtureId);
                    await IngestScoreData(cancellationToken, matchStatsResponse);
                    await IngestFixtureStatusData(cancellationToken, matchStatsResponse, fixture);
                    await UpdateSchedulerTrackingFixtureToSchedulingCompleted(cancellationToken, fixture.Id, matchStatsResponse.RugbyMatchStats.gameState);
                    await IngestEvents(cancellationToken, eventsFlowResponse, fixture);

                    _mongoDbRepository.Save(matchStatsResponse);
                    _mongoDbRepository.Save(eventsFlowResponse);

                    s.Stop();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private async Task UpdateSchedulerTrackingFixtureToSchedulingCompleted(CancellationToken cancellationToken, Guid fixtureId, string gameState)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var schedule = (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync()).FirstOrDefault(s => s.FixtureId == fixtureId);
            var fixtureInDb = (await _publicSportDataUnitOfWork.RugbyFixtures.AllAsync()).FirstOrDefault(f => f.Id == fixtureId);

            if (schedule == null)
            {
                return;
            }

            var fixtureState = GetFixtureStatusFromProviderFixtureState(fixtureInDb, gameState);
            schedule.RugbyFixtureStatus = fixtureState;
            if (schedule.StartDateTime < DateTimeOffset.UtcNow)
            {
                schedule.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted;
            }
            
            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Update(schedule);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task IngestPlayerStatsForCurrentTournaments(int providerTournamentId, int providerSeasonId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            var response =
                await _statsProzoneIngestService.IngestPlayerStatsForTournament(
                    providerTournamentId, 
                    providerSeasonId,
                    cancellationToken);

            foreach (var player in response.RugbyPlayerStats.players)
            {
                var playerInDb =
                    _publicSportDataUnitOfWork.RugbyPlayers.FirstOrDefault(p => p.ProviderPlayerId == player.playerId);

                if (playerInDb == null)
                    continue;

                var teamInDb =
                    _publicSportDataUnitOfWork.RugbyTeams.FirstOrDefault(t => t.ProviderTeamId == player.teamId);

                if (teamInDb == null)
                    continue;

                var seasonInDb = 
                    _publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(s => 
                    s.RugbyTournament.ProviderTournamentId == providerTournamentId &&
                    s.ProviderSeasonId == providerSeasonId);

                if (seasonInDb == null)
                    continue;

                var conversions =
                    player?.playerSeasonStats?.Stat?.FirstOrDefault(s => s.StatTypeID == 2)?.totalValue;
                if (conversions is null) conversions = 0;

                var tries =
                    player?.playerSeasonStats?.Stat?.FirstOrDefault(s => s.StatTypeID == 5)?.totalValue;
                if (tries is null) tries = 0;

                // STATS combines these values :( "Number of Penalty Shots/Conversions/Drop Goals Made"
                var dropGoals =
                    player?.playerSeasonStats?.Stat?.FirstOrDefault(s => s.StatTypeID == 2050)?.totalValue;
                if (dropGoals is null) dropGoals = 0;

                var penalties = dropGoals;

                var points =
                    player?.playerSeasonStats?.Stat?.FirstOrDefault(s => s.StatTypeID == 2001)?.totalValue;
                if (points is null) points = 0;

                var tournament =
                    _publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(t => t.ProviderTournamentId == providerTournamentId);

                var playerStatistics = new RugbyPlayerStatistics
                {
                    RugbyTournament = tournament,
                    RugbyTournamentId = tournament.Id,
                    RugbyPlayer = playerInDb,
                    RugbyPlayerId = playerInDb.Id,
                    RugbyTeam = teamInDb,
                    RugbyTeamId = teamInDb.Id,
                    RugbySeason = seasonInDb,
                    RugbySeasonId = seasonInDb.Id,
                    TriesScored = (int) tries,
                    ConversionsScored = (int) conversions,
                    DropGoalsScored = (int) dropGoals,
                    PenaltiesScored = (int) penalties,
                    TotalPoints = (int) points
                };

                var statInRepo = _publicSportDataUnitOfWork.RugbyPlayerStatistics
                    .FirstOrDefault(s => s.RugbyPlayerId == playerStatistics.RugbyPlayerId
                                         && s.RugbySeasonId == playerStatistics.RugbySeasonId
                                         && s.RugbyTournamentId == playerStatistics.RugbyTournamentId);

                if (statInRepo != null)
                {
                    statInRepo.TriesScored = playerStatistics.TriesScored;
                    statInRepo.ConversionsScored = playerStatistics.ConversionsScored;
                    statInRepo.DropGoalsScored = playerStatistics.DropGoalsScored;
                    statInRepo.PenaltiesScored = playerStatistics.PenaltiesScored;
                    statInRepo.TotalPoints = playerStatistics.TotalPoints;

                    _publicSportDataUnitOfWork.RugbyPlayerStatistics.Update(statInRepo);
                }
                else
                {
                    _publicSportDataUnitOfWork.RugbyPlayerStatistics.Add(playerStatistics);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }
    }
}
