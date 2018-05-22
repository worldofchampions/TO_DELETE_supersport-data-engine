using System.Configuration;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    using Filters;
    using Models.Motorsport;
    using Models.Shared;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
    using ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using Common.Interfaces;
    using Common.Logging;
    using AutoMapper;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.DeprecatedFeed;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using static AutoMapper.Mapper;

    /// <summary>
    /// LegacyFeed Motorsport Endpoints
    /// </summary>
    [LegacyExceptionFilter]
    [RoutePrefix("motorsport")]
    public class MotorsportController : ApiController
    {
        private readonly IMotorsportLegacyFeedService _motorsportLegacyFeedService;
        private readonly IDeprecatedFeedIntegrationServiceMotorsport _deprecatedFeedIntegrationServiceMotorsport;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;

        private const string CacheKeyNamespacePrefixForFeed = "LegacyFeed:Motorsport:";

        public MotorsportController(
            IMotorsportLegacyFeedService motorsportLegacyFeedService,
            IDeprecatedFeedIntegrationServiceMotorsport deprecatedFeedIntegrationServiceMotorsport,
            ICache cache,
            ILoggingService logger)
        {
            _motorsportLegacyFeedService = motorsportLegacyFeedService;
            _deprecatedFeedIntegrationServiceMotorsport = deprecatedFeedIntegrationServiceMotorsport;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint 1.1: http://{host}/motorsport/{category}/schedule
        /// Endpoint 1.2: http://{host}/motorsport/{category}/schedule?current=true
        /// Endpoint 1.2: http://{host}/motorsport/{category}/schedule?current=false
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/schedule/{current?}")]
        [ResponseType(typeof(IEnumerable<Races>))]
        public async Task<IEnumerable<Races>> GetSchedule(string category, string current = null)
        {
            var currentValue = bool.Parse(current ?? "false");

            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/schedule?current=" + currentValue;

            var scheduleFromCache = await GetFromCacheAsync<IEnumerable<Races>>(key);
            if (scheduleFromCache != null)
                return scheduleFromCache;

            var scheduleFromService =
                Map<List<Races>>(
                    (await _motorsportLegacyFeedService.GetSchedules(category, currentValue))
                        .MotorsportRaceEvents ?? new List<MotorsportRaceEvent>());

            foreach (var race in scheduleFromService)
            {
                var deprecatedArticlesAndVideosEntity = await _deprecatedFeedIntegrationServiceMotorsport.GetArticlesAndVideos(race.EventId, race.Date);
                Mapper.Map<DeprecatedArticlesAndVideosEntity, Races>(deprecatedArticlesAndVideosEntity, race);
            }

            PersistToCache(key, scheduleFromService);

            return scheduleFromService;
        }

        /// <summary>
        /// Endpoint 2.1: http://{host}/motorsport/{category}/grid
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/grid")]
        [ResponseType(typeof(MotorsportGrid))]
        public async Task<MotorsportGrid> GetGrid(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/grid";

            var gridFromCache = await GetFromCacheAsync<MotorsportGrid>(key);
            if (gridFromCache != null)
                return gridFromCache;

            var gridFromService =
                Map<MotorsportGrid>(
                    (await _motorsportLegacyFeedService.GetLatestGrid(category)));

            PersistToCache(key, gridFromService);

            return await Task.FromResult(gridFromService);
        }

        /// <summary>
        /// Endpoint 2.2: http://{host}/motorsport/{category}/grid/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/grid/{eventId:int}")]
        [ResponseType(typeof(IEnumerable<Grid>))]
        public async Task<IEnumerable<Grid>> GetGrid(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/grid/{eventId}";

            var gridFromCache = await GetFromCacheAsync<IEnumerable<Grid>>(key);
            if (gridFromCache != null)
                return gridFromCache;

            var gridFromService =
                Map<List<Grid>>(
                    (await _motorsportLegacyFeedService.GetGridForRaceEventId(category, eventId))
                    .MotorsportRaceEventGrids);

            PersistToCache(key, gridFromService);

            return gridFromService;
        }

        /// <summary>
        /// Endpoint 3.1: http://{host}/motorsport/{category}/results
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results")]
        [ResponseType(typeof(MotorsportResult))]
        public async Task<MotorsportResult> GetResults(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/results";

            var resultsFromCache = await GetFromCacheAsync<MotorsportResult>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService = Map<MotorsportResult>(
                (await _motorsportLegacyFeedService.GetLatestResult(category)));

            PersistToCache(key, resultsFromService);

            return await Task.FromResult(resultsFromService);
        }

        /// <summary>
        /// Endpoint 3.2: http://{host}/motorsport/{category}/results/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results/{eventId:int}")]
        [ResponseType(typeof(IEnumerable<ResultMotorsport>))]
        public async Task<IEnumerable<ResultMotorsport>> GetResults(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/results/{eventId}";

            var resultsFromCache = await GetFromCacheAsync<IEnumerable<ResultMotorsport>>(key);
            if (resultsFromCache != null)
                return resultsFromCache;
            
            var resultsFromService = Map<List<Models.Motorsport.ResultMotorsport>>(
                (await _motorsportLegacyFeedService.GetResultsForRaceEventId(category, eventId))
                .MotorsportRaceEventResults ?? new List<MotorsportRaceEventResult>());

            PersistToCache(key, resultsFromService);

            return resultsFromService;
        }

        /// <summary>
        /// Endpoint 4.1: http://{host}/motorsport/{category}/driverstandings
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/driverstandings")]
        [ResponseType(typeof(IEnumerable<DriverStandings>))]
        public async Task<IHttpActionResult> GetDriverStandings(string category)
        {
            var cacheKey = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/driverstandings";
            var resultFromCache = await GetFromCacheAsync<DriverStandings>(cacheKey);
            if (resultFromCache != null)
                return Ok(resultFromCache);

            var motorsportDriverStandingsEntity = await _motorsportLegacyFeedService.GetDriverStandings(category);
            var resultFromService = Map<List<DriverStandings>>(motorsportDriverStandingsEntity);

            PersistToCache(cacheKey, resultFromService);
            return Ok(resultFromService);
        }

        /// <summary>
        /// Endpoint 5.1 http://{host}/motorsport/{category}/teamstandings
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/teamstandings")]
        [ResponseType(typeof(IEnumerable<TeamStandings>))]
        public async Task<IHttpActionResult> GetTeamStandings(string category)
        {
            var cacheKey = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/teamstandings";
            var resultFromCache = await GetFromCacheAsync<TeamStandings>(cacheKey);
            if (resultFromCache != null)
                return Ok(resultFromCache);

            var motorsportTeamStandingsEntity = await _motorsportLegacyFeedService.GetTeamStandings(category);
            var resultFromService = Map<List<TeamStandings>>(motorsportTeamStandingsEntity);

            PersistToCache(cacheKey, resultFromService);
            return Ok(resultFromService);
        }

        /// <summary>
        /// Endpoint 6.1: http://{host}/motorsport/{category}/live
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/live")]
        // TODO: @motorsport-feed: An assumption is being made here with the response type, there is no response currently available to validate this endpoint against.
        [ResponseType(typeof(LiveMotorsportModel))]
        public async Task<IHttpActionResult> GetLive(string category)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetLive");
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
