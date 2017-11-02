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
            var matchDetails = await _rugbyService.GetMatchDetailsByLegacyMatchId(id);

            if (matchDetails is null)
            {
                return ReplyWithGeneralResponseModel();
            }

            var response = Mapper.Map<RugbyMatchDetails>(matchDetails);

            response.Events.AssignOrderingIds();

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
            var fixtures = await _cache.GetAsync<IEnumerable<Match>>(cacheKey);

            if (fixtures == null)
            {
                 fixtures = (await _rugbyService.GetCurrentDayFixturesForActiveTournaments()).Select(res => Mapper.Map<Match>(res));

                _cache.Add(cacheKey, fixtures);
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

            var fixtures = await _cache.GetAsync<IEnumerable<Fixture>>(cacheKey);

            if (fixtures == null)
            {
                fixtures = (await _rugbyService.GetTournamentFixtures(category)).Select(res => Mapper.Map<Fixture>(res));

                _cache.Add(cacheKey, fixtures);
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

            var results = await _cache.GetAsync<IEnumerable<Result>>(cacheKey);

            if (results == null)
            {
                results = (await _rugbyService.GetTournamentResults(category)).Select(res => Mapper.Map<Result>(res));

                _cache.Add(cacheKey, results);
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

            var flatLogsCache = await _cache.GetAsync<IEnumerable<Log>>(flatLogsCacheKey);

            if (flatLogsCache != null)
            {
                return Ok(flatLogsCache);
            }

            var groupedLogsCacheKey = $"rugby/groupedLogs/{category}";

            var groupedLogsCache = await _cache.GetAsync<IEnumerable<Log>>(groupedLogsCacheKey);

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

                _cache.Add(flatLogsCacheKey, flatLogsCache);

                return Ok(flatLogsCache);
            }

            var groupedLogsFromService = await _rugbyService.GetGroupedLogs(category);

            if (groupedLogsFromService.Count() > EmptyCollectionCount)
            {
                var groupedLogsCacheKey = $"rugby/groupedLogs/{category}";

                var groupedLogsCache = groupedLogsFromService.Select(logItem => Mapper.Map<Log>(logItem)).ToList();

                _cache.Add(groupedLogsCacheKey, groupedLogsCache);

                return Ok(groupedLogsCache);
            }

            return Ok(Enumerable.Empty<Log>());
        }
    }
}