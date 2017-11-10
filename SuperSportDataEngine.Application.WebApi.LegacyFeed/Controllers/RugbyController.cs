using AutoMapper;
using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.News;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    /// <summary>
    /// SuperSport Rugby Endpoints
    /// </summary>
    [LegacyExceptionFilter]
    [RoutePrefix("rugby")]
    public class RugbyController : ApiController
    {
        private readonly IRugbyService _rugbyService;
        private readonly ICache _cache;

        public RugbyController(IRugbyService rugbyService,
            ICache cache)
        {
            _cache = cache;
            _rugbyService = rugbyService;
        }

        /// <summary>
        /// Match Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("matchdetails/{id:int}")]
        [ResponseType(typeof(RugbyMatchDetails))]
        public async Task<IHttpActionResult> GetMatchDetails(int id)
        {
            var cacheKey = $"rugby/matchdetails/{id}";

            var matchDetailsFromCache = await GetFromCacheAsync<RugbyMatchDetails>(cacheKey);

            RugbyMatchDetails response = matchDetailsFromCache;

            if (response is null)
            {
                var matchDetailsFromService = await _rugbyService.GetMatchDetailsByLegacyMatchId(id);

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

            return Ok(response);
        }

        /// <summary>
        /// Get News for Rugby
        /// DO NOT REMOVE. THIS HAS BE HERE SO WE CAN REDIRECT REQUESTS TO OLD FEED.
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
            var cacheKey = $"rugby/live/today";

            var fixtures = await GetFromCacheAsync<IEnumerable<Match>>(cacheKey);

            if (fixtures == null)
            {
                fixtures = (await _rugbyService.GetCurrentDayFixturesForActiveTournaments()).Select(res => Mapper.Map<Match>(res));

                PersistToCache(cacheKey, fixtures);
            }

            return Ok(fixtures.ToList());
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

            IEnumerable<Fixture> fixtures = await GetFromCacheAsync<IEnumerable<Fixture>>(cacheKey);

            if (fixtures == null)
            {
                fixtures = (await _rugbyService.GetTournamentFixtures(category)).Select(res => Mapper.Map<Fixture>(res));

                PersistToCache(cacheKey, fixtures);
            }

            return Ok(fixtures.ToList());
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

            IEnumerable<Result> results = await GetFromCacheAsync<IEnumerable<Result>>(cacheKey);

            if (results == null)
            {
                results = (await _rugbyService.GetTournamentResults(category)).Select(res => Mapper.Map<Result>(res));

                PersistToCache(cacheKey, results);
            }

            return Ok(results);
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

            const int EmptyCollectionCount = 0;

            if (flatLogsFromService.Count() > EmptyCollectionCount)
            {
                var flatLogsCacheKey = $"rugby/flatLogs/{category}";

                var flatLogsCache = flatLogsFromService.Select(logItem => Mapper.Map<Log>(logItem));

                PersistToCache(flatLogsCacheKey, flatLogsCache);

                return Ok(flatLogsCache);
            }

            var groupedLogsFromService = await _rugbyService.GetGroupedLogs(category);

            if (groupedLogsFromService.Count() > EmptyCollectionCount)
            {
                var groupedLogsCacheKey = $"rugby/groupedLogs/{category}";

                var groupedLogsCache = groupedLogsFromService.Select(logItem => Mapper.Map<Log>(logItem)).ToList();

                PersistToCache(groupedLogsCacheKey, groupedLogsCache);

                return Ok(groupedLogsCache);
            }

            return Ok(Enumerable.Empty<Log>());
        }

        private void PersistToCache<T>(string cacheKey, T cacheData) where T : class
        {
            if (_cache != null)
            {
                _cache.Add(cacheKey, cacheData);
            }
        }

        private async Task<T> GetFromCacheAsync<T>(string key) where T : class
        {
            if (_cache != null)
            {
                return await _cache.GetAsync<T>(key);
            }

            return null;
        }
    }
}