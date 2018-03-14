using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using static AutoMapper.Mapper;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    using Common.Interfaces;
    using Filters;
    using Models.Motorsport;
    using Models.Shared;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
    using Common.Logging;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    /// <summary>
    /// LegacyFeed Motorsport Endpoints
    /// </summary>
    [LegacyExceptionFilter]
    [RoutePrefix("motorsport")]
    public class MotorsportController : ApiController
    {
        private readonly IMotorsportLegacyFeedService _motorsportLegacyFeedService;
        private readonly IDeprecatedFeedIntegrationService _deprecatedFeedIntegrationService;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;

        private const string CacheKeyNamespacePrefixForFeed = "LegacyFeed:Motorsport:";

        public MotorsportController(
            IMotorsportLegacyFeedService motorsportLegacyFeedService,
            IDeprecatedFeedIntegrationService deprecatedFeedIntegrationService,
            ICache cache,
            ILoggingService logger)
        {
            _motorsportLegacyFeedService = motorsportLegacyFeedService;
            _deprecatedFeedIntegrationService = deprecatedFeedIntegrationService;
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
        [ResponseType(typeof(IEnumerable<RacesModel>))]
        public async Task<IEnumerable<RacesModel>> GetSchedule(string category, string current = null)
        {
            var currentValue = bool.Parse(current ?? "false");

            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/schedule?current=" + currentValue;

            var scheduleFromCache = await GetFromCacheAsync<IEnumerable<RacesModel>>(key);
            if (scheduleFromCache != null)
                return scheduleFromCache;

            var scheduleFromService =
                Map<List<RacesModel>>(
                    (await _motorsportLegacyFeedService.GetSchedules(category, currentValue))
                        .MotorsportRaceEvents ?? new List<MotorsportRaceEvent>());

            PersistToCache(key, scheduleFromService);

            return scheduleFromService;
        }

        /// <summary>
        /// Endpoint 2.1: http://{host}/motorsport/{category}/grid
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/grid")]
        [ResponseType(typeof(GridEventModel))]
        public async Task<GridEventModel> GetGrid(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/grid";

            var gridFromCache = await GetFromCacheAsync<GridEventModel>(key);
            if (gridFromCache != null)
                return gridFromCache;

            var gridFromService =
                Map<GridEventModel>(
                    (await _motorsportLegacyFeedService.GetLatestGrid(category)));

            PersistToCache(key, gridFromService);

            return await Task.FromResult(gridFromService);
        }

        /// <summary>
        /// Endpoint 2.2: http://{host}/motorsport/{category}/grid/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/grid/{eventId:int}")]
        [ResponseType(typeof(IEnumerable<GridModel>))]
        public async Task<IEnumerable<GridModel>> GetGrid(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/grid/{eventId}";

            var gridFromCache = await GetFromCacheAsync<IEnumerable<GridModel>>(key);
            if (gridFromCache != null)
                return gridFromCache;

            var gridFromService =
                Map<List<GridModel>>(
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
        [ResponseType(typeof(ResultEventMotorsportModel))]
        public async Task<ResultEventMotorsportModel> GetResults(string category)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/results";

            var resultsFromCache = await GetFromCacheAsync<ResultEventMotorsportModel>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService =
                Map<ResultEventMotorsportModel>(
                    (await _motorsportLegacyFeedService.GetLatestResult(category)));

            PersistToCache(key, resultsFromService);

            return await Task.FromResult(resultsFromService);
        }

        /// <summary>
        /// Endpoint 3.2: http://{host}/motorsport/{category}/results/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results/{eventId:int}")]
        [ResponseType(typeof(IEnumerable<ResultMotorsportModel>))]
        public async Task<IEnumerable<ResultMotorsportModel>> GetResults(string category, int eventId)
        {
            var key = CacheKeyNamespacePrefixForFeed + $"motorsport/{category}/results/{eventId}";

            var resultsFromCache = await GetFromCacheAsync<IEnumerable<ResultMotorsportModel>>(key);
            if (resultsFromCache != null)
                return resultsFromCache;

            var resultsFromService =
                Map<List<ResultMotorsportModel>>(
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
        [ResponseType(typeof(IEnumerable<DriverStandingsModel>))]
        public async Task<IHttpActionResult> GetDriverStandings(string category)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetDriverStandings");
        }

        /// <summary>
        /// Endpoint 5.1 http://{host}/motorsport/{category}/teamstandings
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/teamstandings")]
        [ResponseType(typeof(IEnumerable<TeamStandingsModel>))]
        public async Task<IHttpActionResult> GetTeamStandings(string category)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetTeamStandings");
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
