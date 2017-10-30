namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Collections.Generic;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System.Text.RegularExpressions;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Extensions;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
    using SuperSportDataEngine.ApplicationLogic.Constants;
    using SuperSportDataEngine.ApplicationLogic.Helpers;

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyVenue> _rugbyVenueRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTeam> _rugbyTeamRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixturesRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyPlayer> _rugbyPlayerRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFlatLog> _rugbyFlatLogsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyLogGroup> _rugbyLogGroupRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyGroupedLog> _rugbyGroupedLogsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyPlayerLineup> _rugbyPlayerLineupsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyCommentary> _rugbyCommentaryRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyMatchStatistics> _rugbyMatchStatisticsRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyMatchEvent> _rugbyMatchEventsRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> _rugbyEventTypeMappingRepository;
        private readonly IRugbyService _rugbyService;

        public RugbyIngestWorkerService(
            IStatsProzoneRugbyIngestService statsProzoneIngestService,
            IMongoDbRugbyRepository mongoDbRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> schedulerTrackingRugbyFixtureRepoitory,
            IBaseEntityFrameworkRepository<RugbyVenue> rugbyVenueRepository,
            IBaseEntityFrameworkRepository<RugbyTeam> rugbyTeamRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixturesRepository,
            IBaseEntityFrameworkRepository<RugbyPlayer> rugbyPlayerRepository,
            IBaseEntityFrameworkRepository<RugbyFlatLog> rugbyFlatLogsRepository,
            IBaseEntityFrameworkRepository<RugbyLogGroup> rugbyLogGroupRepository,
            IBaseEntityFrameworkRepository<RugbyGroupedLog> rugbyGroupedLogsRepository,
            IBaseEntityFrameworkRepository<RugbyPlayerLineup> rugbyPlayerLineupsRepository,
            IBaseEntityFrameworkRepository<RugbyCommentary> rugbyCommentaryRepository,
            IBaseEntityFrameworkRepository<RugbyMatchStatistics> rugbyMatchStatisticsRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbyMatchEvent> rugbyMatchEventsRepository,
            IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> rugbyEventTypeMappingRepository,
            IRugbyService rugbyService)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _schedulerTrackingRugbyFixtureRepository = schedulerTrackingRugbyFixtureRepoitory;
            _rugbyVenueRepository = rugbyVenueRepository;
            _rugbyTeamRepository = rugbyTeamRepository;
            _rugbyFixturesRepository = rugbyFixturesRepository;
            _rugbyPlayerRepository = rugbyPlayerRepository;
            _rugbyFlatLogsRepository = rugbyFlatLogsRepository;
            _rugbyLogGroupRepository = rugbyLogGroupRepository;
            _rugbyGroupedLogsRepository = rugbyGroupedLogsRepository;
            _rugbyPlayerLineupsRepository = rugbyPlayerLineupsRepository;
            _rugbyCommentaryRepository = rugbyCommentaryRepository;
            _rugbyMatchStatisticsRepository = rugbyMatchStatisticsRepository;
            _schedulerTrackingRugbyTournamentRepository = schedulerTrackingRugbyTournamentRepository;
            _rugbyMatchEventsRepository = rugbyMatchEventsRepository;
            _rugbyEventTypeMappingRepository = rugbyEventTypeMappingRepository;
            _rugbyService = rugbyService;
        }

        public async Task IngestReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var entitiesResponse =
                _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            await PersistVenuesInRepository(cancellationToken, entitiesResponse);
            await PersistTeamsInRepository(cancellationToken, entitiesResponse);
            await PersistTournamentsInRepository(cancellationToken, entitiesResponse);
            await PersistPlayerDataInRepository(cancellationToken, entitiesResponse);
            await PersistTournamentSeasonsInRepository(cancellationToken);

            _mongoDbRepository.SaveEntities(entitiesResponse);
        }

        private async Task PersistPlayerDataInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            var playersAlreadyInDb = await _rugbyPlayerRepository.AllAsync();

            foreach (var player in entitiesResponse.Entities.players)
            {
                var playerInDb = playersAlreadyInDb.Where(p => p.ProviderPlayerId == player.id).FirstOrDefault();

                var newPlayer = new RugbyPlayer()
                {
                    FirstName = null,
                    LastName = null,
                    FullName = player.name,
                    ProviderPlayerId = player.id,
                    LegacyPlayerId = player.id,
                    DataProvider = DataProvider.StatsProzone
                };

                if (playerInDb == null)
                {
                    _rugbyPlayerRepository.Add(newPlayer);
                }
            }

            await _rugbyPlayerRepository.SaveAsync();
        }

        private async Task PersistRugbyTournamentsInSchedulerTrackingRugbyTournamentTable(CancellationToken cancellationToken)
        {
            var activeTournaments = await _rugbyService.GetActiveTournaments();
            var seasonsAlreadyInDb = await _schedulerTrackingRugbyTournamentRepository.AllAsync();

            foreach (var tournament in activeTournaments)
            {
                var currentSeasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var seasonInDb =
                    _schedulerTrackingRugbySeasonRepository
                        .Where(s => s.TournamentId == tournament.Id && s.RugbySeasonStatus == RugbySeasonStatus.InProgress).FirstOrDefault();

                if (seasonInDb != null)
                {
                    var tournamentInDb =
                        seasonsAlreadyInDb
                            .Where(t => t.TournamentId == tournament.Id && t.SeasonId == seasonInDb.SeasonId).FirstOrDefault();

                    var newTournament = new SchedulerTrackingRugbyTournament()
                    {
                        SeasonId = seasonInDb.SeasonId,
                        TournamentId = tournament.Id,
                        SchedulerStateLogs = SchedulerStateForRugbyLogPolling.NotRunning,
                        SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning
                    };

                    if (tournamentInDb == null)
                    {
                        _schedulerTrackingRugbyTournamentRepository.Add(newTournament);
                    }
                }
            }

            await _schedulerTrackingRugbyTournamentRepository.SaveAsync();
        }

        private async Task PersistTeamsInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var teamsAlreadyInDb = await _rugbyTeamRepository.AllAsync();

            foreach (var team in entitiesResponse.Entities.teams)
            {
                var teamInDb = teamsAlreadyInDb.Where(t => t.ProviderTeamId == team.id).FirstOrDefault();

                if (teamInDb == null)
                {
                    var newTeam = new RugbyTeam()
                    {
                        ProviderTeamId = team.id,
                        Name = team.name,
                        LogoUrl = team.TeamLogoURL,
                        LegacyTeamId = team.id,
                        Abbreviation = team.TeamAbbrev,
                        DataProvider = DataProvider.StatsProzone
                    };
                    _rugbyTeamRepository.Add(newTeam);
                }
                else
                {
                    teamInDb.Name = team.name;
                    teamInDb.ProviderTeamId = team.id;

                    _rugbyTeamRepository.Update(teamInDb);
                }
            }

            await _rugbyTeamRepository.SaveAsync();
        }

        private async Task PersistVenuesInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;


            var venuesAlreadyInDb = await _rugbyVenueRepository.AllAsync();

            foreach (var venue in entitiesResponse.Entities.venues)
            {
                // Lookup in db
                var venueInDb = venuesAlreadyInDb.Where(v => v.ProviderVenueId == venue.id).FirstOrDefault();

                if (venueInDb == null)
                {
                    var newVenue = new RugbyVenue()
                    {
                        ProviderVenueId = venue.id,
                        Name = venue.name,
                        DataProvider = DataProvider.StatsProzone
                    };
                    _rugbyVenueRepository.Add(newVenue);
                }
                else
                {
                    venueInDb.Name = venue.name;
                    venueInDb.ProviderVenueId = venue.id;

                    _rugbyVenueRepository.Update(venueInDb);
                }
            }

            await _rugbyVenueRepository.SaveAsync();
        }

        private async Task PersistTournamentSeasonsInRepository(CancellationToken cancellationToken)
        {
            var activeTournaments = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.IsEnabled);
            foreach (var tournament in activeTournaments)
            {
                await IngestSeason(cancellationToken, tournament, DateTime.Now.Year);
                await IngestSeason(cancellationToken, tournament, DateTime.Now.Year + 1);
            }
        }

        private async Task IngestSeason(CancellationToken cancellationToken, RugbyTournament tournament, int year)
        {
            var season = _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, year);

            var providerTournamentId = season.RugbySeasons.competitionId;

            if (season.RugbySeasons.season.Count == 0)
                return;

            var providerSeasonId = season.RugbySeasons.season.First().id;

            var isSeasonCurrentlyActive = season.RugbySeasons.season.First().currentSeason;

            var seasonsInDb = _rugbySeasonRepository.All().ToList();

            var seasonEntry =
                    seasonsInDb
                    .Where(s => s.RugbyTournament.ProviderTournamentId == providerTournamentId && s.ProviderSeasonId == providerSeasonId)
                    .FirstOrDefault();

            var tournamentInDb = _rugbyTournamentRepository.Where(t => t.ProviderTournamentId == providerTournamentId).FirstOrDefault();

            var newEntry = new RugbySeason()
            {
                ProviderSeasonId = providerSeasonId,
                RugbyTournament = tournamentInDb,
                IsCurrent = isSeasonCurrentlyActive,
                Name = season.RugbySeasons.season.First().name,
                DataProvider = DataProvider.StatsProzone
            };

            // Not in repo?
            if (seasonEntry == null)
            {
                _rugbySeasonRepository.Add(newEntry);
                seasonsInDb.Add(seasonEntry);
            }
            else
            {
                seasonEntry.StartDateTime = newEntry.StartDateTime;
                seasonEntry.Name = newEntry.Name;
                seasonEntry.IsCurrent = newEntry.IsCurrent;

                _rugbySeasonRepository.Update(seasonEntry);
            }

            await _rugbySeasonRepository.SaveAsync();
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
                var activeSeasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var fixtures =
                    _statsProzoneIngestService.IngestFixturesForTournament(
                        tournament, activeSeasonId, cancellationToken);

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

            var laddersAlreadyInDb = (await _rugbyFlatLogsRepository.AllAsync());

            if (flatLogsResponse.RugbyFlatLogs.ladderposition == null)
                return;

            foreach (var position in flatLogsResponse.RugbyFlatLogs.ladderposition)
            {
                var ladderEntryInDb =
                    laddersAlreadyInDb.Where(
                        l => l.RugbyTournament.ProviderTournamentId == tournamentId &&
                             l.RugbySeason.ProviderSeasonId == seasonId &&
                             l.RoundNumber == roundNumber).FirstOrDefault();

                var tournament = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.ProviderTournamentId == tournamentId).FirstOrDefault();
                var season = (await _rugbySeasonRepository.AllAsync()).Where(s => s.RugbyTournament.Id == tournament.Id && s.ProviderSeasonId == seasonId).FirstOrDefault();
                var team = (await _rugbyTeamRepository.AllAsync()).Where(t => t.ProviderTeamId == position.teamId).FirstOrDefault();

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
                    _rugbyFlatLogsRepository.Add(newLadderEntry);
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

                    _rugbyFlatLogsRepository.Update(ladderEntryInDb);
                }
            }

            await _rugbyFlatLogsRepository.SaveAsync();
        }

        private async Task PersistFixturesData(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, fixtures);
            await PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(fixtures);

            _mongoDbRepository.Save(fixtures);
        }

        private async Task PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(RugbyFixturesResponse fixtures)
        {
            var fixturesAlreadyInDb = (await _rugbyFixturesRepository.AllAsync());

            foreach (var roundFixtures in fixtures.Fixtures.roundFixtures)
            {
                foreach (var fixture in roundFixtures.gameFixtures)
                {
                    var fixtureInDb = fixturesAlreadyInDb.Where(f => f.ProviderFixtureId == fixture.gameId).FirstOrDefault();
                    var fixtureGuid = fixtureInDb.Id;
                    var tournamentGuid = fixtureInDb.RugbyTournament.Id;

                    var fixtureSchedule =
                            (await _schedulerTrackingRugbyFixtureRepository.AllAsync())
                                .Where(
                                    f => f.FixtureId == fixtureGuid && f.TournamentId == tournamentGuid)
                                .FirstOrDefault();

                    if (fixtureSchedule == null)
                    {
                        var newFixtureSchedule = new SchedulerTrackingRugbyFixture()
                        {
                            FixtureId = fixtureGuid,
                            TournamentId = tournamentGuid,
                            StartDateTime = fixtureInDb.StartDateTime,
                            EndedDateTime = DateTimeOffset.MinValue,
                            RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixture.gameStateName),
                            SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted
                        };

                        _schedulerTrackingRugbyFixtureRepository.Add(newFixtureSchedule);
                    }
                    else
                    {
                        // If the schedule already is in the system repo
                        // we need to update the status of the game.
                        var gameState = GetFixtureStatusFromProviderFixtureState(fixture.gameStateName);
                        fixtureSchedule.RugbyFixtureStatus = gameState;
                        fixtureSchedule.StartDateTime = fixtureInDb.StartDateTime;
                        fixtureSchedule.SchedulerStateFixtures = 
                            FixturesStateHelper.GetSchedulerStateForFixture(DateTime.UtcNow, gameState, fixtureInDb.StartDateTime.DateTime);

                        if (HasFixtureEnded(fixture.gameStateName) &&
                           fixtureSchedule.EndedDateTime == DateTimeOffset.MinValue)
                        {
                            fixtureSchedule.EndedDateTime =
                                fixtureSchedule.StartDateTime
                                    .AddSeconds(
                                        fixture.gameSeconds)
                                    .AddMinutes(30); // This is for half-time break.
                        }

                        _schedulerTrackingRugbyFixtureRepository.Update(fixtureSchedule);
                    }
                }
            }

            await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private bool HasFixtureEnded(string gameStateName)
        {
            var state = GetFixtureStatusFromProviderFixtureState(gameStateName);
            return state == RugbyFixtureStatus.Result;
        }

        private async Task PersistRugbyFixturesToPublicSportsRepository(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var tournament = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.ProviderTournamentId == fixtures.Fixtures.competitionId).FirstOrDefault();

            var allFixtures = (await _rugbyFixturesRepository.AllAsync());
            var allTeams = (await _rugbyTeamRepository.AllAsync());
            var allVenues = (await _rugbyVenueRepository.AllAsync());

            foreach (var roundFixture in fixtures.Fixtures.roundFixtures)
            {
                foreach (var fixture in roundFixture.gameFixtures)
                {
                    var fixtureId = fixture.gameId;

                    // Lookup in Db
                    var fixtureInDb = allFixtures.Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();
                    DateTimeOffset.TryParse(fixture.startTimeUTC, out DateTimeOffset startTime);
                    var teams = fixture.teams.ToArray();
                    // We need temporary variables here.
                    // Cannot use indexing in Linq Where clause.
                    var team0 = teams[0];
                    var team1 = teams[1];

                    var teamA = allTeams.Where(t => t.ProviderTeamId == team0.teamId).FirstOrDefault();
                    var teamB = allTeams.Where(t => t.ProviderTeamId == team1.teamId).FirstOrDefault();

                    var newFixture = new RugbyFixture()
                    {
                        ProviderFixtureId = fixtureId,
                        StartDateTime = startTime,
                        RugbyVenue = allVenues.Where(v => v.ProviderVenueId == fixture.venueId).FirstOrDefault(),
                        RugbyTournament = tournament,
                        TeamA = teamA,
                        TeamB = teamB,
                        TeamAIsHomeTeam = team0.isHomeTeam,
                        TeamBIsHomeTeam = team1.isHomeTeam,
                        RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixture.gameStateName),
                        DataProvider = DataProvider.StatsProzone
                    };

                    if (fixtureInDb == null)
                    {
                        _rugbyFixturesRepository.Add(newFixture);
                    }
                    else
                    {
                        // Only update the scores for games that are completed.
                        // Real-time scores will be updated separately 
                        // in a method that runs more frequently.
                        if (fixtureInDb.RugbyFixtureStatus == RugbyFixtureStatus.Result)
                        {
                            newFixture.TeamAScore = team0.teamFinalScore;
                            newFixture.TeamBScore = team1.teamFinalScore;

                            fixtureInDb.TeamAScore = newFixture.TeamAScore;
                            fixtureInDb.TeamBScore = newFixture.TeamBScore;
                        }

                        fixtureInDb.StartDateTime = startTime;
                        fixtureInDb.RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixture.gameStateName);
                        fixtureInDb.RugbyVenue = newFixture.RugbyVenue;
                        fixtureInDb.TeamA = newFixture.TeamA;
                        fixtureInDb.TeamB = newFixture.TeamB;
                        fixtureInDb.TeamAIsHomeTeam = newFixture.TeamAIsHomeTeam;
                        fixtureInDb.TeamBIsHomeTeam = newFixture.TeamBIsHomeTeam;
                        fixtureInDb.RugbyTournament = newFixture.RugbyTournament;

                        _rugbyFixturesRepository.Update(fixtureInDb);
                    }
                }
            }

            await _rugbyFixturesRepository.SaveAsync();
        }

        private RugbyFixtureStatus GetFixtureStatusFromProviderFixtureState(string gameStateName)
        {
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
                (await _rugbySeasonRepository.AllAsync())
                .Where(
                    s => s.ProviderSeasonId == fixtures.Fixtures.seasonId && s.RugbyTournament.ProviderTournamentId == fixtures.Fixtures.competitionId)
                    .FirstOrDefault();

            if (season == null)
                return;

            var seasonId = season.Id;
            var tournamentId = season.RugbyTournament.Id;
            DateTimeOffset.TryParse(fixtures.Fixtures.seasonStartDate, out DateTimeOffset seasonStartDate);
            DateTimeOffset.TryParse(fixtures.Fixtures.seasonFinishDate, out DateTimeOffset seasonEndDate);

            if (season != null)
            {
                season.StartDateTime = seasonStartDate;
                season.EndDateTime = seasonEndDate;
                _rugbySeasonRepository.Update(season);
            }

            var dateOffsetNow = DateTimeOffset.Now;

            var seasonStatus = GetRugbySeasonStatus(seasonStartDate, dateOffsetNow, seasonEndDate);

            var seasonInDb = (await _schedulerTrackingRugbySeasonRepository.AllAsync()).Where(s => s.SeasonId == seasonId && s.TournamentId == tournamentId).FirstOrDefault();

            if (seasonInDb == null)
            {
                _schedulerTrackingRugbySeasonRepository.Add(
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
                _schedulerTrackingRugbySeasonRepository.Update(seasonInDb);
            }

            await _schedulerTrackingRugbySeasonRepository.SaveAsync();
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
                await _rugbyService.GetActiveTournaments();

            await IngestLogsHelper(activeTournaments, cancellationToken);
        }

        public async Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var currentTournaments =
                await _rugbyService.GetCurrentTournaments();

            await IngestLogsHelper(currentTournaments, cancellationToken);
        }

        public async Task IngestLogsHelper(IEnumerable<RugbyTournament> tournaments, CancellationToken cancellationToken)
        {
            var seasons = (await _rugbySeasonRepository.AllAsync());

            foreach (var tournament in tournaments)
            {
                int activeSeasonIdForTournament =
                        await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var season = seasons.Where(s => s.ProviderSeasonId == activeSeasonIdForTournament &&
                                                 s.RugbyTournament.ProviderTournamentId == tournament.ProviderTournamentId);

                if (!season.Any())
                    return;

                var logType = season.First().RugbyLogType;

                if (logType == RugbyLogType.FlatLogs)
                {
                    RugbyFlatLogsResponse logs =
                    _statsProzoneIngestService.IngestFlatLogsForTournament(
                        tournament.ProviderTournamentId, activeSeasonIdForTournament);

                    await PersistFlatLogs(cancellationToken, logs);

                    _mongoDbRepository.Save(logs);
                }
                else
                {
                    RugbyGroupedLogsResponse logs =
                    _statsProzoneIngestService.IngestGroupedLogsForTournament(
                        tournament.ProviderTournamentId, activeSeasonIdForTournament);

                    await PersistGroupedLogs(cancellationToken, logs);

                    _mongoDbRepository.Save(logs);
                }
            }
        }

        private async Task IngestStandings(CancellationToken cancellationToken, RugbyGroupedLogsResponse logs, List<Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.Ladderposition> ladderPositions)
        {
            var tournamentId = logs.RugbyGroupedLogs.competitionId;
            var seasonId = logs.RugbyGroupedLogs.seasonId;
            var roundNumber = logs.RugbyGroupedLogs.roundNumber;

            var rugbyTournament = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.ProviderTournamentId == tournamentId).FirstOrDefault();
            var rugbySeason = (await _rugbySeasonRepository.AllAsync()).Where(s => s.ProviderSeasonId == seasonId).FirstOrDefault();

            foreach (var ladder in ladderPositions)
            {
                var teamId = ladder.teamId;

                var rugbyTeam = (await _rugbyTeamRepository.AllAsync()).Where(t => t.ProviderTeamId == teamId).FirstOrDefault();
                var rugbyLogGroup = (await _rugbyLogGroupRepository.AllAsync()).Where(g => g.ProviderLogGroupId == ladder.group && g.GroupName == ladder.groupName).FirstOrDefault();

                // This means the RugbyLogGroup is not in the db.
                // Should be added to the db via CMS or manually.
                if (rugbyLogGroup == null)
                    continue;

                // Does an entry in the db exist for this tournament-season-team?
                var entryInDb = (await _rugbyGroupedLogsRepository.AllAsync())
                                    .Where(g => g.RugbyLogGroup.ProviderLogGroupId == ladder.group &&
                                                g.RugbyLogGroup.GroupName == ladder.groupName &&
                                                g.RugbyTournament.ProviderTournamentId == tournamentId &&
                                                g.RugbySeason.ProviderSeasonId == seasonId &&
                                                g.RugbyTeam.ProviderTeamId == teamId).FirstOrDefault();

                var newLogEntry = new RugbyGroupedLog()
                {
                    LogPosition = ladder.position,
                    GamesPlayed = ladder.gamesPlayed,
                    GamesWon = ladder.wins,
                    GamesLost = ladder.losses,
                    GamesDrawn = ladder.draws == null ? 0 : (int)ladder.draws,
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
                    _rugbyGroupedLogsRepository.Add(newLogEntry);
                }
                else
                {
                    entryInDb.LogPosition = ladder.position;
                    entryInDb.GamesPlayed = ladder.gamesPlayed;
                    entryInDb.GamesWon = ladder.wins;
                    entryInDb.GamesLost = ladder.losses;
                    entryInDb.GamesDrawn = ladder.draws == null ? 0 : (int)ladder.draws;
                    entryInDb.PointsFor = ladder.pointsFor;
                    entryInDb.PointsAgainst = ladder.pointsAgainst;
                    entryInDb.PointsDifference = ladder.pointsDifference;
                    entryInDb.TournamentPoints = ladder.competitionPoints;
                    entryInDb.BonusPoints = ladder.bonusPoints;
                    entryInDb.TriesFor = ladder.triesFor;
                    entryInDb.TriesAgainst = ladder.triesAgainst;
                    entryInDb.RoundNumber = logs.RugbyGroupedLogs.roundNumber;

                    _rugbyGroupedLogsRepository.Update(entryInDb);
                }
            }
        }

        private async Task PersistGroupedLogs(CancellationToken cancellationToken, RugbyGroupedLogsResponse logs)
        {
            await IngestStandings(cancellationToken, logs, logs.RugbyGroupedLogs.overallStandings.ladderposition);
            await IngestStandings(cancellationToken, logs, logs.RugbyGroupedLogs.groupStandings.ladderposition);
            await IngestStandings(cancellationToken, logs, logs.RugbyGroupedLogs.secondaryGroupStandings.ladderposition);

            await _rugbyGroupedLogsRepository.SaveAsync();
        }

        private async Task PersistTournamentsInRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            foreach (var competition in entitiesResponse.Entities.competitions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var entry = (await _rugbyTournamentRepository.AllAsync())
                    .Where(c => c.ProviderTournamentId == competition.id)
                    .FirstOrDefault();

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
                    LegacyTournamentId = competition.id,
                    DataProvider = DataProvider.StatsProzone
                };

                if (entry == null)
                {
                    _rugbyTournamentRepository.Add(newEntry);
                }
                else
                {
                    entry.ProviderTournamentId = newEntry.ProviderTournamentId;
                    entry.Name = newEntry.Name;
                    entry.LogoUrl = newEntry.LogoUrl;
                    entry.Abbreviation = newEntry.Abbreviation;

                    _rugbyTournamentRepository.Update(entry);
                }
            }

            await _rugbyTournamentRepository.SaveAsync();
        }

        private string GetSlug(string name)
        {
            var lower_case = name.ToLower();
            var lower_case_with_special_characters_removed =
                    RemoveSpecialCharacters(lower_case);

            var slug = lower_case_with_special_characters_removed.Replace(' ', '-');
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
                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);
                var results = _statsProzoneIngestService.IngestFixturesForTournament(tournament, seasonId, cancellationToken);
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
                _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                    tournamentId, seasonId, cancellationToken);

            await PersistFixturesData(cancellationToken, fixtures);
        }

        public async Task IngestResultsForCurrentDayFixtures(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0)
                return;

            var currentDayFixtures = await _rugbyService.GetCurrentDayFixturesForActiveTournaments(); 

            foreach (var fixture in currentDayFixtures)
            {
                var tournamentId = fixture.RugbyTournament.ProviderTournamentId;

                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, fixture.RugbyTournament.Id);

                var results = _statsProzoneIngestService.IngestFixturesForTournamentSeason(tournamentId, seasonId, cancellationToken);

                 await PersistRugbyFixturesToPublicSportsRepository(cancellationToken,results);
            }
        }

        public async Task IngestResultsForFixturesInResultsState(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixtures = await _rugbyService.GetCurrentDayFixturesForActiveTournaments();

            foreach (var fixture in fixtures)
            {
                var tournamentId = fixture.RugbyTournament.ProviderTournamentId;

                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, fixture.RugbyTournament.Id);

                var results = _statsProzoneIngestService.IngestFixturesForTournamentSeason(tournamentId, seasonId, cancellationToken);

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
                        _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                            tournament.ProviderTournamentId,
                            await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id),
                            cancellationToken);

                var upcomingFixtures = RemoveFixturesThatHaveBeenCompleted(fixtures);
                RugbyFixturesResponse oneMonthsfixtures = RemoveFixturesMoreThanAMonthFromNow(upcomingFixtures);

                await PersistFixturesData(cancellationToken, oneMonthsfixtures);

                _mongoDbRepository.Save(fixtures);
            }
        }

        private async Task<RugbyTournament> GetEnabledTournamentForId(int id)
        {
            var tournament = (await _rugbyService.GetCurrentTournaments()).Where(t => t.ProviderTournamentId == id).FirstOrDefault();

            return tournament;
        }

        private RugbyFixturesResponse RemoveFixturesThatHaveBeenCompleted(RugbyFixturesResponse fixtures)
        {
            foreach (var round in fixtures.Fixtures.roundFixtures)
            {
                round.gameFixtures
                    .RemoveAll(
                        f => GetFixtureStatusFromProviderFixtureState(f.gameStateName) == RugbyFixtureStatus.Result);
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
            var fixtureInDb = (await _rugbyFixturesRepository.AllAsync()).Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var matchStatsResponse =
                    await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, providerFixtureId);

                var eventsFlowResponse =
                    await _statsProzoneIngestService.IngestEventsFlow(cancellationToken, providerFixtureId);

                await IngestCommentary(cancellationToken, eventsFlowResponse.RugbyEventsFlow.commentaryFlow, providerFixtureId);
                await IngestMatchStatisticsData(cancellationToken, matchStatsResponse, providerFixtureId);
                await IngestScoreData(cancellationToken, matchStatsResponse);
                await IngestFixtureStatusData(cancellationToken, matchStatsResponse);
                await UpdateSchedulerTrackingFixturesTable(fixtureInDb.Id, matchStatsResponse.RugbyMatchStats.gameState);

                await IngestLineUpsForFixtures(cancellationToken, new List<RugbyFixture>(){ fixtureInDb });

                await IngestEvents(cancellationToken, eventsFlowResponse);

                _mongoDbRepository.Save(matchStatsResponse);
                _mongoDbRepository.Save(eventsFlowResponse);

                //// Check if should stop looping?
                var matchState = GetFixtureStatusFromProviderFixtureState(matchStatsResponse.RugbyMatchStats.gameState);
                var schedulerState = FixturesStateHelper.GetSchedulerStateForFixture(DateTime.UtcNow, matchState, fixtureInDb.StartDateTime.DateTime);

                if (schedulerState == SchedulerStateForRugbyFixturePolling.SchedulingCompleted ||
                    schedulerState == SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted ||
                    schedulerState == SchedulerStateForRugbyFixturePolling.ResultOnlyPolling)
                    break;

                Thread.Sleep(5_000);
            }
        }

        private async Task UpdateSchedulerTrackingFixturesTable(Guid FixtureId, string fixtureGameState)
        {
            var schedule = (await _schedulerTrackingRugbyFixtureRepository.AllAsync())
                                .Where(s => s.FixtureId == FixtureId).FirstOrDefault();

            if(schedule == null)
            {
                return;
            }
            else
            {
                var fixtureState = GetFixtureStatusFromProviderFixtureState(fixtureGameState);
                schedule.RugbyFixtureStatus = fixtureState;
                schedule.SchedulerStateFixtures = 
                    FixturesStateHelper.GetSchedulerStateForFixture(DateTime.UtcNow, fixtureState, schedule.StartDateTime.DateTime);
                _schedulerTrackingRugbyFixtureRepository.Update(schedule);
            }

            await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private async Task IngestEvents(CancellationToken cancellationToken, RugbyEventsFlowResponse eventsFlowResponse)
        {
            await IngestScoreEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.scoreFlow, eventsFlowResponse.RugbyEventsFlow.gameId);
            await IngestPenaltyEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.penaltyFlow, eventsFlowResponse.RugbyEventsFlow.gameId);
            await IngestErrorEvents(cancellationToken, eventsFlowResponse.RugbyEventsFlow.errorFlow, eventsFlowResponse.RugbyEventsFlow.gameId);

            await _rugbyMatchEventsRepository.SaveAsync();
        }

        private async Task IngestErrorEvents(CancellationToken cancellationToken, ErrorFlow errorFlow, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (errorFlow == null)
                return;

            if (errorFlow.errorEvent == null)
                return;

            if (errorFlow.errorEvent.statErrorEvent == null)
                return;

            var fixtures = (await _rugbyFixturesRepository.AllAsync()).ToList();
            var players = (await _rugbyPlayerRepository.AllAsync()).ToList();
            var teamsInDb = (await _rugbyTeamRepository.AllAsync()).ToList();
            var events = (await _rugbyMatchEventsRepository.AllAsync()).ToList();
            var eventTypeProviderMappings = (await _rugbyEventTypeMappingRepository.AllAsync());
            var fixture = fixtures.Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();

            foreach (var error in errorFlow.errorEvent.statErrorEvent)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var eventTypeMapping = eventTypeProviderMappings.Where(m => m.ProviderEventTypeId == error.statId).FirstOrDefault();

                if (eventTypeMapping == null)
                    return;

                if (eventTypeMapping.RugbyEventType == null)
                    return;

                var teamInDb = teamsInDb.Where(t => t.ProviderTeamId == error.teamId).FirstOrDefault();

                var newEvent = new RugbyMatchEvent()
                {
                    EventValue = (float)error.statValue,
                    GameTimeInSeconds = error.gameSeconds,
                    GameTimeInMinutes = error.gameSeconds / 60,
                    RugbyFixture = fixture,
                    RugbyFixtureId = fixture.Id,
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
                var eventInDb = events.Where(e =>
                                    e.RugbyFixtureId == newEvent.RugbyFixtureId &&
                                    e.RugbyTeamId == newEvent.RugbyTeamId &&
                                    e.RugbyEventTypeId == newEvent.RugbyEventTypeId &&
                                    e.GameTimeInSeconds == newEvent.GameTimeInSeconds).FirstOrDefault();

                if (eventInDb == null)
                {
                    _rugbyMatchEventsRepository.Add(newEvent);
                }
                else
                {
                    // TODO: We need to update an existing record here.
                    _rugbyMatchEventsRepository.Update(eventInDb);
                }
            }
        }

        private async Task IngestPenaltyEvents(CancellationToken cancellationToken, PenaltyFlow penaltyFlow, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (penaltyFlow == null)
                return;

            if (penaltyFlow.penaltyEvent == null)
                return;

            var penalties = penaltyFlow.penaltyEvent.statPenaltyEvent;
            if (penalties == null)
                return;

            var fixtures = (await _rugbyFixturesRepository.AllAsync()).ToList();
            var players = (await _rugbyPlayerRepository.AllAsync()).ToList();
            var teamsInDb = (await _rugbyTeamRepository.AllAsync()).ToList();
            var events = (await _rugbyMatchEventsRepository.AllAsync()).ToList();
            var eventTypeProviderMappings = (await _rugbyEventTypeMappingRepository.AllAsync());

            var fixture = fixtures.Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();

            foreach(var penaltyEvent in penalties)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                // Can we find a mapping for this event in the db?
                var eventTypeMapping = eventTypeProviderMappings.Where(m => m.ProviderEventTypeId == penaltyEvent.statId).FirstOrDefault();
                if (eventTypeMapping == null)
                    return;

                if (eventTypeMapping.RugbyEventType == null)
                    return;

                var teamInDb = teamsInDb.Where(t => t.ProviderTeamId == penaltyEvent.teamId).FirstOrDefault();

                var newEvent = new RugbyMatchEvent()
                {
                    EventValue = (float)penaltyEvent.statValue,
                    GameTimeInSeconds = penaltyEvent.gameSeconds,
                    GameTimeInMinutes = penaltyEvent.gameSeconds / 60,
                    RugbyFixture = fixture,
                    RugbyFixtureId = fixture.Id,
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
                var eventInDb = events.Where(e =>
                                    e.RugbyFixtureId == newEvent.RugbyFixtureId &&
                                    e.RugbyTeamId == newEvent.RugbyTeamId &&
                                    e.RugbyEventTypeId == newEvent.RugbyEventTypeId &&
                                    e.GameTimeInSeconds == newEvent.GameTimeInSeconds).FirstOrDefault();

                if (eventInDb == null)
                {
                    _rugbyMatchEventsRepository.Add(newEvent);
                }
                else
                {
                    // TODO: We need to update an existing record here.
                    _rugbyMatchEventsRepository.Update(eventInDb);
                }
            }
        }

        private async Task IngestScoreEvents(CancellationToken cancellationToken, ScoreFlow scoreFlow, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (scoreFlow == null)
                return;

            if (scoreFlow.scoreEvent == null)
                return;

            var teams = scoreFlow.scoreEvent.teams[0];
            if (teams == null)
                return;

            var fixtures = (await _rugbyFixturesRepository.AllAsync()).ToList();
            var players = (await _rugbyPlayerRepository.AllAsync()).ToList();
            var teamsInDb = (await _rugbyTeamRepository.AllAsync()).ToList();
            var events = (await _rugbyMatchEventsRepository.AllAsync()).ToList();
            var eventTypeProviderMappings = (await _rugbyEventTypeMappingRepository.AllAsync());

            var fixture = fixtures.Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();

            foreach (var team in teams.team)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var teamInDb = teamsInDb.Where(t => t.ProviderTeamId == team.teamId).FirstOrDefault();

                var scoreEvents = team.statScoringEvent;
                if (scoreEvents == null)
                    continue;

                foreach(var scoreEvent in scoreEvents)
                {
                    // Can we find a mapping for this event in the db?
                    var eventTypeMapping = eventTypeProviderMappings.Where(m => m.ProviderEventTypeId == scoreEvent.statId).FirstOrDefault();
                    if (eventTypeMapping == null)
                        return;

                    if (eventTypeMapping.RugbyEventType == null)
                        return;

                    var player = players.Where(p => p.ProviderPlayerId == scoreEvent.playerId).FirstOrDefault();

                    var newEvent = new RugbyMatchEvent()
                    {
                        EventValue = (float)scoreEvent.statValue,
                        GameTimeInSeconds = scoreEvent.gameSeconds,
                        GameTimeInMinutes = scoreEvent.gameSeconds / 60,
                        RugbyFixture = fixture,
                        RugbyFixtureId = fixture.Id,
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
                    var eventInDb = events.Where(e =>
                                        e.RugbyFixtureId == newEvent.RugbyFixtureId &&
                                        e.RugbyTeamId == newEvent.RugbyTeamId &&
                                        e.RugbyEventTypeId == newEvent.RugbyEventTypeId &&
                                        e.GameTimeInSeconds == newEvent.GameTimeInSeconds).FirstOrDefault();

                    if (eventInDb == null)
                    {
                        _rugbyMatchEventsRepository.Add(newEvent);
                    }
                    else
                    {
                        // TODO: We need to update an existing record here.
                        _rugbyMatchEventsRepository.Update(eventInDb);
                    }
                }
            }
        }

        private async Task IngestFixtureStatusData(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixtureId = matchStatsResponse.RugbyMatchStats.gameId;
            var fixtureInDb = (await _rugbyFixturesRepository.AllAsync()).Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();
            var fixtureState = GetFixtureStatusFromProviderFixtureState(matchStatsResponse.RugbyMatchStats.gameState);

            if (fixtureInDb != null)
            {
                fixtureInDb.RugbyFixtureStatus = fixtureState;
                _rugbyFixturesRepository.Update(fixtureInDb);
            }

            await _rugbyFixturesRepository.SaveAsync();
        }

        private async Task IngestScoreData(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var allFixtures = (await _rugbyFixturesRepository.AllAsync());

            var scores = GetScoresForFixture(cancellationToken, matchStatsResponse);
            var fixtureId = matchStatsResponse.RugbyMatchStats.gameId;

            var fixtureInDb = allFixtures.Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();

            if (fixtureInDb == null)
            {
                // Is this even possible?
                // To be ingesting scores for a fixture that doesnt
                // exist in the DB.
            }
            else
            {
                fixtureInDb.TeamAScore = scores.teamAScore;
                fixtureInDb.TeamBScore = scores.teamBScore;
                _rugbyFixturesRepository.Update(fixtureInDb);
            }

            await _rugbyFixturesRepository.SaveAsync();
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

        public async Task IngestMatchStatisticsData(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixture = (await _rugbyFixturesRepository.AllAsync()).Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();
            var teamsInRepo = (await _rugbyTeamRepository.AllAsync()).ToList();
            var matchStatistics = (await _rugbyMatchStatisticsRepository.AllAsync()).ToList();

            var teamsFromProvider = matchStatsResponse.RugbyMatchStats.teams;

            foreach(var teamMatch in teamsFromProvider.teamsMatch)
            {
                var team = teamsInRepo.Where(t => t.ProviderTeamId == teamMatch.teamId).FirstOrDefault();

                var stats = teamMatch.teamStats.matchStats.matchStat;
                var statsInDb = matchStatistics.Where(s => s.RugbyFixtureId == fixture.Id && s.RugbyTeamId == team.Id).FirstOrDefault();

                var statsMap = MakeStatisticsMap(stats);
                var newStats = new RugbyMatchStatistics()
                {
                    RugbyFixture = fixture,
                    RugbyFixtureId = fixture.Id,
                    RugbyTeam = team,
                    RugbyTeamId = team.Id,
                    YellowCards = (int)statsMap.GetValueOrDefault(2),
                    //CarriesCrossedGainLine = statsMap.GetValueOrDefault("(Time)Territory"),
                    CleanBreaks = (int)statsMap.GetValueOrDefault(7),
                    ConversionAttempts = (int)statsMap.GetValueOrDefault(2047),
                    Conversions = (int)statsMap.GetValueOrDefault(2046),
                    ConversionsMissed = (int)statsMap.GetValueOrDefault(2048),
                    DefendersBeaten = (int)statsMap.GetValueOrDefault(8),
                    DropGoalAttempts = (int)statsMap.GetValueOrDefault(2049),
                    DropGoals = (int)statsMap.GetValueOrDefault(2050),
                    DropGoalsMissed = (int)(statsMap.GetValueOrDefault(2049) - statsMap.GetValueOrDefault(2050)),
                    LineOutsLost = (int)statsMap.GetValueOrDefault(20),
                    LineOutsWon = (int)statsMap.GetValueOrDefault(19),
                    Offloads = (int)statsMap.GetValueOrDefault(46),
                    Passes = (int)statsMap.GetValueOrDefault(2012),
                    Penalties = (int)(statsMap.GetValueOrDefault(2038) - statsMap.GetValueOrDefault(2039)),
                    PenaltiesConceded = (int)statsMap.GetValueOrDefault(2079),
                    PenaltiesMissed = (int)statsMap.GetValueOrDefault(2039),
                    PenaltyAttempts = (int)statsMap.GetValueOrDefault(2038),
                    PenaltyTries = (int)statsMap.GetValueOrDefault(10530),
                    Possession = (int)statsMap.GetValueOrDefault(42),
                    RedCards = (int)statsMap.GetValueOrDefault(3),
                    ScrumsLost = (int)statsMap.GetValueOrDefault(55),
                    ScrumsWon = (int)statsMap.GetValueOrDefault(53),
                    Tackles = (int)statsMap.GetValueOrDefault(72),
                    TacklesMissed = (int)statsMap.GetValueOrDefault(71),
                    Territory = (int)statsMap.GetValueOrDefault(10000),
                    Tries = (int)statsMap.GetValueOrDefault(5)
                };

                if (statsInDb == null)
                {
                    _rugbyMatchStatisticsRepository.Add(newStats);
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

                    _rugbyMatchStatisticsRepository.Update(statsInDb);
                }
            }

            await _rugbyMatchStatisticsRepository.SaveAsync();
        }

        private IDictionary<int, double> MakeStatisticsMap(IList<MatchStat> matchStats)
        {
            var dictionary = new Dictionary<int, double>();
            foreach (var stat in matchStats)
            {
                dictionary.Add(stat.StatTypeID, stat.StatValue);
            }

            return dictionary;
        }

        public async Task IngestCommentary(CancellationToken cancellationToken, CommentaryFlow commentary, long providerFixtureId)
        {
            if (commentary == null)
                return;

            if (commentary.commentaryEvent == null)
                return;

            var fixtures = (await _rugbyFixturesRepository.AllAsync()).ToList();
            var teams = (await _rugbyTeamRepository.AllAsync()).ToList();
            var players = (await _rugbyPlayerRepository.AllAsync()).ToList();
            var commentaries = (await _rugbyCommentaryRepository.AllAsync()).ToList();

            foreach(var comment in commentary.commentaryEvent)
            {
                var commentText = comment.commentary;
                var commentTimeInSeconds = comment.gameSeconds;
                var commentaryTimeInMinutes = commentTimeInSeconds / 60;
                var gameTimeDisplayHoursMinutesSeconds = comment.gameTime;
                var gameTimeDisplayMinutesSeconds = comment.GameMinutes;

                var fixture = fixtures.Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();
                var team = teams.Where(t => t.ProviderTeamId == comment.teamId).FirstOrDefault();
                var player = players.Where(p => p.ProviderPlayerId == comment.playerId).FirstOrDefault();

                var dbCommentary = commentaries.Where(c =>
                                                c.GameTimeRawSeconds == commentTimeInSeconds &&
                                                c.CommentaryText == commentText &&
                                                c.RugbyFixture.Id == fixture.Id &&
                                                c.RugbyPlayer == player &&
                                                c.RugbyTeam == team).FirstOrDefault();

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
                    _rugbyCommentaryRepository.Add(newCommentary);
                    // Add the commentary to the local list.
                    commentaries.Add(newCommentary);
                }
                else
                {
                    // There isnt something which uniquely identifies 
                }
            }

            await _rugbyCommentaryRepository.SaveAsync();
        }

        public async Task IngestLineupsForUpcomingGames(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var now = DateTime.UtcNow;
            var NowPlusTwoDays = DateTime.UtcNow + TimeSpan.FromDays(2);

            var gamesInTheNext2Days =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(
                            fixture => fixture.RugbyTournament != null &&
                                       fixture.RugbyTournament.IsEnabled &&
                                       fixture.StartDateTime >= now &&
                                       fixture.StartDateTime <= NowPlusTwoDays);

            await IngestLineUpsForFixtures(cancellationToken, gamesInTheNext2Days);
        }

        private async Task IngestLineUpsForFixtures(CancellationToken cancellationToken, IEnumerable<RugbyFixture> rugbyFixtures)
        {
            foreach (var fixture in rugbyFixtures)
            {
                var fixtureId = fixture.ProviderFixtureId;
                var matchStatsResponse =
                    await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, fixtureId);

                await IngestPlayerLineups(cancellationToken, matchStatsResponse);
            }
        }

        private async Task IngestPlayerLineups(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
        {
            // Do we have provider info?
            if (matchStatsResponse.RugbyMatchStats.teams.teamsMatch.Count == 0)
                return;

            foreach (var squad in matchStatsResponse.RugbyMatchStats.teams.teamsMatch)
            {
                var lineup = squad.teamLineup;

                if (lineup == null)
                    continue;

                var players = lineup.teamPlayer;

                if (players == null)
                    continue;

                foreach (var player in players)
                {
                    var playerId = player.playerId;
                    var dbPlayer = (await _rugbyPlayerRepository.AllAsync()).Where(p => p.ProviderPlayerId == playerId).FirstOrDefault();

                    if (dbPlayer == null)
                        continue;

                    if (dbPlayer.FirstName == null && dbPlayer.LastName == null)
                    {
                        dbPlayer.FirstName = player.playerFirstName;
                        dbPlayer.LastName = player.playerLastName;
                        _rugbyPlayerRepository.Update(dbPlayer);
                    }

                    var fixtureId = matchStatsResponse.RugbyMatchStats.gameId;
                    var dbFixture = (await _rugbyFixturesRepository.AllAsync()).Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();

                    var teamId = squad.teamId;
                    var dbTeam = (await _rugbyTeamRepository.AllAsync()).Where(t => t.ProviderTeamId == teamId).FirstOrDefault();

                    var shirtNumber = player.shirtNum;
                    var positionName = player.playerPosition;

                    var isCaptain = player.isCaptain == null ? false : (bool)player.isCaptain;
                    var isSubstitute = player.shirtNum >= 16;

                    var dbEntry =
                            (await _rugbyPlayerLineupsRepository.AllAsync())
                                .Where(l =>
                                    l.RugbyPlayerId == dbPlayer.Id &&
                                    l.RugbyFixtureId == dbFixture.Id &&
                                    l.RugbyTeamId == dbTeam.Id).FirstOrDefault();

                    if (dbEntry == null)
                    {
                        var newEntry = new RugbyPlayerLineup()
                        {
                            RugbyPlayerId = dbPlayer.Id,
                            RugbyFixtureId = dbFixture.Id,
                            RugbyTeamId = dbTeam.Id,
                            RugbyFixture = dbFixture,
                            RugbyTeam = dbTeam,
                            RugbyPlayer = dbPlayer,
                            IsCaptain = isCaptain,
                            IsSubstitute = isSubstitute,
                            PositionName = positionName,
                            JerseyNumber = shirtNumber
                        };

                        _rugbyPlayerLineupsRepository.Add(newEntry);
                    }
                    else
                    {
                        dbEntry.IsSubstitute = isSubstitute;
                        dbEntry.PositionName = positionName;
                        dbEntry.JerseyNumber = shirtNumber;

                        _rugbyPlayerLineupsRepository.Update(dbEntry);
                    }
                }
            }

            await _rugbyPlayerLineupsRepository.SaveAsync();
            await _rugbyPlayerRepository.SaveAsync();
        }

        public async Task IngestLogsForTournamentSeason(CancellationToken cancellationToken, int providerTournamentId, int seasonId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var seasons = (await _rugbySeasonRepository.AllAsync());

            var season =
                    seasons
                        .Where(s =>
                            s.ProviderSeasonId == seasonId &&
                            s.RugbyTournament.ProviderTournamentId == providerTournamentId);

            if (!season.Any())
                return;

            var logType = season.First().RugbyLogType;

            if (logType == RugbyLogType.FlatLogs)
            {
                RugbyFlatLogsResponse logs =
                _statsProzoneIngestService.IngestFlatLogsForTournament(
                    providerTournamentId, seasonId);

                await PersistFlatLogs(cancellationToken, logs);

                _mongoDbRepository.Save(logs);
            }
            else
            {
                RugbyGroupedLogsResponse logs =
                _statsProzoneIngestService.IngestGroupedLogsForTournament(
                    providerTournamentId, seasonId);
               
                await PersistGroupedLogs(cancellationToken, logs);

                _mongoDbRepository.Save(logs);
            }
        }

        public async Task IngestLineupsForPastGames(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var now = DateTime.UtcNow;
            var NowMinus30days = DateTime.UtcNow - TimeSpan.FromDays(1);

            var gamesInPastDay =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(
                            fixture => fixture.RugbyTournament != null &&
                                       fixture.RugbyTournament.IsEnabled &&
                                       fixture.StartDateTime < now &&
                                       fixture.StartDateTime >= NowMinus30days &&
                                       (fixture.RugbyFixtureStatus == RugbyFixtureStatus.Result));

            await IngestLineUpsForFixtures(cancellationToken, gamesInPastDay);
        }
    }
}