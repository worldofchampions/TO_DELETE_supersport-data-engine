namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

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
                    if (league.ProviderSlug is null || league.MotorsportSportType == MotorsportSportType.Superbike) continue;

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
                    if (league.ProviderSlug is null || league.MotorsportSportType == MotorsportSportType.Superbike) continue;

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
                        if (race.IsDisabledInbound) continue;

                        var motorsportRaceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);

                        if (motorsportRaceEvents == null) continue;

                        foreach (var raceEvent in motorsportRaceEvents)
                        {
                            var providerResponse =
                                _statsMotorsportIngestService.IngestRaceResults(league.ProviderSlug, season.ProviderSeasonId,
                                    race.ProviderRaceId);

                            await _motorsportStorageService.PersistResultsInRepository(providerResponse, raceEvent, league);

                            await _mongoDbMotorsportRepository.Save(providerResponse);
                        }
                    }
                }
            }
        }

        public async Task IngestResultsForRaceEvent(MotorsportRaceEvent motorsportRaceEvent, int threadSleepInSeconds, int pollingDurationInMinutes)
        {
            while (true)
            {
                var league = motorsportRaceEvent.MotorsportRace.MotorsportLeague;

                var season = motorsportRaceEvent.MotorsportSeason;

                var race = motorsportRaceEvent.MotorsportRace;

                var providerResponse =
                    _statsMotorsportIngestService.IngestRaceResults(league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                await _motorsportStorageService.PersistResultsInRepository(providerResponse, motorsportRaceEvent, league);

                await _mongoDbMotorsportRepository.Save(providerResponse);

                PauseIngest(threadSleepInSeconds);

                if (await ShouldStopPolling(motorsportRaceEvent, pollingDurationInMinutes))
                {
                    break;
                }
            }
        }

        public async Task IngestRaceEventsForLeague(MotorsportRaceEvent motorsportRaceEvent, int threadSleepInSeconds, int pollingDurationInMinutes)
        {
            var league = motorsportRaceEvent.MotorsportRace.MotorsportLeague;

            if (league.ProviderSlug is null) return;

            var season = motorsportRaceEvent.MotorsportSeason;

            if (season is null) return;

            var leagueRaces = (await _motorsportService.GetRacesForLeague(league.Id))?.ToList();

            if (leagueRaces is null) return;

            while (true)
            {
                foreach (var race in leagueRaces)
                {
                    if (race.IsDisabledInbound) continue;

                    var providerResponse =
                        _statsMotorsportIngestService.IngestRaceEventsForLeague(league.ProviderSlug,
                            season.ProviderSeasonId, race.ProviderRaceId);

                    await
                        _motorsportStorageService.PersistRaceEventsInRepository(providerResponse, race, season, CancellationToken.None);

                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }

                PauseIngest(threadSleepInSeconds);

                if (await ShouldStopPolling(motorsportRaceEvent, pollingDurationInMinutes))
                {
                    break;
                }
            }
        }

        public async Task IngestRaceEventGrids(MotorsportRaceEvent motorsportRaceEvent, int ingestSleepInSeconds, int pollingDurationInMinutes)
        {
            while (true)
            {
                var league = motorsportRaceEvent.MotorsportRace.MotorsportLeague;

                var season = motorsportRaceEvent.MotorsportSeason;

                var race = motorsportRaceEvent.MotorsportRace;

                var raceGrid =
                    _statsMotorsportIngestService.IngestRaceGrid(league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                await _motorsportStorageService.PersistGridInRepository(raceGrid, motorsportRaceEvent, CancellationToken.None);

                await _mongoDbMotorsportRepository.Save(raceGrid);

                PauseIngest(ingestSleepInSeconds);

                if (await ShouldStopPolling(motorsportRaceEvent, pollingDurationInMinutes))
                {
                    break;
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

                    var motorsportSeasons = (await _motorsportService.GetHistoricSeasonsForLeague(league.Id, true)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        foreach (var race in leagueRaces)
                        {
                            if (race.IsDisabledInbound) continue;

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

                    var seasons = await _motorsportService.GetCurrentAndFutureSeasonsForLeague(league.Id);

                    if (seasons is null) continue;

                    foreach (var season in seasons)
                    {
                        var leagueRaces = await _motorsportService.GetRacesForLeague(league.Id);

                        if (leagueRaces is null) continue;

                        foreach (var race in leagueRaces)
                        {
                            if (race.IsDisabledInbound) continue;

                            var providerResponse =
                                _statsMotorsportIngestService.IngestRaceEventsForLeague(league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                            await _motorsportStorageService.PersistRaceEventsInRepository(providerResponse, race, season, cancellationToken);

                            await _mongoDbMotorsportRepository.Save(providerResponse);
                        }
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
                        if (race.IsDisabledInbound) continue;

                        var raceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);

                        if (raceEvents == null) continue;

                        foreach (var raceEvent in raceEvents)
                        {
                            var raceGrid =
                                _statsMotorsportIngestService.IngestRaceGrid(league.ProviderSlug, season.ProviderSeasonId,
                                    race.ProviderRaceId);

                            await _motorsportStorageService.PersistGridInRepository(raceGrid, raceEvent, cancellationToken);

                            await _mongoDbMotorsportRepository.Save(raceGrid);
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

                    var motorsportSeasons = (await _motorsportService.GetHistoricSeasonsForLeague(league.Id, true)).ToList();

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
                var motorsportSeasons = (await _motorsportService.GetHistoricSeasonsForLeague(league.Id, true)).ToList();

                foreach (var season in motorsportSeasons)
                {
                    var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);

                    if (motorsportRaces == null) continue;

                    foreach (var race in motorsportRaces)
                    {
                        if (race.IsDisabledInbound) continue;

                        var motorsportRaceEvents = await _motorsportService.GetEventsForRace(race.Id, season.Id);

                        if (motorsportRaceEvents == null) continue;

                        foreach (var raceEvent in motorsportRaceEvents)
                        {
                            var providerResponse =
                                _statsMotorsportIngestService.IngestRaceResults(league.ProviderSlug, season.ProviderSeasonId,
                                    race.ProviderRaceId);

                            await _motorsportStorageService.PersistResultsInRepository(providerResponse, raceEvent, league);

                            await _mongoDbMotorsportRepository.Save(providerResponse);
                        }
                    }
                }
            }
        }

        public async Task IngestLiveRaceEventData(MotorsportRaceEvent raceEvent, int pollingTimeInSeconds, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (raceEvent?.MotorsportSeason != null)
                {
                    var providerSeasonId = raceEvent.MotorsportSeason.ProviderSeasonId;

                    if (raceEvent.MotorsportRace.MotorsportLeague?.ProviderSlug != null)
                    {
                        var providerSlug = raceEvent.MotorsportRace.MotorsportLeague.ProviderSlug;

                        var providerRaceId = raceEvent.MotorsportRace.ProviderRaceId;

                        var providerResponse =
                            _statsMotorsportIngestService.IngestRaceResults(providerSlug, providerSeasonId, providerRaceId);

                        await _motorsportStorageService.PersistLiveResultsInRepository(providerResponse, raceEvent);

                        await _mongoDbMotorsportRepository.Save(providerResponse);
                    }
                }

                if (await ShouldStopLivePollingForEvent(raceEvent))
                {
                    break;
                }

                PauseIngest(pollingTimeInSeconds);
            }
        }

        public async Task IngestHistoricTeamStandings(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null || !LeagueHasTeamStandings(league)) continue;

                    var motorsportSeasons =
                        (await _motorsportService.GetHistoricSeasonsForLeague(league.Id, true)).ToList();

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

        public async Task IngestTeamStandingsForLeague(MotorsportRaceEvent raceEvent, int pollingTimeInSeconds, int pollingDurationInMinutes)
        {
            while (true)
            {
                var league = raceEvent.MotorsportRace.MotorsportLeague;

                var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, CancellationToken.None);

                var providerResponse =
                    _statsMotorsportIngestService.IngestTeamStandings(league.ProviderSlug, season.ProviderSeasonId);

                await _motorsportStorageService.PersistTeamStandingsInRepository(providerResponse, league, season, CancellationToken.None);

                await _mongoDbMotorsportRepository.Save(providerResponse);

                if (await ShouldStopPolling(raceEvent, pollingDurationInMinutes))
                {
                    break;
                }
            }
        }

        public async Task IngestDriverStandingsForLeague(MotorsportRaceEvent raceEvent, int pollingTimeInSeconds, int pollingDurationInMinutes)
        {
            while (true)
            {
                if (raceEvent?.MotorsportRace.MotorsportLeague != null)
                {
                    var league = raceEvent.MotorsportRace.MotorsportLeague;

                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, CancellationToken.None);

                    var providerResponse =
                        _statsMotorsportIngestService.IngestDriverStandings(league.ProviderSlug, season.ProviderSeasonId);

                    await _motorsportStorageService.PersistDriverStandingsInRepository(providerResponse, league, season, CancellationToken.None);

                    await _mongoDbMotorsportRepository.Save(providerResponse);
                }
                else
                {
                    break;
                }

                PauseIngest(pollingTimeInSeconds);

                if (await ShouldStopPolling(raceEvent, pollingDurationInMinutes))
                {
                    break;
                }
            }
        }

        private static bool LeagueHasTeamStandings(MotorsportLeague league)
        {
            return league.MotorsportSportType != MotorsportSportType.Superbike;
        }

        public async Task IngestHistoricDriverStandings(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            if (motorsportLeagues != null)
            {
                foreach (var league in motorsportLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var motorsportSeasons = (await _motorsportService.GetHistoricSeasonsForLeague(league.Id, true)).ToList();

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

        public async Task IngestHistoricEventsGrids(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            if (activeLeagues != null)
            {
                foreach (var league in activeLeagues)
                {
                    var motorsportSeasons = (await _motorsportService.GetHistoricSeasonsForLeague(league.Id, true)).ToList();

                    foreach (var season in motorsportSeasons)
                    {
                        var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);

                        if (motorsportRaces == null) continue;

                        foreach (var race in motorsportRaces)
                        {
                            if (race.IsDisabledInbound) continue;

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

        private static void PauseIngest(int pauseTimeInSeconds)
        {
            var sleepTimeInMilliseconds = pauseTimeInSeconds * 1000;

            Thread.Sleep(sleepTimeInMilliseconds);
        }

        private async Task<bool> ShouldStopLivePollingForEvent(MotorsportRaceEvent raceEvent)
        {
            var schedulerTrackingEvent = await _motorsportService.GetSchedulerTrackingEvent(raceEvent);

            return schedulerTrackingEvent?.EndedDateTimeUtc != null && schedulerTrackingEvent.MotorsportRaceEventStatus == MotorsportRaceEventStatus.Result;
        }

        private async Task<bool> ShouldStopPolling(MotorsportRaceEvent raceEvent, int pollingDurationInMinutes)
        {
            var schedulerTrackingEvent = await _motorsportService.GetSchedulerTrackingEvent(raceEvent);

            if (schedulerTrackingEvent.EndedDateTimeUtc == null) return true;

            var timeDiff = DateTimeOffset.UtcNow.Subtract(schedulerTrackingEvent.EndedDateTimeUtc.Value).TotalMinutes;

            return timeDiff >= pollingDurationInMinutes;
        }

    }
}