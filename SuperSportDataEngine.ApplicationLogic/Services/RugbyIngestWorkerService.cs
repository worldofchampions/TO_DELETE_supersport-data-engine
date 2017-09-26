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

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepoitory;
        private readonly IBaseEntityFrameworkRepository<RugbyVenue> _rugbyVenueRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTeam> _rugbyTeamRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixturesRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournamentRepository;
        private readonly IRugbyService _rugbyService;

        static SemaphoreSlim FixuresSemaphore = new SemaphoreSlim(1, 1);

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
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> schedulerTrackingRugbyTournamentRepository,
            IRugbyService rugbyService)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _schedulerTrackingRugbyFixtureRepoitory = schedulerTrackingRugbyFixtureRepoitory;
            _rugbyVenueRepository = rugbyVenueRepository;
            _rugbyTeamRepository = rugbyTeamRepository;
            _rugbyFixturesRepository = rugbyFixturerRepository;
            _schedulerTrackingRugbyTournamentRepository = schedulerTrackingRugbyTournamentRepository;
            _rugbyService = rugbyService;
        }

        public async Task IngestReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var entitiesResponse =
                _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            PersistVenuesInPublicSportsDataRepository(cancellationToken, entitiesResponse);
            PersistTeamsInPublicSportsDataRepository(cancellationToken, entitiesResponse);
            PersistRugbyTournamentsInRepositoryAsync(entitiesResponse, cancellationToken);

            await IngestRugbyTournamentSeasons(cancellationToken);

            _mongoDbRepository.Save(entitiesResponse);

            // Save all the affected repositories.
            await _rugbyVenueRepository.SaveAsync();
            await _rugbyTeamRepository.SaveAsync();
            await _rugbyTournamentRepository.SaveAsync();
        }

        private void PersistRugbyTournamentsInSystemTrackingTable()
        {
            var activeTournaments = _rugbyService.GetActiveTournaments();

            foreach (var tournament in activeTournaments)
            {
                var currentSeasonId = _rugbyService.GetCurrentProviderSeasonIdForTournament(tournament.Id);

                var seasonInDb =
                    _schedulerTrackingRugbySeasonRepository
                        .Where(s => s.TournamentId == tournament.Id && s.RugbySeasonStatus == RugbySeasonStatus.InProgress).FirstOrDefault();

                if (seasonInDb != null)
                {
                    var tournamentInDb =
                        _schedulerTrackingRugbyTournamentRepository
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

        private void PersistTeamsInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach (var team in entitiesResponse.Entities.teams)
            {
                var teamInDb = _rugbyTeamRepository.Where(t => t.ProviderTeamId == team.id).FirstOrDefault();

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

        private void PersistVenuesInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach (var venue in entitiesResponse.Entities.venues)
            {
                // Lookup in db
                var venueInDb = _rugbyVenueRepository.Where(v => v.ProviderVenueId == venue.id).FirstOrDefault();

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
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled);
            foreach (var tournament in activeTournaments)
            {
                var season = _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, DateTime.Now.Year);

                PersistRugbySeasonDataToSystemSportsDataRepository(cancellationToken, season);
            }

            await _rugbySeasonRepository.SaveAsync();
        }

        private void PersistRugbySeasonDataToSystemSportsDataRepository(CancellationToken cancellationToken, RugbySeasonResponse season)
        {
            var providerTournamentId = season.RugbySeasons.competitionId;
            var providerSeasonId = season.RugbySeasons.season.First().id;
            var isSeasonCurrentlyActive = season.RugbySeasons.season.First().currentSeason;

            var seasonEntry =
                    _rugbySeasonRepository
                    .Where(s => s.RugbyTournament.ProviderTournamentId == providerTournamentId && s.ProviderSeasonId == providerSeasonId)
                    .FirstOrDefault();

            var tour = _rugbyTournamentRepository.Where(t => t.ProviderTournamentId == providerTournamentId).FirstOrDefault();

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

            var activeTournaments =
                _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var fixtures =
                    _statsProzoneIngestService.IngestFixturesForTournament(
                        tournament, cancellationToken);

                PersistFixturesData(cancellationToken, fixtures);

                // TODO: Also persist in SQL DB.
                
                PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(fixtures);
                PersistRugbyTournamentsInSystemTrackingTable();
                _mongoDbRepository.Save(fixtures);
            }

            // Saving to the DB's should not be done inside a for-loop (like above)
            
            await _schedulerTrackingRugbySeasonRepository.SaveAsync();
            await _schedulerTrackingRugbyTournamentRepository.SaveAsync();
        }

        private async Task PersistFixturesData(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            await FixuresSemaphore.WaitAsync();

            try
            {
                PersistRugbyFixturesToPublicSportsRepository(cancellationToken, fixtures);
                PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(fixtures);

                await _rugbyFixturesRepository.SaveAsync();
                await _schedulerTrackingRugbyFixtureRepoitory.SaveAsync();
            }
            finally
            {
                FixuresSemaphore.Release();
            }
        }

        private void PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(RugbyFixturesResponse fixtures)
        {
            foreach (var roundFixtures in fixtures.Fixtures.roundFixtures)
            {
                foreach (var fixture in roundFixtures.gameFixtures)
                {
                    var fixtureInDb = _rugbyFixturesRepository.Where(f => f.ProviderFixtureId == fixture.gameId).FirstOrDefault();
                    var fixtureGuid = fixtureInDb.Id;
                    var tournamentGuid = fixtureInDb.RugbyTournament.Id;

                    var fixtureSchedule =
                            _schedulerTrackingRugbyFixtureRepoitory
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

                        _schedulerTrackingRugbyFixtureRepoitory.Add(newFixtureSchedule);
                    }
                    else
                    {
                        // If the schedule already is in the system repo
                        // we need to update the status of the game.
                        fixtureSchedule.RugbyFixtureStatus = GetFixtureStatusFromProviderFixtureState(fixture.gameStateName);

                        if (HasFixtureEnded(fixture.gameStateName) &&
                           fixtureSchedule.EndedDateTime == DateTimeOffset.MinValue)
                        {
                            fixtureSchedule.EndedDateTime =
                                fixtureSchedule.StartDateTime
                                    .AddSeconds(
                                        fixture.gameSeconds);
                        }

                        _schedulerTrackingRugbyFixtureRepoitory.Update(fixtureSchedule);
                    }
                }
            }
        }

        private bool HasFixtureEnded(string gameStateName)
        {
            return gameStateName == "Final";
        }

        private void PersistRugbyFixturesToPublicSportsRepository(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var tournament = _rugbyTournamentRepository.Where(t => t.ProviderTournamentId == fixtures.Fixtures.competitionId).FirstOrDefault();

            foreach (var roundFixture in fixtures.Fixtures.roundFixtures)
            {
                foreach (var fixture in roundFixture.gameFixtures)
                {
                    var fixtureId = fixture.gameId;

                    // Lookup in Db
                    var fixtureInDb = _rugbyFixturesRepository.Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();
                    DateTimeOffset.TryParse(fixture.startTimeUTC, out DateTimeOffset startTime);
                    var teams = fixture.teams.ToArray();
                    // We need temporary variables here.
                    // Cannot use indexing in Linq Where clause.
                    var team0 = teams[0];
                    var team1 = teams[1];

                    var teamA = _rugbyTeamRepository.Where(t => t.ProviderTeamId == team0.teamId).FirstOrDefault();
                    var teamB = _rugbyTeamRepository.Where(t => t.ProviderTeamId == team1.teamId).FirstOrDefault();
                    var newFixture = new RugbyFixture()
                    {
                        LegacyFixtureId = fixtureId,
                        ProviderFixtureId = fixtureId,
                        StartDateTime = startTime,
                        RugbyVenue = _rugbyVenueRepository.Where(v => v.ProviderVenueId == fixture.venueId).FirstOrDefault(),
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
                        if (fixtureInDb.RugbyFixtureStatus == RugbyFixtureStatus.Final)
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
        }

        private RugbyFixtureStatus GetFixtureStatusFromProviderFixtureState(string gameStateName)
        {
            if (gameStateName == "Pre Game")
                return RugbyFixtureStatus.PreMatch;

            if (gameStateName == "Final")
                return RugbyFixtureStatus.Final;

            if (gameStateName == "Game End")
                return RugbyFixtureStatus.GameEnd;

            if (gameStateName == "Full Time")
                return RugbyFixtureStatus.Result;

            return RugbyFixtureStatus.InProgress;
        }

        private void PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(RugbyFixturesResponse fixtures)
        {
            var season =
                _rugbySeasonRepository
                .Where(
                    s => s.ProviderSeasonId == fixtures.Fixtures.seasonId && s.RugbyTournament.ProviderTournamentId == fixtures.Fixtures.competitionId)
                    .FirstOrDefault();

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

            var seasonInDb = _schedulerTrackingRugbySeasonRepository.Where(s => s.SeasonId == seasonId && s.TournamentId == tournamentId).FirstOrDefault();

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

            var activeTournaments =
                _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var logs =
                    _statsProzoneIngestService.IngestLogsForTournament(
                        tournament, cancellationToken);

                // TODO: Also persist in SQL DB.

                _mongoDbRepository.Save(logs);
            }
        }

        public async Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var currentTournaments =
                _rugbyService.GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                var logs =
                    _statsProzoneIngestService.IngestLogsForTournament(
                        tournament, cancellationToken);

                // TODO: Also persist in SQL DB.

                _mongoDbRepository.Save(logs);
            }
        }

        private void PersistRugbyTournamentsInRepositoryAsync(RugbyEntitiesResponse entitiesResponse, CancellationToken cancellationToken)
        {
            foreach (var competition in entitiesResponse.Entities.competitions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var entry = _rugbyTournamentRepository
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
                    entry.LegacyTournamentId = newEntry.LegacyTournamentId;
                    entry.Name = newEntry.Name;
                    entry.Slug = newEntry.Slug;
                    entry.LogoUrl = newEntry.LogoUrl;
                    entry.Abbreviation = newEntry.Abbreviation;

                    _rugbyTournamentRepository.Update(entry);
                }
            }
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
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var results = _statsProzoneIngestService.IngestFixturesForTournament(tournament, cancellationToken);
                await PersistFixturesResultsInRepository(results, cancellationToken);
            }
        }

        private async Task PersistFixturesResultsInRepository(RugbyFixturesResponse fixturesResponse, CancellationToken cancellationToken)
        {
            // Only persist data for completed matches.
            // The provider endpoint for results is just a variation of the fixtures endpoint,
            // It will also return results for completed matches.
        }

        public async Task IngestFixturesForTournamentSeason(CancellationToken cancellationToken, int tournamentId, int seasonId)
        {
            //if (cancellationToken.IsCancellationRequested)
            //    return;

            //var fixtures =
            //    _statsProzoneIngestService.IngestFixturesForTournamentSeason(
            //        tournamentId, seasonId, cancellationToken);

            //// TODO: Also persist in SQL DB.
            //PersistRugbyFixturesToPublicSportsRepository(cancellationToken, fixtures);
            //PersistRugbyFixturesToSchedulerTrackingRugbyFixturesTable(fixtures);

            //await _rugbyFixturesRepository.SaveAsync();
            //await _schedulerTrackingRugbyFixtureRepoitory.SaveAsync();
        }

        public async Task IngestResultsForCurrentDayFixtures(CancellationToken cancellationToken)
        {
            var currentRoundFixtures = GetCurrentDayRoundFixturesForActiveTournaments();

            foreach (var round in currentRoundFixtures)
            {
                var results = await _statsProzoneIngestService.IngestFixtureResults(round.Item1, round.Item2, round.Item3);

                // TODO: Also persist in SQL DB.
            }
        }

        public async Task IngestResultsForFixturesInResultsState(CancellationToken cancellationToken)
        {
            var fixtures = GetRoundFixturesInResultsStateForActiveTournaments();

            foreach (var fixture in fixtures)
            {
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
        public List<Tuple<int, int, int>> GetCurrentDayRoundFixturesForActiveTournaments()
        {
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled);

            //TODO: Must be able to deduce the following fields via repository
            int tournamentId = 121;
            int seasonId = 2017;
            int roundId = 1;

            return new List<Tuple<int, int, int>> { new Tuple<int, int, int>(tournamentId, seasonId, roundId) };
        }

        /// <summary>
        /// Returs round fixtures that has been played.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Tuple<int, int, int>> GetRoundFixturesInResultsStateForActiveTournaments()
        {
            //TODO:
            return GetCurrentDayRoundFixturesForActiveTournaments();
        }
        #endregion

        public async Task IngestOneMonthsFixturesForTournament(CancellationToken cancellationToken, int providerTournamentId)
        {
            var activeTournaments = 
                    _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var fixtures =
                        _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                            tournament.ProviderTournamentId,
                            _rugbyService.GetCurrentProviderSeasonIdForTournament(tournament.Id),
                            cancellationToken);

                var upcomingFixtures = RemoveFixturesThatHaveBeenCompleted(fixtures);
                RugbyFixturesResponse oneMonthsfixtures = RemoveFixturesMoreThanAMonthFromNow(upcomingFixtures);

                PersistFixturesData(cancellationToken, oneMonthsfixtures);

                _mongoDbRepository.Save(fixtures);
            }
        }

        private RugbyFixturesResponse RemoveFixturesThatHaveBeenCompleted(RugbyFixturesResponse fixtures)
        {
            foreach(var round in fixtures.Fixtures.roundFixtures)
            {
                round.gameFixtures.RemoveAll(f => f.gameStateName == "Final");
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

        public async Task IngestMatchStatsForFixture(CancellationToken cancellationToken, long providerFixtureId)
        {
            while (true)
            {
                var fixtureResponse =
                        await _statsProzoneIngestService.IngestMatchStatsForFixtureAsync(cancellationToken, providerFixtureId);

                if (fixtureResponse.RugbyMatchStats.gameState == "Game End")
                    break;
                // Check if should stop looping?

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        public async Task IngestLogsForTournamentSeason(CancellationToken cancellationToken, int providerTournamentId, int seasonId)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var results = await _statsProzoneIngestService.IngestLogsForTournament(providerTournamentId, seasonId);

            // TODO: Also persist in SQL DB.
        }
    }
}
