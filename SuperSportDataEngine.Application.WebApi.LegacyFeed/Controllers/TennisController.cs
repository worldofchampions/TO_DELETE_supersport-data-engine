using System;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
using SuperSportDataEngine.Common.Interfaces;
using SuperSportDataEngine.Common.Logging;
using static AutoMapper.Mapper;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    /// <summary>
    /// LegacyFeed Tennis Endpoints
    /// </summary>
    [LegacyExceptionFilter]
    [RoutePrefix("tennis")]
    public class TennisController : ApiController
    {
        private readonly ITennisLegacyFeedService _tennisLegacyFeedService;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;

        private const string CacheKeyNamespacePrefixForFeed = "LegacyFeed:Tennis:";

        public TennisController(
            ITennisLegacyFeedService tennisLegacyFeedService,
            ICache cache, 
            ILoggingService logger)
        {
            _tennisLegacyFeedService = tennisLegacyFeedService;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint 1.1: http://{host}/tennis/{category}/schedule
        /// Endpoint 1.2: http://{host}/tennis/{category}/schedule?current=true
        /// Endpoint 1.2: http://{host}/tennis/{category}/schedule?current=false
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/schedule/{current?}")]
        public async Task<IEnumerable<TennisTournament>> GetSchedule(string category, string current = null)
        {
            var currentValue = bool.Parse(current ?? "false");

            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/schedule?current=" + currentValue;

            var scheduleFromCache = await GetFromCacheAsync<IEnumerable<TennisTournament>>(key);
            if (scheduleFromCache != null)
                return scheduleFromCache;

            var scheduleFromService =
                Map<List<TennisTournament>>(
                    (await _tennisLegacyFeedService.GetSchedules(category, currentValue))) 
                ?? new List<TennisTournament>();

            PersistToCache(key, scheduleFromService);

            return scheduleFromService;
        }

        /// <summary>
        /// Endpoint 2.1: http://{host}/tennis/{category}/rankings
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/rankings")]
        public async Task<IEnumerable<TennisRankings>> GetRankings(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/rankings";

            var rankingsFromCache = await GetFromCacheAsync<IEnumerable<TennisRankings>>(key);

            if (rankingsFromCache != null)
                return rankingsFromCache;

            var rankingsFromService =
                Map<List<TennisRankings>>(
                    await _tennisLegacyFeedService.GetRankings(category))
                ?? new List<TennisRankings>();

            PersistToCache(key, rankingsFromService);

            return rankingsFromService;
        }

        /// <summary>
        /// Endpoint 2.2: http://{host}/tennis/{category}/race
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/race")]
        public async Task<IEnumerable<TennisRankings>> GetRaceRankings(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/race";

            var rankingsFromCache = await GetFromCacheAsync<IEnumerable<TennisRankings>>(key);

            if (rankingsFromCache != null)
                return rankingsFromCache;

            var rankingsFromService =
                Map<List<TennisRankings>>(
                    await _tennisLegacyFeedService.GetRaceRankings(category))
                ?? new List<TennisRankings>();

            PersistToCache(key, rankingsFromService);

            return rankingsFromService;
        }

        /// <summary>
        /// Endpoint 3.1: http://{host}/tennis/live
        /// </summary>
        [HttpGet, HttpHead]
        [Route("live")]
        public async Task<IEnumerable<TennisTournament>> GetLive()
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/live";

            var scheduleFromCache = await GetFromCacheAsync<IEnumerable<TennisTournament>>(key);
            if (scheduleFromCache != null)
                return scheduleFromCache;

            var scheduleFromService =
                Map<List<TennisTournament>>(
                    (await _tennisLegacyFeedService.GetCurrentSchedules()))
                ?? new List<TennisTournament>();

            PersistToCache(key, scheduleFromService);

            return scheduleFromService;
        }

        /// <summary>
        /// Endpoint 3.2: http://{host}/tennis/{category}/live
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/live")]
        public async Task<IEnumerable<TennisTournament>> GetLive(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/live";

            var scheduleFromCache = await GetFromCacheAsync<IEnumerable<TennisTournament>>(key);
            if (scheduleFromCache != null)
                return scheduleFromCache;

            var scheduleFromService =
                Map<List<TennisTournament>>(
                    (await _tennisLegacyFeedService.GetCurrentSchedules(category)))
                ?? new List<TennisTournament>();

            PersistToCache(key, scheduleFromService);

            return scheduleFromService;
        }

        /// <summary>
        /// Endpoint 3.3: http://{host}/tennis/{category}/live/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/live/{eventId}")]
        public async Task<IEnumerable<TennisMatch>> GetLiveMatchesForLeague(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/live/{eventId}";

            var matchesFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (matchesFromCache != null)
                return matchesFromCache;

            var matchesFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetLiveMatchesForEvent(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, matchesFromService);

            return matchesFromService;
        }

        /// <summary>
        /// Endpoint 3.4: http://{host}/tennis/{category}/live/{eventId}/m
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/live/{eventId}/m")]
        public async Task<IEnumerable<TennisMatch>> GetLiveMatchesForLeagueMen(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/live/{eventId}/m";

            var matchesFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (matchesFromCache != null)
                return matchesFromCache;

            var matchesFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetLiveMatchesForEventForMen(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, matchesFromService);

            return matchesFromService;
        }

        /// <summary>
        /// Endpoint 3.5: http://{host}/tennis/{category}/live/{eventId}/m
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/live/{eventId}/w")]
        public async Task<IEnumerable<TennisMatch>> GetLiveMatchesForLeagueWomen(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/live/{eventId}/w";

            var matchesFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (matchesFromCache != null)
                return matchesFromCache;

            var matchesFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetLiveMatchesForEventForWomen(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, matchesFromService);

            return matchesFromService;
        }

        /// <summary>
        /// Endpoint 3.6: http://{host}/tennis/live/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("live/{eventId}")]
        public async Task<IEnumerable<TennisMatch>> GetLiveMatches(int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/live/{eventId}";

            var matchesFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (matchesFromCache != null)
                return matchesFromCache;

            var matchesFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetLiveMatchesForEvent(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, matchesFromService);

            return matchesFromService;
        }

        /// <summary>
        /// Endpoint 3.7: http://{host}/tennis/live/{eventId}/m
        /// </summary>
        [HttpGet, HttpHead]
        [Route("live/{eventId}/m")]
        public async Task<IEnumerable<TennisMatch>> GetLiveMatchesForMen(int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/live/{eventId}/m";

            var matchesFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (matchesFromCache != null)
                return matchesFromCache;

            var matchesFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetLiveMatchesForEventForMen(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, matchesFromService);

            return matchesFromService;
        }

        /// <summary>
        /// Endpoint 3.8: http://{host}/tennis/live/{eventId}/w
        /// </summary>
        [HttpGet, HttpHead]
        [Route("live/{eventId}/w")]
        public async Task<IEnumerable<TennisMatch>> GetLiveMatchesForWomen(int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/live/{eventId}/w";

            var matchesFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (matchesFromCache != null)
                return matchesFromCache;

            var matchesFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetLiveMatchesForEventForWomen(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, matchesFromService);

            return matchesFromService;
        }

        /// <summary>
        /// Endpoint 4.1: http://{host}/tennis/results/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("results/{eventId}")]
        public async Task<IEnumerable<TennisMatch>> GetResults(int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/results/{eventId}";

            var resultsFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetTennisResults(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, resultsFromService);

            return resultsFromService;
        }

        /// <summary>
        /// Endpoint 4.2: http://{host}/tennis/{category}/results/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results/{eventId}")]
        public async Task<IEnumerable<TennisMatch>> GetResultsForEvent(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/results/{eventId}";

            var resultsFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetTennisResults(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, resultsFromService);

            return resultsFromService;
        }

        /// <summary>
        /// Endpoint 4.3: http://{host}/tennis/{category}/results/{eventId}/m
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results/{eventId}/m")]
        public async Task<IEnumerable<TennisMatch>> GetMenResultsForEvent(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/results/{eventId}/m";

            var resultsFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetTennisResultsForMen(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, resultsFromService);

            return resultsFromService;
        }

        /// <summary>
        /// Endpoint 4.4: http://{host}/tennis/{category}/results/{eventId}/w
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results/{eventId}/w")]
        public async Task<IEnumerable<TennisMatch>> GetWomenResultsForEvent(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"tennis/{category}/results/{eventId}/w";

            var resultsFromCache = await GetFromCacheAsync<IEnumerable<TennisMatch>>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService =
                Map<List<TennisMatch>>(
                    await _tennisLegacyFeedService.GetTennisResultsForWomen(eventId))
                ?? new List<TennisMatch>();

            PersistToCache(key, resultsFromService);

            return resultsFromService;
        }

        // [TODO] Refactor this method out of this class and into a base class that has the cache.
        private void PersistToCache<T>(string cacheKey, T cacheData) where T : class
        {
            try
            {
                _cache?.Add(cacheKey, cacheData);
            }
            catch (Exception exception)
            {
                _logger?.Error("LOGGING:PersistToCache." + cacheKey, "key = " + cacheKey + " " + exception.Message + exception.StackTrace);
            }
        }

        // [TODO] Refactor this method out of this class and into a base class that has the cache.
        private async Task<T> GetFromCacheAsync<T>(string key) where T : class
        {
            try
            {
                if (_cache != null)
                {
                    return await _cache.GetAsync<T>(key);
                }

                return null;
            }
            catch (Exception exception)
            {
                _logger?.Error("LOGGING:GetFromCacheAsync." + key, "key = " + key + " " + exception.Message + exception.StackTrace);

                return null;
            }
        }
    }
}
