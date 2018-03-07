using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
    using SuperSportDataEngine.Common.Logging;
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
        public async Task<IHttpActionResult> GetSchedule(string category, string current = null)
        {
            // TODO: @motorsport-feed: Convert "current" string to a boolean type, then pass that to service as boolean parameter.
            // TODO: @motorsport-feed: implement.

         //   var eventsFromService = _motorsportLegacyFeedService.GetSchedules();
            return Content(HttpStatusCode.OK, "DEBUG GetSchedule");
        }

        /// <summary>
        /// Endpoint 2.1: http://{host}/motorsport/{category}/grid
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/grid")]
        // TODO: @motorsport-feed: Can't find existing model in project source. Create a new model that contains a collection of <GridModel> for this one with the additional required fields?
        //[ResponseType(typeof(IEnumerable<GridModel>))]
        public async Task<IHttpActionResult> GetGrid(string category)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetGrid");
        }

        /// <summary>
        /// Endpoint 2.2: http://{host}/motorsport/{category}/grid/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/grid/{eventId:int}")]
        [ResponseType(typeof(IEnumerable<GridModel>))]
        public async Task<IHttpActionResult> GetGrid(string category, int eventId)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetGrid + eventId");
        }

        /// <summary>
        /// Endpoint 3.1: http://{host}/motorsport/{category}/results
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results")]
        // TODO: @motorsport-feed: Can't find existing model in project source. Create a new model that contains a collection of <ResultMotorsportModel> for this one with the additional required fields?
        //[ResponseType(typeof(IEnumerable<ResultMotorsportModel>))]
        public async Task<IHttpActionResult> GetResults(string category)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetResults");
        }

        /// <summary>
        /// Endpoint 3.2: http://{host}/motorsport/{category}/results/{eventId}
        /// </summary>
        [HttpGet, HttpHead]
        [Route("{category}/results/{eventId:int}")]
        [ResponseType(typeof(IEnumerable<ResultMotorsportModel>))]
        public async Task<IHttpActionResult> GetResults(string category, int eventId)
        {
            // TODO: @motorsport-feed: implement.
            return Content(HttpStatusCode.OK, "DEBUG GetResults + eventId");
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
                _cache?.Add(CacheKeyNamespacePrefixForFeed + cacheKey, cacheData);
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
                    return await _cache.GetAsync<T>(CacheKeyNamespacePrefixForFeed + key);
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
