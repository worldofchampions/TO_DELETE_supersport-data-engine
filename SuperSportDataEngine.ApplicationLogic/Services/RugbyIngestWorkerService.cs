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

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyVenue> _rugbyVenueRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTeam> _rugbyTeamRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixturerRepository;
        private readonly IRugbyService _rugbyService;

        public RugbyIngestWorkerService(
            IStatsProzoneRugbyIngestService statsProzoneIngestService,
            IMongoDbRugbyRepository mongoDbRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository,
            IBaseEntityFrameworkRepository<RugbyVenue> rugbyVenueRepository,
            IBaseEntityFrameworkRepository<RugbyTeam> rugbyTeamRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixturerRepository,
            IRugbyService rugbyService)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
            _rugbyVenueRepository = rugbyVenueRepository;
            _rugbyTeamRepository = rugbyTeamRepository;
            _rugbyFixturerRepository = rugbyFixturerRepository;
            _rugbyService = rugbyService;
        }

        public async Task IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var entitiesResponse =
                _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            await PersistVenuesInPublicSportsDataRepository(cancellationToken, entitiesResponse);
            await PersistTeamsInPublicSportsDataRepository(cancellationToken, entitiesResponse);
            await PersistRugbyTournamentsInRepositoryAsync(entitiesResponse, cancellationToken);
            await IngestRugbyTournamentSeasons(cancellationToken);

            _mongoDbRepository.Save(entitiesResponse);
        }

        private async Task PersistTeamsInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
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
                        Abbreviation = team.TeamAbbrev

                    };
                    _rugbyTeamRepository.Add(newTeam);
                }
                else
                {
                    teamInDb.Name = team.name;
                    teamInDb.ProviderTeamId = team.id;
                }
            }

            await _rugbyTeamRepository.SaveAsync();
        }

        private async Task PersistVenuesInPublicSportsDataRepository(CancellationToken cancellationToken, RugbyEntitiesResponse entitiesResponse)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach(var venue in entitiesResponse.Entities.venues)
            {
                // Lookup in db
                var venueInDb = _rugbyVenueRepository.Where(v => v.ProviderVenueId == venue.id).FirstOrDefault();

                if (venueInDb == null)
                {
                    var newVenue = new RugbyVenue()
                    {
                        ProviderVenueId = venue.id,
                        Name = venue.name
                    };
                    _rugbyVenueRepository.Add(newVenue);
                }
                else
                {
                    venueInDb.Name = venue.name;
                    venueInDb.ProviderVenueId = venue.id;
                }

                await _rugbyVenueRepository.SaveAsync();
            }
        }

        private async Task IngestRugbyTournamentSeasons(CancellationToken cancellationToken)
        {
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled);
            foreach (var tournament in activeTournaments)
            {
                var season = _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, DateTime.Now.Year);

                await PersistRugbySeasonDataToSystemSportsDataRepository(cancellationToken, season);
            }
        }

        private async Task PersistRugbySeasonDataToSystemSportsDataRepository(CancellationToken cancellationToken, RugbySeasonResponse season)
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
                Id = seasonEntry != null ? seasonEntry.Id : Guid.NewGuid(),
                ProviderSeasonId = providerSeasonId,
                RugbyTournament = tour,
                IsCurrent = isSeasonCurrentlyActive,
                Name = season.RugbySeasons.season.First().name
            };

            // Not in repo?
            if (seasonEntry == null)
            {
                _rugbySeasonRepository.Add(newEntry);
            }
            else
            {
                _rugbySeasonRepository.Update(newEntry);
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _rugbySeasonRepository.SaveAsync();
            }
            catch (Exception e)
            {
                throw e;
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

                // TODO: Also persist in SQL DB.
                await PersistRugbyFixturesToPublicSportsRepository(cancellationToken, fixtures);
                await PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(fixtures);

                _mongoDbRepository.Save(fixtures);
            }
        }

        private async Task PersistRugbyFixturesToPublicSportsRepository(CancellationToken cancellationToken, RugbyFixturesResponse fixtures)
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
                    var fixtureInDb = _rugbyFixturerRepository.Where(f => f.ProviderFixtureId == fixtureId).FirstOrDefault();
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
                        HomeTeam = teamA,
                        AwayTeam = teamB
                    };

                    if (fixtureInDb == null)
                    {
                        _rugbyFixturerRepository.Add(newFixture);
                    }
                    else
                    {
                        _rugbyFixturerRepository.Update(newFixture);
                    }
                }
            }

            await _rugbyFixturerRepository.SaveAsync();
        }

        private async Task PersistRugbySeasonDataInSchedulerTrackingRugbySeasonTable(RugbyFixturesResponse fixtures)
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
            }

            try
            {
                await _schedulerTrackingRugbySeasonRepository.SaveAsync();
            }
            catch (Exception e)
            {
                throw e;
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

        private async Task PersistRugbyTournamentsInRepositoryAsync(RugbyEntitiesResponse entitiesResponse, CancellationToken cancellationToken)
        {
            foreach (var competition in entitiesResponse.Entities.competitions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var entry = _rugbyTournamentRepository
                    .Where(c => c.ProviderTournamentId == competition.id)
                    .FirstOrDefault();

                var newEntry = new RugbyTournament
                {
                    Id = entry != null ? entry.Id : Guid.NewGuid(),
                    ProviderTournamentId = competition.id,
                    Name = competition.name,
                    IsEnabled = entry != null,
                    LogoUrl = competition.CompetitionLogoURL,
                    Abbreviation = competition.CompetitionAbbrev,
                    Slug = "/competition/" + competition.id,
                    LegacyTournamentId = competition.id
                };

                if (entry == null)
                {
                    _rugbyTournamentRepository.Add(newEntry);
                }
                else
                {
                    _rugbyTournamentRepository.Update(newEntry);
                }
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _rugbyTournamentRepository.SaveAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task IngestRugbyResultsForAllFixtures(CancellationToken cancellationToken)
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
            if (cancellationToken.IsCancellationRequested)
                return;

            var fixtures =
                _statsProzoneIngestService.IngestFixturesForTournamentSeason(
                    tournamentId, seasonId, cancellationToken);

            // TODO: Also persist in SQL DB.
        }

        public async Task IngestRugbyResultsForCurrentDayFixtures(CancellationToken cancellationToken)
        {
            var currentRoundFixtures = GetCurrentDayRoundFixturesForActiveTournaments();
            
            foreach (var round in currentRoundFixtures)
            {
                var results = await _statsProzoneIngestService.IngestFixtureResults(round.Item1, round.Item2, round.Item3);

                // TODO: Also persist in SQL DB.
            }
        }

        public async Task IngestRugbyResultsForFixturesInResultsState(CancellationToken cancellationToken)
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
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled);
            foreach (var tournament in activeTournaments)
            {
                var season = _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, DateTime.Now.Year);

                // Process and persist one months data asynchronously.
            }

            return;
        }
    }
}
