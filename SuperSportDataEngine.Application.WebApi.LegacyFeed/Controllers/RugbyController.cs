using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    using AutoMapper;
    using Common.Interfaces;
    using Filters;
    using Helpers.Extensions;
    using Models.Mappers;
    using Models.News;
    using Models.Rugby;
    using Models.Shared;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Common.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    /// <summary>
    /// SuperSport Rugby Endpoints
    /// </summary>
    [LegacyExceptionFilter]
    [RoutePrefix("rugby")]
    public class RugbyController : ApiController
    {
        private readonly IRugbyService _rugbyService;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;

        public RugbyController(
            IRugbyService rugbyService,
            ICache cache,
            ILoggingService logger)
        {
            _cache = cache;
            _rugbyService = rugbyService;
            _logger = logger;
        }

        /// <summary>
        /// Match Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("matchdetails/{id:int}")]
        [ResponseType(typeof(RugbyMatchDetails))]
        [LogTimeFilter]
        public async Task<IHttpActionResult> GetMatchDetails(int id)
        {
            var cacheKey = $"rugby/matchdetails/{id}";

            var matchDetailsFromCache = await GetFromCacheAsync<RugbyMatchDetails>(cacheKey);

            var response = matchDetailsFromCache;

            if ((response != null)) return Ok(response);
            
            var matchDetailsFromService = await _rugbyService.GetMatchDetailsByLegacyMatchId(id, true);

            response = Mapper.Map<RugbyMatchDetails>(matchDetailsFromService);


            if (response != null)
            {
                response.Events.AssignOrderingIds();

                PersistToCache(cacheKey, response);
            }
            else
            {
                return ReplyWithGeneralResponseModel();
            }

            return Ok(response);
        }

        /// <summary>
        /// Return GeneralResponse model if service return null for match details to mimic legacy feed behaviour.
        /// </summary>
        /// <returns></returns>
        private IHttpActionResult ReplyWithGeneralResponseModel()
        {
            var response = new GeneralResponse
            {
                Message = LegacyFeedConstants.GeneralResponseMessage
            };

            //_logger.Info("Replying with general response message.");

            return Ok(response);
        }

        /// <summary>
        /// Get News for Rugby
        /// DO NOT REMOVE. THIS HAS TO BE HERE SO WE CAN REDIRECT REQUESTS TO OLD FEED.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("news")]
        [ResponseType(typeof(NewsModel))]
        public IHttpActionResult GetNews()
        {
            return Ok();
        }

        /// <summary>
        /// Get News for Rugby
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("live")]
        [ResponseType(typeof(Match))]
        public async Task<IHttpActionResult> GetTodayFixtures()
        {
            const string cacheKey = "rugby/live/today";

            var fixtures = await GetFromCacheAsync<IEnumerable<Match>>(cacheKey);

            if (fixtures != null) return Ok(fixtures.ToList());

            fixtures = (await _rugbyService.GetCurrentDayFixturesForActiveTournaments())
                .Where(x => !x.IsDisabledOutbound)
                .Select(Mapper.Map<Match>);

            var cacheData = fixtures as IList<Match> ?? fixtures.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData.ToList());
        }

        /// <summary>
        /// Get Fixtures for Tournament
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{category}/fixtures")]
        [ResponseType(typeof(List<Fixture>))]
        public async Task<IHttpActionResult> GetFixtures(string category)
        {
            var cacheKey = $"rugby/{category}/fixtures";

            var fixtures = await GetFromCacheAsync<IEnumerable<Fixture>>(cacheKey);

            if (fixtures != null)
            {
                return Ok(fixtures.ToList());
            }

            fixtures = (await _rugbyService.GetTournamentFixtures(category))
                .Where(x => !x.IsDisabledOutbound)
                .Select(Mapper.Map<Fixture>);

            var cacheData = fixtures as IList<Fixture> ?? fixtures.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData);
        }

        [HttpGet]
        [Route("{category}/fixtures/excludeinactive")]
        [ResponseType(typeof(List<Fixture>))]
        public async Task<IHttpActionResult> GetFixturesByStatus(string category)
        {
            var cacheKey = $"rugby/{category}/fixtures/excludeinactive";

            var fixtures = await GetFromCacheAsync<IEnumerable<Fixture>>(cacheKey);

            if (fixtures != null)
            {
                return Ok(fixtures.ToList());
            }

            fixtures = (await _rugbyService.GetTournamentFixtures(category))
                .Where(x =>
                    !x.IsDisabledOutbound &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Abandoned &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Cancelled &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Delayed &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Postponed &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Suspended)
                .Select(Mapper.Map<Fixture>);

            var cacheData = fixtures as IList<Fixture> ?? fixtures.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData);
        }

        /// <summary>
        /// Get Results for Tournament
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{category}/results")]
        [ResponseType(typeof(List<Result>))]
        public async Task<IHttpActionResult> GetResults(string category)
        {
            var cacheKey = $"rugby/{category}/results";

            var results = await GetFromCacheAsync<IEnumerable<Result>>(cacheKey);

            if (results != null)
            {
                return Ok(results);
            }

            results = (await _rugbyService.GetTournamentResults(category))
                .Where(x => !x.IsDisabledOutbound)
                .Select(Mapper.Map<Result>);

            var cacheData = results as IList<Result> ?? results.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData);
        }

        [HttpGet]
        [Route("{category}/results/excludeinactive")]
        [ResponseType(typeof(List<Result>))]
        public async Task<IHttpActionResult> GetResultsByStatus(string category)
        {
            var cacheKey = $"rugby/{category}/results/excludeinactive";

            var results = await GetFromCacheAsync<IEnumerable<Result>>(cacheKey);

            if (results != null)
            {
                return Ok(results);
            }

            results = (await _rugbyService.GetTournamentResults(category))
                .Where(x =>
                    !x.IsDisabledOutbound &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Abandoned &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Cancelled &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Delayed &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Postponed &&
                    x.RugbyFixtureStatus != RugbyFixtureStatus.Suspended)
                .Select(Mapper.Map<Result>);

            var cacheData = results as IList<Result> ?? results.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData);
        }

        /// <summary>
        /// Get Logs for Tournament
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{category}/logs")]
        [ResponseType(typeof(List<Log>))]
        public async Task<IHttpActionResult> GetLogs(string category)
        {
            var flatLogsCacheKey = $"rugby/flatLogs/{category}";

            var flatLogsCache = await GetFromCacheAsync<IEnumerable<Log>>(flatLogsCacheKey);

            if (flatLogsCache != null)
            {
                return Ok(flatLogsCache);
            }

            var groupedLogsCacheKey = $"rugby/groupedLogs/{category}";

            var groupedLogsCache = await GetFromCacheAsync<IEnumerable<Log>>(groupedLogsCacheKey);

            if (groupedLogsCache != null)
            {
                return Ok(groupedLogsCache);
            }

            return await GetLogsFromService(category);
        }

        /// <summary>
        /// Get Tournament News
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{category}/news")]
        [ResponseType(typeof(NewsModel))]
        public IHttpActionResult GetTournamentNews()
        {
            return Ok();
        }

        /// <summary>
        /// Get News for specific Tournament
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{category}/news/{id:int}")]
        [ResponseType(typeof(NewsModel))]
        public IHttpActionResult GetTournamentNews(int id)
        {
            return Ok();
        }

        private async Task<IHttpActionResult> GetLogsFromService(string category)
        {
            var flatLogsFromService = await _rugbyService.GetFlatLogs(category);

            const int emptyCollectionCount = 0;

            var logsFromService = flatLogsFromService as IList<RugbyFlatLog> ?? flatLogsFromService.ToList();

            if (logsFromService.Count() > emptyCollectionCount)
            {
                var flatLogsCacheKey = $"rugby/flatLogs/{category}";

                var flatLogsCache = logsFromService.Select(Mapper.Map<Log>);

                var logsCache = flatLogsCache as IList<Log> ?? flatLogsCache.ToList();

                PersistToCache(flatLogsCacheKey, logsCache);

                return Ok(logsCache);
            }

            var groupedLogsFromService = await _rugbyService.GetGroupedLogs(category);

            var rugbyGroupedLogs = groupedLogsFromService as IList<RugbyGroupedLog> ?? groupedLogsFromService.ToList();

            if (rugbyGroupedLogs.Count() <= emptyCollectionCount)
            {
                return Ok(Enumerable.Empty<Log>());
            }

            var groupedLogsCacheKey = $"rugby/groupedLogs/{category}";

            var groupedLogsCache = rugbyGroupedLogs.Select(Mapper.Map<Log>).ToList();

            PersistToCache(groupedLogsCacheKey, groupedLogsCache);

            return Ok(groupedLogsCache);
        }

        private void PersistToCache<T>(string cacheKey, T cacheData) where T : class
        {
            try
            {
                _cache?.Add(cacheKey, cacheData);
            }
            catch (Exception exception)
            {
                var loggerService = ActionContext.Request.GetDependencyScope().GetService(typeof(ILoggingService)) as ILoggingService;

                loggerService?.Error("PersistToCache." + cacheKey, "key = " + cacheKey + " " + exception.Message + exception.StackTrace);
                loggerService?.Debug("PersistToCache.CacheData." + cacheKey, "cacheData = " + JsonConvert.SerializeObject(cacheData));
            }
        }

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
                var loggerService = ActionContext.Request.GetDependencyScope().GetService(typeof(ILoggingService)) as ILoggingService;

                loggerService?.Error("GetFromCacheAsync." + key, "key = " + key + " " + exception.Message + exception.StackTrace);

                return null;
            }
        }
    }
}