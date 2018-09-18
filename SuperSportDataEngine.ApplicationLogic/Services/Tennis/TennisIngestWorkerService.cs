using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.ApplicationLogic.Services.Tennis
{
    public class TennisIngestWorkerService : ITennisIngestWorkerService
    {
        private readonly IStatsTennisIngestService _statsIngestService;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly ITennisStorageService _storageService;
        private readonly ILoggingService _loggingService;
        private readonly IMongoDbTennisRepository _mongoStorageService;
        private readonly int _postLivePollingDurationInMinutes;

        public TennisIngestWorkerService(
            IStatsTennisIngestService statsIngestService,
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            ITennisStorageService storageService,
            ILoggingService loggingService, 
            IMongoDbTennisRepository mongoStorageService)
        {
            _statsIngestService = statsIngestService;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _storageService = storageService;
            _loggingService = loggingService;
            _mongoStorageService = mongoStorageService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;

            _postLivePollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["TennisPostLivePollingDurationInMinutes"] ?? "60");
        }

        public async Task IngestReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await QueryAndSaveLeaguesInPublicSportsData(cancellationToken);
            await QueryAndSaveSeasonsInPublicSportsData(cancellationToken);
            await QueryAndSaveTournamentsInPublicSportsData(cancellationToken);
            await QueryAndSaveTournamentTypesInPublicSportsData(cancellationToken);
            await QueryAndSaveSurfaceTypesInPublicSportsData(cancellationToken);
            await QueryAndSaveVenuesInPublicSportsData(cancellationToken);
            await QueryAndSaveParticipantsInPublicSportsData(cancellationToken);
        }

        public async Task IngestRankingsForEnabledLeagues_Helper(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await IngestRankingsForEnabledLeagues_Helper(_statsIngestService.GetRankRankings, cancellationToken);
        }

        public async Task IngestRaceRankingsForEnabledLeagues(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await IngestRankingsForEnabledLeagues_Helper(_statsIngestService.GetRaceRankings, cancellationToken);
        }

        public async Task IngestRankingsForLeague(string providerLeagueSlug, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await IngestRankingsForLeague(providerLeagueSlug, _statsIngestService.GetRankRankings, cancellationToken);
        }

        public async Task IngestRaceRankingsForLeague(string providerLeagueSlug, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await IngestRankingsForLeague(providerLeagueSlug, _statsIngestService.GetRaceRankings, cancellationToken);
        }

        public async Task IngestHistoricRankingsForEnabledLeagues(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await IngestHistoricRankingsForEnabledLeagues(_statsIngestService.GetRankRankings, cancellationToken);
        }

        public async Task IngestHistoricRaceRankingsForEnabledLeagues(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await IngestHistoricRankingsForEnabledLeagues(_statsIngestService.GetRaceRankings, cancellationToken);
        }

        public async Task IngestResultsForEnabledLeagues(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var enabledLeagues = _storageService.GetEnabledLeagues();
            foreach(var enabledLeague in enabledLeagues)
            {
                var currentSeason = _storageService.GetCurrentTennisSeasonForLeague(enabledLeague.ProviderSlug);

                var currentEventsForEnabledLeague = 
                      _storageService.GetEventsForLeague(
                          enabledLeague.ProviderLeagueId, currentSeason.ProviderSeasonId);

                foreach(var tennisEvent in currentEventsForEnabledLeague)
                {
                    var resultsForEvent = GetResultsForEvent(enabledLeague, currentSeason, tennisEvent);
                    var providerEventTypes = resultsForEvent?.apiResults?.FirstOrDefault()?.league?.subLeague?.season?.eventType?.FirstOrDefault()?.events;

                    var tennisEventTennisLeagueMapping =
                          _storageService.GetMappingForTennisEventTennisLeague(
                              tennisEvent.ProviderEventId,
                              enabledLeague.ProviderLeagueId);

                    if (tennisEventTennisLeagueMapping == null)
                        continue;

                    if (providerEventTypes == null)
                        continue;

                    foreach (var eventType in providerEventTypes)
                    {
                        var singlesMatches =
                                eventType.matchTypes?.First(eType =>
                                    eType.matchTypeId == 1 || eType.matchTypeId == 3);

                        if (singlesMatches == null)
                            continue;

                        await _storageService.AddOrUpdateTennisEventSeeds(singlesMatches.seeds, tennisEvent);

                        var recentTennisMatches = singlesMatches.matches.Where(IsTennisMatchWithinLastWeek).ToList();
                        await _storageService.AddOrUpdateTennisMatch(recentTennisMatches, singlesMatches, tennisEventTennisLeagueMapping);
                    }

                    await _mongoStorageService.Save(resultsForEvent);
                }
            }
        }

        private TennisResultsResponse GetResultsForEvent(TennisLeague enabledLeague, TennisSeason currentSeason, TennisEvent tennisEvent)
        {
            return _statsIngestService.GetResultsForEvent(enabledLeague.ProviderSlug, tennisEvent.ProviderEventId, currentSeason.ProviderSeasonId);
        }

        public async Task IngestResultsForEvent(string providerLeagueSlug, int providerEventId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var league = _storageService.GetTennisLeagueFor(providerLeagueSlug);
            var season = _storageService.GetCurrentTennisSeasonForLeague(providerLeagueSlug);
            var tennisEvent = _storageService.GetTennisEventFor(providerEventId);

            if (tennisEvent == null)
                return;

            if (season == null)
                return;

            var results = GetResultsForEvent(league, season, tennisEvent);
            var eventTypes = results?.apiResults?.FirstOrDefault()?.league?.subLeague?.season?.eventType?.FirstOrDefault()?.events;

            var tennisEventTennisLeagueMapping =
                    _storageService.GetMappingForTennisEventTennisLeague(
                        providerEventId,
                        league.ProviderLeagueId);

            if (tennisEventTennisLeagueMapping == null)
                return;

            if (eventTypes == null)
                return;

            foreach (var eventType in eventTypes)
            {
                var singlesMatchTypes =
                        eventType.matchTypes?.First(eType =>
                            eType.matchTypeId == 1 || eType.matchTypeId == 3);

                if (singlesMatchTypes == null)
                    continue;

                await _storageService.AddOrUpdateTennisEventSeeds(singlesMatchTypes.seeds, tennisEvent);
                await _storageService.AddOrUpdateTennisMatch(singlesMatchTypes.matches, singlesMatchTypes, tennisEventTennisLeagueMapping);
            }

            await _mongoStorageService.Save(results);
        }

        public async Task IngestResultsForMatch(string providerLeagueSlug, int providerEventId, int providerMatchId, CancellationToken cancellationToken)
        {
            var tennisLeague = _storageService.GetTennisLeagueFor(providerLeagueSlug);
            var tennisSeason = _storageService.GetCurrentTennisSeasonForLeague(providerLeagueSlug);
            var tennisEvent = _storageService.GetTennisEventFor(providerEventId);
            var tennisMatch = _storageService.GetTennisMatchFor(providerMatchId);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (tennisEvent == null)
                    return;

                if (tennisSeason == null)
                    return;

                var results = GetResultsForTennisMatch(tennisLeague, tennisSeason, tennisEvent, tennisMatch);
                var eventTypes = results?.apiResults?.FirstOrDefault()?.league?.subLeague?.season?.eventType?.FirstOrDefault()?.events;

                var mapping =
                    _storageService.GetMappingForTennisEventTennisLeague(
                        providerEventId,
                        tennisLeague.ProviderLeagueId);

                if (mapping == null)
                    return;

                if (eventTypes == null)
                    return;

                var matchStatus = SchedulerStateForTennisMatchPolling.LivePolling;

                foreach (var eventType in eventTypes)
                {
                    var matchType =
                        eventType.matchTypes?.First(eType =>
                            eType.matchTypeId == 1 || eType.matchTypeId == 3);

                    if (matchType == null)
                        continue;

                    await _storageService.AddOrUpdateTennisEventSeeds(matchType.seeds, tennisEvent);
                    var matches = matchType.matches.Where(m => m.matchId == providerMatchId).ToList();
                    await _storageService.AddOrUpdateTennisMatch(matches, matchType, mapping);

                    var thisMatch = matches.FirstOrDefault(m => m.matchId == providerMatchId);
                    matchStatus = await UpdateMatchTrackingTable(thisMatch, tennisMatch);
                }

                await _mongoStorageService.Save(results);

                switch (matchStatus)
                {
                    case SchedulerStateForTennisMatchPolling.LivePolling:
                        Thread.Sleep(TimeSpan.FromSeconds(30));
                        break;
                    case SchedulerStateForTennisMatchPolling.PostLivePolling:
                        Thread.Sleep(TimeSpan.FromSeconds(60));
                        break;
                    case SchedulerStateForTennisMatchPolling.PollingComplete:
                        return;
                    case SchedulerStateForTennisMatchPolling.NotStarted:
                        break;
                }
            }
        }

        private TennisResultsResponse GetResultsForTennisMatch(TennisLeague tennisLeague, TennisSeason tennisSeason, TennisEvent tennisEvent, TennisMatch tennisMatch)
        {
            return _statsIngestService.GetResultsForMatch(tennisLeague.ProviderSlug, tennisEvent.ProviderEventId, tennisMatch.ProviderMatchId, tennisSeason.ProviderSeasonId);
        }

        private async Task<SchedulerStateForTennisMatchPolling> UpdateMatchTrackingTable(Match thisMatch, TennisMatch dbMatch)
        {
            var matchStatus = SchedulerStateForTennisMatchPolling.LivePolling;

            if (thisMatch == null) return await Task.FromResult(matchStatus);

            var status = _storageService.GetMatchStateFromProviderState(thisMatch.matchStatus.matchStatusId);
            var matchDate = thisMatch.matchTime.First(d => d.dateType.ToLower().Equals("utc"));
            var startTimeUtc = new DateTimeOffset(matchDate.full, TimeSpan.FromHours(0));
            var now = DateTimeOffset.UtcNow;
            var timeDifference = now - startTimeUtc;

            if (status != TennisMatchStatus.Suspended && status != TennisMatchStatus.Final)
                return await Task.FromResult(matchStatus);

            var trackingEvent =
                (await _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.AllAsync()).FirstOrDefault(
                    t => t.TennisMatchId == dbMatch?.Id);

            if (timeDifference > TimeSpan.FromMinutes(_postLivePollingDurationInMinutes))
            {
                if (trackingEvent != null)
                    trackingEvent.SchedulerStateForTennisMatchPolling =
                        SchedulerStateForTennisMatchPolling.PollingComplete;

                matchStatus = SchedulerStateForTennisMatchPolling.PollingComplete;
            }
            else
            {
                if (trackingEvent != null)
                    trackingEvent.SchedulerStateForTennisMatchPolling =
                        SchedulerStateForTennisMatchPolling.PostLivePolling;

                matchStatus = SchedulerStateForTennisMatchPolling.PostLivePolling;
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();

            return await Task.FromResult(matchStatus);
        }

        private static bool IsTennisMatchWithinLastWeek(Match match)
        {
            var date = match.matchTime.FirstOrDefault(d => d.dateType.ToLower().Equals("utc"));
            if (date == null)
                return false;

            var dateTime = new DateTime(date.year, date.month, date.date);

            var now = DateTime.UtcNow;
            return dateTime > now - TimeSpan.FromDays(7) && dateTime < now;
        }

        public async Task IngestHistoricResults(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var leagues = _storageService.GetEnabledLeagues();
            foreach (var league in leagues)
            {
                IList<TennisSeason> pastSeasonsForLeague =
                    _publicSportDataUnitOfWork.TennisSeasons.Where(s =>
                        s.ProviderSeasonId <= DateTime.UtcNow.Year &&
                        s.TennisLeague.ProviderSlug == league.ProviderSlug).ToList();

                foreach (var season in pastSeasonsForLeague)
                {
                    var eventsForLeague =
                        _storageService.GetEventsForLeague(
                            league.ProviderLeagueId, season.ProviderSeasonId);

                    foreach (var @event in eventsForLeague)
                    {
                        var results = GetResultsForEvent(league, season, @event);
                        var eventTypes = results?.apiResults?.FirstOrDefault()?.league?.subLeague?.season?.eventType?.FirstOrDefault()?.events;

                        var mapping =
                            _storageService.GetMappingForTennisEventTennisLeague(
                                @event.ProviderEventId,
                                league.ProviderLeagueId);

                        if (mapping == null)
                            continue;

                        if (eventTypes == null)
                            continue;

                        foreach (var eventType in eventTypes)
                        {
                            var matchType =
                                eventType.matchTypes?.First(eType =>
                                    eType.matchTypeId == 1 ||
                                    eType.matchTypeId == 3);

                            if (matchType == null)
                                continue;

                            await _storageService.AddOrUpdateTennisEventSeeds(matchType.seeds, @event);

                            await _storageService.AddOrUpdateTennisMatch(matchType.matches, matchType, mapping);
                        }

                        await _mongoStorageService.Save(results);
                    }
                }
            }
        }

        private async Task QueryAndSaveTournamentTypesInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await _storageService.SetTennisTournamentTypes();
        }

        private async Task QueryAndSaveParticipantsInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var dbLeagues = _storageService.GetEnabledLeagues();

            foreach (var league in dbLeagues)
            {
                var participantsForLeague = 
                        _statsIngestService.GetParticipants(
                            league.ProviderSlug);

                if (participantsForLeague == null)
                    return;

                var participants = participantsForLeague.apiResults.First().league.subLeague.players;

                await _storageService.AddOrUpdateTennisParticipants(participants);
                await _mongoStorageService.Save(participantsForLeague);
            }
        }

        private async Task QueryAndSaveSurfaceTypesInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var dbLeagues = _storageService.GetEnabledLeagues();

            foreach (var league in dbLeagues)
            {
                var surfaceTypesFromProvider = 
                        _statsIngestService.GetSurfaceTypes(
                            league.ProviderSlug);

                await _storageService.AddOrUpdateSurfaceTypesFromProvider(surfaceTypesFromProvider);
                await _mongoStorageService.Save(surfaceTypesFromProvider);
            }
        }

        private async Task QueryAndSaveVenuesInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var dbLeagues = _storageService.GetEnabledLeagues();

            foreach (var league in dbLeagues)
            {
                var venuesFromProvider = _statsIngestService.GetVenuesForLeague(
                    league.ProviderSlug);

                await _storageService.AddOrUpdateTennisVenuesFromProvider(venuesFromProvider);
                await _mongoStorageService.Save(venuesFromProvider);
            }
        }

        public async Task IngestCalendarsForEnabledLeagues(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            IList<TennisLeague> dbLeagues = _storageService.GetEnabledLeagues().ToList();

            foreach (var league in dbLeagues)
            {
                var currentSeason = _storageService.GetCurrentTennisSeasonForLeague(league.ProviderSlug);

                var currentSeasonId = currentSeason.ProviderSeasonId;
                await IngestCalendarForSeason(league, currentSeasonId);

                var nextSeason = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s =>
                    s.TennisLeague.ProviderSlug == league.ProviderSlug && s.ProviderSeasonId == currentSeasonId + 1);

                if (nextSeason == null)
                    continue;

                await IngestCalendarForSeason(league, nextSeason.ProviderSeasonId);
            }
        }

        public async Task IngestCalendarForLeague(int providerLeagueId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var league = _storageService.GetTennisLeagueFor(providerLeagueId);

            var season = _storageService.GetCurrentTennisSeasonForLeague(league.ProviderSlug);

            var currentSeasonId = season.ProviderSeasonId;
            await IngestCalendarForSeason(league, currentSeasonId);

            var nextSeason = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s =>
                s.TennisLeague.ProviderSlug == league.ProviderSlug && s.ProviderSeasonId == currentSeasonId + 1);

            if (nextSeason == null)
                return;

            await IngestCalendarForSeason(league, nextSeason.ProviderSeasonId);
        }

        private async Task IngestCalendarForSeason(TennisLeague tennisLeague, int seasonProviderSeasonId)
        {
            var eventsForLeague = _statsIngestService.GetLeagueEvents(
                tennisLeague.ProviderSlug, seasonProviderSeasonId);

            await _storageService.AddOrUpdateEventsFromProvider(eventsForLeague, tennisLeague);
        }

        public async Task IngestHistoricCalendarsForEnabledLeagues(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            IList<TennisLeague> dbLeagues = _storageService.GetEnabledLeagues().ToList();

            foreach (var league in dbLeagues)
            {
                IList<TennisSeason> pastSeasonsForLeague =
                    _publicSportDataUnitOfWork.TennisSeasons.Where(s => 
                        s.ProviderSeasonId <= DateTime.UtcNow.Year && 
                        s.TennisLeague.ProviderSlug == league.ProviderSlug).ToList();

                foreach (var season in pastSeasonsForLeague)
                {
                    var eventsForLeague = _statsIngestService.GetLeagueEvents(
                        league.ProviderSlug, season.ProviderSeasonId);

                    await _storageService.AddOrUpdateEventsFromProvider(eventsForLeague, league);
                }
            }
        }

        private async Task QueryAndSaveSeasonsInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var enabledLeagues = _storageService.GetEnabledLeagues();

            foreach (var enabledLeague in enabledLeagues)
            {
                var seasonsResponse =
                    _statsIngestService.GetSeasonForLeague(enabledLeague.ProviderSlug);

                await _storageService.AddOrUpdateSeasonsFromProvider(seasonsResponse, enabledLeague);
                await _mongoStorageService.Save(seasonsResponse);
            }
        }

        private async Task QueryAndSaveLeaguesInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var tournamentResponse =
                _statsIngestService.GetLeagues();

            var leagues = tournamentResponse?.apiResults.FirstOrDefault();

            if (leagues == null)
                return;

            await _storageService.AddOrUpdateTennisLeaguesFromProvider(leagues);

            await _mongoStorageService.Save(tournamentResponse);
        }

        private async Task QueryAndSaveTournamentsInPublicSportsData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var leagues = (await _publicSportDataUnitOfWork.TennisLeagues.AllAsync()).ToList();
            
            foreach (var league in leagues)
            {
                var apiResponse = _statsIngestService.GetTournamentsForLeague(league.ProviderSlug);

                if (apiResponse == null)
                    continue;

                var tournaments = apiResponse.apiResults?.FirstOrDefault()?.league.tournaments;
                var providerLeague = apiResponse.apiResults?.FirstOrDefault()?.league;

                if (providerLeague == null)
                    continue;

                if (tournaments == null) continue;

                await _storageService.AddOrUpdateTennisTournamentsFromProvider(tournaments, providerLeague, league);
                await _mongoStorageService.Save(apiResponse);
            }
        }

        private async Task IngestRankingsForEnabledLeagues_Helper(Func<string, int, TennisRankingsResponse> providerRankingsFunc, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var enabledLeagues = _storageService.GetEnabledLeagues();
            
            foreach (var league in enabledLeagues)
            {
                var currentSeason = _storageService.GetCurrentTennisSeasonForLeague(league.ProviderSlug);

                var apiResponse = providerRankingsFunc(league.ProviderSlug, currentSeason.ProviderSeasonId);
                var categoryId = apiResponse?.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory?.First()?.leaderCategoryId ?? 0;

                if (categoryId == 0)
                    return;

                var leaderboard = apiResponse?.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory?.First()?.ranking;

                if (leaderboard == null)
                    return;

                var dateThrough = apiResponse.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory
                    ?.First()?.dateThrough;

                if (dateThrough == null)
                    continue;

                var dateThroughDateTime = new DateTimeOffset(new DateTime(dateThrough.year, dateThrough.month, dateThrough.date));

                var seasonId = apiResponse.apiResults?.First()?.league?.subLeague?.seasons?.First()?.season;
                var dbSeason = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s => s.ProviderSeasonId == seasonId && s.TennisLeague.ProviderLeagueId == league.ProviderLeagueId);

                await _storageService.AddOrUpdateRanking((TennisRankingType)categoryId, leaderboard, league, dbSeason, dateThroughDateTime);
                await _mongoStorageService.Save(apiResponse);
            }
        }

        private async Task IngestRankingsForLeague(string providerLeagueSlug, Func<string, int, TennisRankingsResponse> providerRankingsFunc, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var league = _storageService.GetEnabledLeagues().FirstOrDefault(l => l.ProviderSlug == providerLeagueSlug);
            if (league == null)
                return;

            var season = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s =>
                s.IsCurrent && s.TennisLeague.ProviderSlug == league.ProviderSlug);

            var apiResponse = providerRankingsFunc(league.ProviderSlug, season.ProviderSeasonId);
            var categoryId = apiResponse?.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory?.First()?.leaderCategoryId ?? 0;

            if (categoryId == 0)
                return;

            var leaderboard = apiResponse?.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory?.First()?.ranking;

            if (leaderboard == null)
                return;

            var dateThrough = apiResponse.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory
                ?.FirstOrDefault()?.dateThrough;

            if (dateThrough == null)
                return;

            var dateThroughDateTime = new DateTimeOffset(new DateTime(dateThrough.year, dateThrough.month, dateThrough.date));

            var seasonId = apiResponse.apiResults?.First()?.league?.subLeague?.seasons?.First()?.season;
            var dbSeason = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s => s.ProviderSeasonId == seasonId && s.TennisLeague.ProviderLeagueId == league.ProviderLeagueId);

            await _storageService.AddOrUpdateRanking((TennisRankingType)categoryId, leaderboard, league, dbSeason, dateThroughDateTime);
            await _mongoStorageService.Save(apiResponse);
        }

        private async Task IngestHistoricRankingsForEnabledLeagues(Func<string, int, TennisRankingsResponse> providerRankingsFunc, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var leagues = _storageService.GetEnabledLeagues();

            foreach (var league in leagues)
            {
                IList<TennisSeason> pastSeasons = _publicSportDataUnitOfWork.TennisSeasons.Where(s =>
                    s.ProviderSeasonId < DateTime.UtcNow.Year && 
                    s.TennisLeague.ProviderSlug == league.ProviderSlug).ToList();

                foreach (var season in pastSeasons)
                {
                    var apiResponse = providerRankingsFunc(league.ProviderSlug, season.ProviderSeasonId);
                    var categoryId = apiResponse?.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory?.First()?.leaderCategoryId ?? 0;

                    if (categoryId == 0)
                        return;

                    var dateThrough = apiResponse?.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory
                        ?.First()?.dateThrough;

                    if (dateThrough == null)
                        continue;

                    var dateThroughDateTime = new DateTimeOffset(new DateTime(dateThrough.year, dateThrough.month, dateThrough.date));

                    var leaderboard = apiResponse.apiResults?.First()?.league?.subLeague?.seasons?.First()?.leaderCategory?.First()?.ranking;

                    if (leaderboard == null)
                        return;

                    var seasonId = apiResponse.apiResults?.First()?.league?.subLeague?.seasons?.First()?.season;
                    var dbSeason = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s => s.ProviderSeasonId == seasonId && s.TennisLeague.ProviderLeagueId == league.ProviderLeagueId);

                    await _storageService.AddOrUpdateRanking((TennisRankingType)categoryId, leaderboard, league, dbSeason, dateThroughDateTime);
                    await _mongoStorageService.Save(apiResponse);
                }
            }
        }
    }
}
