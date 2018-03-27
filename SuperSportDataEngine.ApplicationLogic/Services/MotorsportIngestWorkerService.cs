using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Common.Logging;

    public class MotorsportIngestWorkerService : IMotorsportIngestWorkerService
    {
        private readonly IStatsMotorsportIngestService _statsMotorsportIngestService;
        private readonly IMotorsportService _motorsportService;
        private readonly ILoggingService _loggingService;
        private readonly IMotorsportStorageService _motorsportStorageService;
        private readonly IMongoDbMotorsportRepository _mongoDbMotorsportRepository;

        public MotorsportIngestWorkerService(
            IStatsMotorsportIngestService statsMotorsportIngestService,
            ILoggingService loggingService,
            IMotorsportService motorsportService, 
            IMotorsportStorageService motorsportStorageService, 
            IMongoDbMotorsportRepository mongoDbMotorsportRepository)
        {
            _statsMotorsportIngestService = statsMotorsportIngestService;
            _loggingService = loggingService;
            _motorsportService = motorsportService;
            _motorsportStorageService = motorsportStorageService;
            _mongoDbMotorsportRepository = mongoDbMotorsportRepository;
        }
        
        public async Task IngestLeagues(CancellationToken cancellationToken)
        {
            var leagues = _statsMotorsportIngestService.IngestLeagues();

            await _motorsportStorageService.PersistLeaguesInRepository(leagues, cancellationToken);

            await _mongoDbMotorsportRepository.Save(leagues);
        }

        public async Task IngestSeasons(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    var providerResponse = _statsMotorsportIngestService.IngestLeagueSeason(league.ProviderSlug);

                    await _motorsportStorageService.PersistSeasonsInRepository(providerResponse, league, cancellationToken);

                    await _mongoDbMotorsportRepository.Save(providerResponse);

                }
            }
        }

        public async Task IngestDriversForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    var providerResponse =
                        _statsMotorsportIngestService.IngestDriversForLeague(league.ProviderSlug);

                    await _motorsportStorageService.PersistLeagueDriversInRepository(providerResponse, league);
                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }
            }
        }

        public async Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var providerResponse = _statsMotorsportIngestService.IngestTeamsForLeague(league.ProviderSlug);

                    await _motorsportStorageService.PersistTeamsInRepository(providerResponse, league);
                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }
            }
        }

        public async Task IngestDriverStandingsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                    if (season is null) continue;

                    var providerResponse =
                        _statsMotorsportIngestService.IngestDriverStandings(league.ProviderSlug, season.ProviderSeasonId);

                    await _motorsportStorageService.PersistDriverStandingsInRepository(providerResponse, league, season, cancellationToken);
                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }
            }
        }

        public async Task IngestTeamStandingsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorsportService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                    if (season is null) continue;

                    var providerResponse =
                        _statsMotorsportIngestService.IngestTeamStandings(league.ProviderSlug, season.ProviderSeasonId);

                    await _motorsportStorageService.PersistTeamStandingsInRepository(providerResponse, league, season, cancellationToken);
                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }
            }
        }

        public async Task IngestRacesForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                    if (season is null) continue;

                    var providerResponse =
                        _statsMotorsportIngestService.IngestLeagueRaces(league.ProviderSlug, season.ProviderSeasonId);

                    await _motorsportStorageService.PersistRacesInRepository(providerResponse, league, cancellationToken);
                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }
            }
        }

        public async Task IngestResultsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);
                    if (season is null) continue;

                    var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);
                    if (motorsportRaces == null) continue;

                    foreach (var race in motorsportRaces)
                    {
                        var motorsportRaceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);
                        if (motorsportRaceEvents == null) continue;

                        foreach (var raceEvent in motorsportRaceEvents)
                        {
                            var providerResponse =
                                _statsMotorsportIngestService.IngestRaceResults(league.ProviderSlug, season.ProviderSeasonId,
                                    race.ProviderRaceId);

                            await _motorsportStorageService.PersistResultsInRepository(providerResponse, raceEvent, cancellationToken);
                            await _mongoDbMotorsportRepository.Save(providerResponse);
                        }
                    }
                }
            }
        }

        public async Task IngestHistoricRaceEvents(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorsportService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var leagueRaces = (await _motorsportService.GetRacesForLeague(league.Id)).ToList();
                    var motorsportSeasons = (await _motorsportService.GetPastSeasonsForLeague(league.Id, cancellationToken)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        foreach (var race in leagueRaces)
                        {
                            var providerResponse =
                                _statsMotorsportIngestService.IngestRaceEventsForLeague(
                                    league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                            await _motorsportStorageService.PersistRaceEventsInRepository(providerResponse, race, season, cancellationToken);
                            await _mongoDbMotorsportRepository.Save(providerResponse);
                        }
                    }
                }
            }
        }

        public async Task IngestRacesEvents(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var leagueRaces = await _motorsportService.GetRacesForLeague(league.Id);
                    if (leagueRaces is null) continue;

                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);
                    if (season is null) continue;

                    foreach (var race in leagueRaces)
                    {
                        var providerResponse =
                            _statsMotorsportIngestService.IngestRaceEventsForLeague(league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                        await _motorsportStorageService.PersistRaceEventsInRepository(providerResponse, race, season, cancellationToken);
                        await _mongoDbMotorsportRepository.Save(providerResponse);
                    }
                }
            }
        }

        public async Task IngestRacesEventsGrids(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);
                    if (season == null) continue;

                    var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);
                    if (motorsportRaces == null) continue;

                    foreach (var race in motorsportRaces)
                    {
                        var raceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);
                        if (raceEvents == null) continue;

                        foreach (var raceEvent in raceEvents)
                        {
                            var raceResults =
                                _statsMotorsportIngestService.IngestRaceGrid(league.ProviderSlug, season.ProviderSeasonId,
                                    race.ProviderRaceId);

                            await _motorsportStorageService.PersistGridInRepository(raceResults, raceEvent, cancellationToken);
                            await _mongoDbMotorsportRepository.Save(raceResults);
                        }
                    }
                }
            }
        }

        public async Task IngestHistoricRaces(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var motorsportSeasons = (await _motorsportService.GetPastSeasonsForLeague(league.Id, cancellationToken)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        var providerResponse =
                            _statsMotorsportIngestService.IngestLeagueRaces(league.ProviderSlug, season.ProviderSeasonId);

                        await _motorsportStorageService.PersistRacesInRepository(providerResponse, league, cancellationToken);
                        await _mongoDbMotorsportRepository.Save(providerResponse);
                    }
                }
            }
        }

        public async Task IngestHistoricEventsResults(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in motorsportLeagues)
            {
                var motorsportSeasons = (await _motorsportService.GetPastSeasonsForLeague(league.Id, cancellationToken)).ToList();
                foreach (var season in motorsportSeasons)
                {
                    var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);
                    if (motorsportRaces == null) continue;

                    foreach (var race in motorsportRaces)
                    {
                        var motorsportRaceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);
                        if (motorsportRaceEvents == null) continue;

                        foreach (var raceEvent in motorsportRaceEvents)
                        {
                            var providerResponse =
                                _statsMotorsportIngestService.IngestRaceResults(league.ProviderSlug, season.ProviderSeasonId,
                                    race.ProviderRaceId);

                            await _motorsportStorageService.PersistResultsInRepository(providerResponse, raceEvent, cancellationToken);
                            await _mongoDbMotorsportRepository.Save(providerResponse);
                        }
                    } 
                }
            }
        }

        public async Task IngestHistoricTeamStandings(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var motorsportSeasons = 
                        (await _motorsportService.GetPastSeasonsForLeague(league.Id, cancellationToken)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        var providerResponse =
                            _statsMotorsportIngestService.IngestTeamStandings(league.ProviderSlug, season.ProviderSeasonId);

                        await _motorsportStorageService.PersistTeamStandingsInRepository(providerResponse, league, season, cancellationToken); 
                        await _mongoDbMotorsportRepository.Save(providerResponse);
                    }
                }
            }
        }

        public async Task IngestHistoricDriverStandings(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var motorsportSeasons = (await _motorsportService.GetPastSeasonsForLeague(league.Id, cancellationToken)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        var providerResponse =
                            _statsMotorsportIngestService.IngestDriverStandings(league.ProviderSlug, season.ProviderSeasonId);

                        await _motorsportStorageService.PersistDriverStandingsInRepository(providerResponse, league, season, cancellationToken); 
                        await _mongoDbMotorsportRepository.Save(providerResponse);
                    }
                }
            }
        }

        public async Task IngestLiveRaceEventData(MotorsportRace race, CancellationToken cancellationToken)
        {
            //TODO
            await Task.FromResult(0);
        }

        public async Task IngestHistoricEventsGrids(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            if (activeLeagues != null)
            {
                foreach (var league in activeLeagues)
                {
                    var motorsportSeasons = (await _motorsportService.GetPastSeasonsForLeague(league.Id, cancellationToken)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);
                        if (motorsportRaces == null) continue;

                        foreach (var race in motorsportRaces)
                        {
                            var motorsportRaceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);
                            if (motorsportRaceEvents == null) continue;

                            foreach (var raceEvent in motorsportRaceEvents)
                            {
                                var providerResponse =
                                    _statsMotorsportIngestService.IngestRaceGrid(league.ProviderSlug, season.ProviderSeasonId,
                                        race.ProviderRaceId);

                                await _motorsportStorageService.PersistGridInRepository(providerResponse, raceEvent, cancellationToken);
                                await _mongoDbMotorsportRepository.Save(providerResponse);
                            }
                        } 
                    }
                }
            }
        }
    }
}