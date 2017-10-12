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
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixturerRepository,
            IBaseEntityFrameworkRepository<RugbyPlayer> rugbyPlayerRepository,
            IBaseEntityFrameworkRepository<RugbyFlatLog> rugbyFlatLogsRepository,
            IBaseEntityFrameworkRepository<RugbyLogGroup> rugbyLogGroupRepository,
            IBaseEntityFrameworkRepository<RugbyGroupedLog> rugbyGroupedLogsRepository,
            IBaseEntityFrameworkRepository<RugbyPlayerLineup> rugbyPlayerLineupsRepository,
            IBaseEntityFrameworkRepository<RugbyCommentary> rugbyCommentaryRepository,
            IBaseEntityFrameworkRepository<RugbyMatchStatistics> rugbyMatchStatisticsRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournamentRepository,
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
            _rugbyFixturesRepository = rugbyFixturerRepository;
            _rugbyPlayerRepository = rugbyPlayerRepository;
            _rugbyFlatLogsRepository = rugbyFlatLogsRepository;
            _rugbyLogGroupRepository = rugbyLogGroupRepository;
            _rugbyGroupedLogsRepository = rugbyGroupedLogsRepository;
            _rugbyPlayerLineupsRepository = rugbyPlayerLineupsRepository;
            _rugbyCommentaryRepository = rugbyCommentaryRepository;
            _rugbyMatchStatisticsRepository = rugbyMatchStatisticsRepository;
            _schedulerTrackingRugbyTournamentRepository = schedulerTrackingRugbyTournamentRepository;
            _rugbyService = rugbyService;
        }

        public async Task IngestReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            // If there is a live game happening while we are ingesting entities,
            // Stop ingesting entities. It might interfere with the repo access.
            if (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0)
                return;

            var entitiesResponse =
                _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            await PersistVenues(cancellationToken, entitiesResponse);
            await PersistTeams(cancellationToken, entitiesResponse);
            await PersistTournaments(cancellationToken, entitiesResponse);
            await PersistPlayers(cancellationToken, entitiesResponse);
            await IngestRugbyTournamentSeasons(cancellationToken);

            _mongoDbRepository.Save(entitiesResponse);
        }

        private async Task PersistPlayersInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            IList<RugbyPlayer> playersAlreadyInDb = (await _rugbyPlayerRepository.AllAsync()).ToList();

            foreach (var player in entitiesResponse.Entities.players)
            {
                var playerInDb = playersAlreadyInDb.Where(p => p.ProviderPlayerId == player.id).FirstOrDefault();

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
                    _rugbyPlayerRepository.Add(newPlayer);
                }
            }
        }

        private async Task PersistRugbyTournamentsInSchedulerTrackingRugbyTournamentTable(CancellationToken cancellationToken)
        {
            var activeTournaments = await _rugbyService.GetActiveTournaments();
            var seasonsAlreadyInDb = (await _schedulerTrackingRugbySeasonRepository.AllAsync()).ToList();

            foreach (var tournament in activeTournaments)
            {
                var currentSeasonId = _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var seasonInDb =
                    seasonsAlreadyInDb
                        .Where(s => s.TournamentId == tournament.Id && s.RugbySeasonStatus == RugbySeasonStatus.InProgress).FirstOrDefault();

                if (seasonInDb != null)
                {
                    var tournamentInDb =
                        (await _schedulerTrackingRugbyTournamentRepository.AllAsync())
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
        }

        private async Task PersistTeamsInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var teamsAlreadyInDb = (await _rugbyTeamRepository.AllAsync()).ToList();

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
        }

        private async Task PersistVenuesInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;


            var venuesAlreadyInDb = (await _rugbyVenueRepository.AllAsync()).ToList();

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
        }

        private async Task IngestRugbyTournamentSeasons(CancellationToken cancellationToken)
        {
            var activeTournaments = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.IsEnabled);
            foreach (var tournament in activeTournaments)
            {
                var season = _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, DateTime.Now.Year);

                await PersistRugbySeasonDataToSystemSportsDataRepository(cancellationToken, season);
            }

            await _rugbySeasonRepository.SaveAsync();
        }

        private async Task PersistRugbySeasonDataToSystemSportsDataRepository(CancellationToken cancellationToken, RugbySeasonResponse season)
        {
            var providerTournamentId = season.RugbySeasons.competitionId;

            if (season.RugbySeasons.season.Count == 0)
                return;

            var providerSeasonId = season.RugbySeasons.season.First().id;

            var isSeasonCurrentlyActive = season.RugbySeasons.season.First().currentSeason;

            var seasonEntry =
                    (await _rugbySeasonRepository.AllAsync())
                    .Where(s => s.RugbyTournament.ProviderTournamentId == providerTournamentId && s.ProviderSeasonId == providerSeasonId)
                    .FirstOrDefault();

            var tour = (await _rugbyTournamentRepository.AllAsync()).Where(t => t.ProviderTournamentId == providerTournamentId).FirstOrDefault();

            var newEntry = new RugbySeason()
            {
                ProviderSeasonId = providerSeasonId,
                RugbyTournament = tour,
                IsCurrent = isSeasonCurrentlyActive,
                Name = season.RugbySeasons.season.First().name,
                DataProvider = DataProvider.StatsProzone
            };

            // Not in repo?
            if (seasonEntry == null)
            {
                _rugbySeasonRepository.Add(newEntry);
            }
            else
            {
                seasonEntry.StartDateTime = newEntry.StartDateTime;
                seasonEntry.Name = newEntry.Name;
                seasonEntry.IsCurrent = newEntry.IsCurrent;

                _rugbySeasonRepository.Update(seasonEntry);
            }
        }

        public async Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if ((await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0))
                return;

            await IngestRugbyTournamentSeasons(cancellationToken);

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

        private static SemaphoreSlim PersistVenuesControl = new SemaphoreSlim(1, 1);
        private async Task PersistVenues(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            try
            {
                await PersistVenuesControl.WaitAsync(cancellationToken);
                await PersistVenuesInPublicSportsDataRepository(cancellationToken, entitiesResponse);

                await _rugbyVenueRepository.SaveAsync();
            }
            finally
            {
                PersistVenuesControl.Release();
            }
        }

        private static SemaphoreSlim PersistPlayersControl = new SemaphoreSlim(1, 1);
        private async Task PersistPlayers(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            try
            {
                await PersistPlayersControl.WaitAsync(cancellationToken);
                await PersistPlayersInPublicSportsDataRepository(cancellationToken, entitiesResponse);

                await _rugbyPlayerRepository.SaveAsync();
            }
            finally
            {
                PersistPlayersControl.Release();
            }
        }

        private static SemaphoreSlim PersistFlatLogsControl = new SemaphoreSlim(1, 1);
        private async Task PersistFlatLogs(CancellationToken cancellationToken, RugbyFlatLogsResponse flatLogsResponse)
        {
            try
            {
                await PersistFlatLogsControl.WaitAsync(cancellationToken);
                await PersistLogsInPublicSportsDataRepository(cancellationToken, flatLogsResponse);
                await _rugbyFlatLogsRepository.SaveAsync();
            }
            finally
            {
                PersistFlatLogsControl.Release();
            }
        }

        private async Task PersistLogsInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyFlatLogsResponse logsResponse)
        {
            var tournamentId = logsResponse.RugbyFlatLogs.competitionId;
            var seasonId = logsResponse.RugbyFlatLogs.seasonId;
            var roundNumber = logsResponse.RugbyFlatLogs.roundNumber;

            IList<RugbyFlatLog> laddersAlreadyInDb = (await _rugbyFlatLogsRepository.AllAsync()).ToList();

            if (logsResponse.RugbyFlatLogs.ladderposition == null)
                return;

            foreach (var position in logsResponse.RugbyFlatLogs.ladderposition)
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
        }

        private SemaphoreSlim PersistFixturesDataControl = new SemaphoreSlim(1, 1);
        private async Task PersistFixturesData(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            try
            {
                await PersistFixturesDataControl.WaitAsync();

                await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, fixtures);

                await PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(fixtures);
                _mongoDbRepository.Save(fixtures);
            }
            finally
            {
                PersistFixturesDataControl.Release();
            }
        }

        private static SemaphoreSlim PersistTournamentsControl = new SemaphoreSlim(1, 1);
        private async Task PersistTournaments(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            try
            {
                await PersistTournamentsControl.WaitAsync(cancellationToken);
                await PersistRugbyTournamentsInRepositoryAsync(entitiesResponse, cancellationToken);
            }
            finally
            {
                PersistTournamentsControl.Release();
            }
        }

        private static SemaphoreSlim PersistTeamsControl = new SemaphoreSlim(1, 1);
        private async Task PersistTeams(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            try
            {
                await PersistTeamsControl.WaitAsync(cancellationToken);
                await PersistTeamsInPublicSportsDataRepository(cancellationToken, entitiesResponse);

                await _rugbyTeamRepository.SaveAsync();
            }
            finally
            {
                PersistTeamsControl.Release();
            }
        }

        private async Task PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(RugbyFixturesResponse fixtures)
        {
            IList<RugbyFixture> fixturesAlreadyInDb = (await _rugbyFixturesRepository.AllAsync()).ToList();

            foreach (var roundFixtures in fixtures.Fixtures.roundFixtures)
            {
                foreach (var fixture in roundFixtures.gameFixtures)
                {
                    var fixtureInDb = fixturesAlreadyInDb.Where(f => f.ProviderFixtureId == fixture.gameId).FirstOrDefault();
                    var fixtureGuid = fixtureInDb.Id;
                    var tournamentGuid = fixtureInDb.RugbyTournament.Id;

                    var fixtureSchedule =
                            (await _schedulerTrackingRugbyFixtureRepository.AllAsync()).ToList()
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
                        fixtureSchedule.RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixture.gameStateName);
                        fixtureSchedule.StartDateTime = fixtureInDb.StartDateTime;
                        
                        if (HasFixtureEnded(fixture.gameStateName) &&
                           fixtureSchedule.EndedDateTime == DateTimeOffset.MinValue)
                        {
                            fixtureSchedule.EndedDateTime =
                                fixtureSchedule.StartDateTime
                                    .AddSeconds(
                                        fixture.gameSeconds);
                        }

                        _schedulerTrackingRugbyFixtureRepository.Update(fixtureSchedule);
                    }
                }
            }

            await _schedulerTrackingRugbyFixtureRepository.SaveAsync();
        }

        private bool HasFixtureEnded(string gameStateName)
        {
            return GetFixtureStatusFromProviderFixtureState(gameStateName) == RugbyFixtureStatus.PostMatch;
        }

        private SemaphoreSlim PersistRugbyFixturesToPublicSportsRepositoryControl = new SemaphoreSlim(1, 1);
        private async Task PersistRugbyFixturesToPublicSportsRepository(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                await PersistRugbyFixturesToPublicSportsRepositoryControl.WaitAsync();

                var tournament = (await _rugbyTournamentRepository.AllAsync()).ToList().Where(t => t.ProviderTournamentId == fixtures.Fixtures.competitionId).FirstOrDefault();

                var allFixtures = (await _rugbyFixturesRepository.AllAsync()).ToList();
                var allTeams = (await _rugbyTeamRepository.AllAsync()).ToList();
                var allVenues = (await _rugbyVenueRepository.AllAsync()).ToList();

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
                            if (fixtureInDb.RugbyFixtureStatus == RugbyFixtureStatus.PostMatch)
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
            finally
            {
                PersistRugbyFixturesToPublicSportsRepositoryControl.Release();
            }
        }

        private RugbyFixtureStatus GetFixtureStatusFromProviderFixtureState(string gameStateName)
        {
            if (gameStateName == "Pre Game")
                return RugbyFixtureStatus.PreMatch;

            if (gameStateName == "Final")
                return RugbyFixtureStatus.PostMatch;

            if (gameStateName == "Game End")
                return RugbyFixtureStatus.PostMatch;

            if (gameStateName == "Full Time")
                return RugbyFixtureStatus.Result;

            return RugbyFixtureStatus.InProgress;
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

            if (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0)
                return;

            var activeTournaments =
                await _rugbyService.GetActiveTournaments();

            await IngestLogsHelper(activeTournaments, cancellationToken);
        }

        private static SemaphoreSlim IngestLogsForCurrentTournamentsControl = new SemaphoreSlim(1, 1);
        public async Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                await IngestLogsForCurrentTournamentsControl.WaitAsync();

                if (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0)
                    return;

                var currentTournaments =
                    await _rugbyService.GetCurrentTournaments();

                await IngestLogsHelper(currentTournaments, cancellationToken);
            }
            finally
            {
                IngestLogsForCurrentTournamentsControl.Release();
            }
        }

        public async Task IngestLogsHelper(IEnumerable<RugbyTournament> tournaments, CancellationToken cancellationToken)
        {
            IList<RugbySeason> seasons = (await _rugbySeasonRepository.AllAsync()).ToList();

            foreach (var tournament in tournaments)
            {
                int activeSeasonIdForTournament =
                        await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);

                var season = seasons.Where(s => s.ProviderSeasonId == activeSeasonIdForTournament &&
                                                 s.RugbyTournament.ProviderTournamentId == tournament.ProviderTournamentId).ToList();

                if (season.Count == 0)
                    return;

                var logType = season.First().RugbyLogType;

                if (logType == RugbyLogType.FlatLogs)
                {
                    RugbyFlatLogsResponse logs =
                    _statsProzoneIngestService.IngestFlatLogsForTournament(
                        tournament.ProviderTournamentId, activeSeasonIdForTournament);

                    // TODO: Also persist in SQL DB.
                    await PersistFlatLogs(cancellationToken, logs);

                    _mongoDbRepository.Save(logs);
                }
                else
                {
                    RugbyGroupedLogsResponse logs =
                    _statsProzoneIngestService.IngestGroupedLogsForTournament(
                        tournament.ProviderTournamentId, activeSeasonIdForTournament);

                    // TODO: Also persist in SQL DB.
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

        private async Task PersistRugbyTournamentsInRepositoryAsync(RugbyEntitiesResponse entitiesResponse, CancellationToken cancellationToken)
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
            var activeTournaments = (await _rugbyService.GetActiveTournaments()).ToList();

            foreach (var tournament in activeTournaments)
            {
                var seasonId = await _rugbyService.GetCurrentProviderSeasonIdForTournament(cancellationToken, tournament.Id);
                var results = _statsProzoneIngestService.IngestFixturesForTournament(tournament, seasonId, cancellationToken);
                await PersistFixturesResultsInRepository(results, cancellationToken);
            }
        }

        private async Task PersistFixturesResultsInRepository(RugbyFixturesResponse fixturesResponse, CancellationToken cancellationToken)
        {
            // Only persist data for completed matches.
            // The provider endpoint for results is just a variation of the fixtures endpoint,
            // It will also return results for completed matches.
        }

        private static SemaphoreSlim IngestFixturesForTournamentSeasonControl = new SemaphoreSlim(1, 1);
        public async Task IngestFixturesForTournamentSeason(CancellationToken cancellationToken, int tournamentId, int seasonId)
        {
            try
            {
                await IngestFixturesForTournamentSeasonControl.WaitAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested || (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0))
                {
                    return;
                }

                await IngestRugbyTournamentSeasons(cancellationToken);

                var fixtures =
                    _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                        tournamentId, seasonId, cancellationToken);

            
                await PersistFixturesData(cancellationToken, fixtures);
            }
            finally
            {
                IngestFixturesForTournamentSeasonControl.Release();
            }
        }

        public async Task IngestResultsForCurrentDayFixtures(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0)
                return;

            var currentRoundFixtures = await GetCurrentDayRoundFixturesForActiveTournaments();

            foreach (var round in currentRoundFixtures)
            {
                var results = await _statsProzoneIngestService.IngestFixtureResults(round.Item1, round.Item2, round.Item3);

                // TODO: Also persist in SQL DB.
            }
        }

        public async Task IngestResultsForFixturesInResultsState(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixtures = await GetRoundFixturesInResultsStateForActiveTournaments();

            foreach (var fixture in fixtures)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var results = await _statsProzoneIngestService.IngestFixtureResults(fixture.Item1, fixture.Item2, fixture.Item3);

                // TODO: Also persist in SQL DB.
            }
        }

        #region TempHelpers_Remove_Once_Properly_Implemented
        /// <summary>
        /// Returs round fixtures playing on current day.
        /// </summary>
        /// <param name="tournamemnts"></param>
        /// <returns></returns>
        private static SemaphoreSlim GetCurrentDayRoundFixturesForActiveTournamentsControl = new SemaphoreSlim(1, 1);
        public async Task<List<Tuple<int, int, int>>> GetCurrentDayRoundFixturesForActiveTournaments()
        {
            try
            {
                await GetCurrentDayRoundFixturesForActiveTournamentsControl.WaitAsync();

                var activeTournaments = await _rugbyService.GetActiveTournaments();

                //TODO: Must be able to deduce the following fields via repository
                int tournamentId = 121;
                int seasonId = 2017;
                int roundId = 1;

                return new List<Tuple<int, int, int>> { new Tuple<int, int, int>(tournamentId, seasonId, roundId) };
            }
            finally
            {
                GetCurrentDayRoundFixturesForActiveTournamentsControl.Release();
            }
        }

        /// <summary>
        /// Returs round fixtures that has been played.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<List<Tuple<int, int, int>>> GetRoundFixturesInResultsStateForActiveTournaments()
        {
            //TODO:
            return await GetCurrentDayRoundFixturesForActiveTournaments();
        }
        #endregion

        private static SemaphoreSlim IngestOneMonthsFixturesForTournamentControl = new SemaphoreSlim(1, 1);
        public async Task IngestOneMonthsFixturesForTournament(CancellationToken cancellationToken, int providerTournamentId)
        {
            try
            {
                await IngestOneMonthsFixturesForTournamentControl.WaitAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested || (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0))
                {
                    return;
                }

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
            finally
            {
                IngestOneMonthsFixturesForTournamentControl.Release();
            }

        }

        private static SemaphoreSlim GetEnabledTournamentForIdControl = new SemaphoreSlim(1, 1);
        private async Task<RugbyTournament> GetEnabledTournamentForId(int id)
        {
            try
            {
                GetEnabledTournamentForIdControl.Wait();

                var tournament = (await _rugbyService.GetCurrentTournaments()).Where(t => t.ProviderTournamentId == id).FirstOrDefault();

                return tournament;
            }
            finally
            {
                GetEnabledTournamentForIdControl.Release();
            }
        }

        private RugbyFixturesResponse RemoveFixturesThatHaveBeenCompleted(RugbyFixturesResponse fixtures)
        {
            foreach (var round in fixtures.Fixtures.roundFixtures)
            {
                round.gameFixtures
                    .RemoveAll(
                        f => GetFixtureStatusFromProviderFixtureState(f.gameStateName) == RugbyFixtureStatus.PostMatch);
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

        private static SemaphoreSlim IngestMatchStatsForFixtureControl = new SemaphoreSlim(1, 1);
        public async Task IngestMatchStatsForFixture(CancellationToken cancellationToken, long providerFixtureId)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var matchStatsResponse =
                    await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, providerFixtureId);

                var eventsFlowResponse =
                    await _statsProzoneIngestService.IngestEventsFlow(cancellationToken, providerFixtureId);

                try
                {
                    await IngestMatchStatsForFixtureControl.WaitAsync(cancellationToken);

                    await IngestCommentary(cancellationToken, eventsFlowResponse.RugbyEventsFlow.commentaryFlow, providerFixtureId);
                    await IngestMatchStatistics(cancellationToken, providerFixtureId);
                    await IngestScoresForFixture(cancellationToken, matchStatsResponse);
                    await IngestFixtureState(cancellationToken, matchStatsResponse);

                    _mongoDbRepository.Save(eventsFlowResponse);
                }
                finally
                {
                    IngestMatchStatsForFixtureControl.Release();
                }


                //// Check if should stop looping?
                if (GetFixtureStatusFromProviderFixtureState(
                        matchStatsResponse.RugbyMatchStats.gameState) == RugbyFixtureStatus.PostMatch)
                    break;

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        private async Task IngestFixtureState(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
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

        private async Task IngestScoresForFixture(CancellationToken cancellationToken, RugbyMatchStatsResponse matchStatsResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var allFixtures = (await _rugbyFixturesRepository.AllAsync()).ToList();

            var scores = GetScoresForFixture(cancellationToken, matchStatsResponse);
            var fixtureId = matchStatsResponse.RugbyMatchStats.gameId;

            var fixtureInDb = allFixtures.Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();

            if(fixtureInDb == null)
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

        public async Task IngestMatchStatistics(CancellationToken cancellationToken, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var matchStatsResponse =
                    await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, providerFixtureId);

            var fixture = (await _rugbyFixturesRepository.AllAsync()).Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();
            var teamsInRepo = (await _rugbyTeamRepository.AllAsync());

            var teamsFromProvider = matchStatsResponse.RugbyMatchStats.teams;
            
            foreach(var teamMatch in teamsFromProvider.teamsMatch)
            {

                var team = teamsInRepo.Where(t => t.ProviderTeamId == teamMatch.teamId).FirstOrDefault();

                var stats = teamMatch.teamStats.matchStats.matchStat;
                var statsInDb = (await _rugbyMatchStatisticsRepository.AllAsync()).Where(s => s.RugbyFixtureId == fixture.Id && s.RugbyTeamId == team.Id).FirstOrDefault();

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
                    DropGoalsMissed = (int) (statsMap.GetValueOrDefault(2049) - statsMap.GetValueOrDefault(2050)),
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

                if(statsInDb == null)
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
            _mongoDbRepository.Save(matchStatsResponse);
        }

        private IDictionary<int, double> MakeStatisticsMap(IList<MatchStat> matchStats)
        {
            var dictionary = new Dictionary<int, double>();
            foreach(var stat in matchStats)
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

            var fixtures = (await _rugbyFixturesRepository.AllAsync());
            var teams = (await _rugbyTeamRepository.AllAsync());
            var players = (await _rugbyPlayerRepository.AllAsync());

            DateTime d0 = DateTime.Now;

            foreach (var comment in commentary.commentaryEvent)
            {
                var commentText = comment.commentary;
                var commentTimeInSeconds = comment.gameSeconds;
                var commentaryTimeInMinutes = commentTimeInSeconds / 60;
                var gameTimeDisplayHoursMinutesSeconds = comment.gameTime;
                var gameTimeDisplayMinutesSeconds = comment.GameMinutes;

                var fixture = fixtures.Where(f => f.ProviderFixtureId == providerFixtureId).FirstOrDefault();
                var team = teams.Where(t => t.ProviderTeamId == comment.teamId).FirstOrDefault();
                var player = players.Where(p => p.ProviderPlayerId == comment.playerId).FirstOrDefault();

                var dbCommentary = (await _rugbyCommentaryRepository.AllAsync()).Where(c =>
                                                c.GameTimeRawSeconds == commentTimeInSeconds &&
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
                }
                else
                {
                    // There isnt something which uniquely identifies 
                }
            }

            await _rugbyCommentaryRepository.SaveAsync();

            Console.WriteLine(providerFixtureId + ": " + (DateTime.Now - d0).TotalMilliseconds + "ms");
        }

        public async Task IngestLineupsForUpcomingGames(CancellationToken cancellationToken)
        {
            if (await _rugbyService.GetLiveFixturesCount(cancellationToken) > 0)
                return;

            var now = DateTime.UtcNow;
            var NowPlusTwoDays = DateTime.UtcNow + TimeSpan.FromDays(2);

            var gamesInTheNext2Days =
                    (await _rugbyFixturesRepository.AllAsync())
                        .Where(
                            f => f.StartDateTime >= now && f.StartDateTime <= NowPlusTwoDays)
                        .ToList();

            foreach (var fixture in gamesInTheNext2Days)
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
                    var isSubstitute = player.shirtNum == 16;

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

            IList<RugbySeason> seasons = (await _rugbySeasonRepository.AllAsync()).ToList();

            var season = seasons.Where(s => s.ProviderSeasonId == seasonId &&
                                            s.RugbyTournament.ProviderTournamentId == providerTournamentId).ToList();

            // TODO: Also persist in SQL DB.
            if (season.Count == 0)
                return;

            var logType = season.First().RugbyLogType;

            if (logType == RugbyLogType.FlatLogs)
            {
                RugbyFlatLogsResponse logs =
                _statsProzoneIngestService.IngestFlatLogsForTournament(
                    providerTournamentId, seasonId);

                // TODO: Also persist in SQL DB.
                await PersistFlatLogs(cancellationToken, logs);

                _mongoDbRepository.Save(logs);
            }
            else
            {
                RugbyGroupedLogsResponse logs =
                _statsProzoneIngestService.IngestGroupedLogsForTournament(
                    providerTournamentId, seasonId);

                // TODO: Also persist in SQL DB.
                await PersistGroupedLogs(cancellationToken, logs);

                _mongoDbRepository.Save(logs);
            }
        }
    }
}
