namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers.Extensions;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.News;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
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
        private readonly IDeprecatedFeedIntegrationService _deprecatedFeedIntegrationService;
        private readonly IRugbyService _rugbyService;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;

        private const string CacheKeyNamespacePrefixForFeed = "LegacyFeed:";

        public RugbyController(
            IRugbyService rugbyService,
            IDeprecatedFeedIntegrationService deprecatedFeedIntegrationService,
            ICache cache,
            ILoggingService logger)
        {
            _deprecatedFeedIntegrationService = deprecatedFeedIntegrationService;
            _rugbyService = rugbyService;
            _cache = cache;
            _logger = logger;
        }

        [Route("{category}/try-scorers")]
        [ResponseType(typeof(List<RugbyPointsScorerModel>))]
        public async Task<IHttpActionResult> GetTournamentTryScorers(string category)
        {
            const string cachePrefix = "TOURNAMENT:";
            var cacheKey = $"{cachePrefix}rugby/{category}try-scores";

            var players = await GetFromCacheAsync<IEnumerable<RugbyPointsScorerModel>>(cacheKey);

            if (players != null) return Ok(players.ToList());

            players = (await _rugbyService.GetTournamentTryScorers(category))
                .Select(Mapper.Map<RugbyPointsScorerModel>).ToList();

            if (!players.Any()) return Ok(new List<RugbyPointsScorerModel>());

            var cacheData = (IList<RugbyPointsScorerModel>)players;

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData.ToList());
        }

        /// <summary>
        /// Match Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, HttpHead]
        [Route("matchdetails/{id:int}")]
        [ResponseType(typeof(RugbyMatchDetails))]
        [LogTimeFilter]
        public async Task<IHttpActionResult> GetMatchDetails(int id)
        {
            const string cachePrefix = "MATCHDETAILS:";
            var cacheKey = cachePrefix + $"rugby/matchdetails/{id}";

            var matchDetailsFromCache = await GetFromCacheAsync<RugbyMatchDetails>(cacheKey);

            var response = matchDetailsFromCache;

            if (response != null)
                return Ok(response);

            var matchDetailsFromService = await _rugbyService.GetMatchDetailsByLegacyMatchId(id, true);

            response = Mapper.Map<RugbyMatchDetails>(matchDetailsFromService);

            if (response != null)
            {
                response.Events.AssignOrderingIds();

                var deprecatedArticlesAndVideosEntity = await _deprecatedFeedIntegrationService.GetArticlesAndVideos("rugby", id);
                response = Mapper.Map<DeprecatedArticlesAndVideosEntity, RugbyMatchDetails>(deprecatedArticlesAndVideosEntity, response);

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
        [HttpGet, HttpHead]
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
        [HttpGet, HttpHead]
        [Route("live")]
        [ResponseType(typeof(Match))]
        public async Task<IHttpActionResult> GetTodayFixtures()
        {
            const string cachePrefix = "LIVE:";
            const string cacheKey = cachePrefix + "rugby/live/today";

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
        /// Get Today fixtures for a tournament
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet, HttpHead]
        [Route("{category}/live")]
        [ResponseType(typeof(List<Match>))]
        public async Task<IHttpActionResult> GetTodayFixturesForTournament(string category)
        {
            const string cachePrefix = "LIVE:";
            var cacheKey = $"{cachePrefix}rugby/{category}live";

            var fixtures = await GetFromCacheAsync<IEnumerable<Match>>(cacheKey);

            if (fixtures != null) return Ok(fixtures.ToList());

            fixtures = (await _rugbyService.GetCurrentDayFixturesForTournament(category))
                .Where(x => !x.IsDisabledOutbound)
                .Select(Mapper.Map<Match>).ToList();

            if (!fixtures.Any()) return ReplyWithGeneralResponseModel();

            var cacheData = (IList<Match>)fixtures;

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData.ToList());
        }

        /// <summary>
        /// Get Fixtures for Rugby (all tournaments)
        /// </summary>
        /// <returns></returns>
        [HttpGet, HttpHead]
        [Route("fixtures")]
        [ResponseType(typeof(List<Fixture>))]
        public async Task<IHttpActionResult> GetFixtures()
        {
            const string cachePrefix = "FIXTURES:";
            var cacheKey = cachePrefix + $"rugby/fixtures";

            var fixtures = await GetFromCacheAsync<IEnumerable<Fixture>>(cacheKey);

            if (fixtures != null)
            {
                return Ok(fixtures.ToList());
            }

            fixtures = (await _rugbyService.GetUpcomingFixtures())
                .Select(Mapper.Map<Fixture>);

            var cacheData = fixtures as IList<Fixture> ?? fixtures.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData);
        }

        /// <summary>
        /// Get Fixtures for Tournament
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet, HttpHead]
        [Route("{category}/fixtures")]
        [ResponseType(typeof(List<Fixture>))]
        public async Task<IHttpActionResult> GetFixtures(string category)
        {
            const string cachePrefix = "FIXTURES:";
            var cacheKey = cachePrefix + $"rugby/{category}/fixtures";

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

        [HttpGet, HttpHead]
        [Route("{category}/fixtures/excludeinactive")]
        [ResponseType(typeof(List<Fixture>))]
        public async Task<IHttpActionResult> GetFixturesByStatus(string category)
        {
            const string cachePrefix = "FIXTURES:";
            var cacheKey = cachePrefix + $"rugby/{category}/fixtures/excludeinactive";

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
        /// Get Results for Rugby (all tournaments)
        /// </summary>
        /// <returns></returns>
        [HttpGet, HttpHead]
        [Route("results")]
        [ResponseType(typeof(List<Result>))]
        public async Task<IHttpActionResult> GetRugbyResults()
        {
            const string cachePrefix = "RESULTS:";
            var cacheKey = cachePrefix + $"rugby/results";

            var results = await GetFromCacheAsync<IEnumerable<Result>>(cacheKey);

            if (results != null)
            {
                return Ok(results);
            }

            var maxCount = int.Parse(ConfigurationManager.AppSettings["MaxResponseCount.RugbyRecentFixtures"]);

            results = (await _rugbyService.GetRecentResultsFixtures(maxCount))
                .Select(Mapper.Map<Result>);

            var cacheData = results as IList<Result> ?? results.ToList();

            PersistToCache(cacheKey, cacheData);

            return Ok(cacheData);
        }

        /// <summary>
        /// Get Results for Tournament
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet, HttpHead]
        [Route("{category}/results")]
        [ResponseType(typeof(List<Result>))]
        public async Task<IHttpActionResult> GetResults(string category)
        {
            const string cachePrefix = "RESULTS:";
            var cacheKey = cachePrefix + $"rugby/{category}/results";

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

        [HttpGet, HttpHead]
        [Route("{category}/results/excludeinactive")]
        [ResponseType(typeof(List<Result>))]
        public async Task<IHttpActionResult> GetResultsByStatus(string category)
        {
            const string cachePrefix = "RESULTS:";
            var cacheKey = cachePrefix + $"rugby/{category}/results/excludeinactive";

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
        [HttpGet, HttpHead]
        [Route("{category}/logs/{groupName?}")]
        [ResponseType(typeof(List<Log>))]
        public async Task<IHttpActionResult> GetLogs(string category, string groupName = null)
        {
            const string flatLogsCacheKeyPrefix = "FLATLOGS:";
            var flatLogsCacheKey = flatLogsCacheKeyPrefix + $"rugby/flatLogs/{category}";

            var flatLogsCache = await GetFromCacheAsync<IEnumerable<Log>>(flatLogsCacheKey);

            if (flatLogsCache != null)
            {
                return Ok(flatLogsCache);
            }

            const string groupedLogsCacheKeyPrefix = "GROUPEDLOGS:";
            var groupedLogsCacheKey = groupedLogsCacheKeyPrefix + $"rugby/groupedLogs/{category}";

            var groupedLogsCache = await GetFromCacheAsync<IEnumerable<Log>>(groupedLogsCacheKey);

            if (groupedLogsCache != null)
            {
                if (groupName != null)
                    groupedLogsCache = groupedLogsCache.Where(g => String.Equals(g.GroupName, groupName, StringComparison.CurrentCultureIgnoreCase));

                return Ok(groupedLogsCache);
            }

            return await GetLogsFromService(category, groupName);
        }

        /// <summary>
        /// Get Tournament News
        /// </summary>
        /// <returns></returns>
        [HttpGet, HttpHead]
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
        [HttpGet, HttpHead]
        [Route("{category}/news/{id:int}")]
        [ResponseType(typeof(NewsModel))]
        public IHttpActionResult GetTournamentNews(int id)
        {
            return Ok();
        }

        private async Task<IHttpActionResult> GetLogsFromService(string category, string groupName)
        {
            var flatLogsFromService = await _rugbyService.GetFlatLogs(category);

            const int emptyCollectionCount = 0;

            var logsFromService = flatLogsFromService as IList<RugbyFlatLog> ?? flatLogsFromService.ToList();

            if (logsFromService.Count() > emptyCollectionCount)
            {
                const string flatLogsCacheKeyPrefix = "FLATLOGS:";
                var flatLogsCacheKey = flatLogsCacheKeyPrefix + $"rugby/flatLogs/{category}";

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

            const string groupedLogsCacheKeyPrefix = "GROUPEDLOGS:";
            var groupedLogsCacheKey = groupedLogsCacheKeyPrefix + $"rugby/groupedLogs/{category}";

            var groupedLogsCache = rugbyGroupedLogs.Select(Mapper.Map<Log>).ToList();

            PersistToCache(groupedLogsCacheKey, groupedLogsCache);

            if (groupName != null)
                groupedLogsCache = groupedLogsCache.Where(g => String.Equals(g.GroupName, groupName, StringComparison.CurrentCultureIgnoreCase)).ToList();

            return Ok(groupedLogsCache);
        }

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
