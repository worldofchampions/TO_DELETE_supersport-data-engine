using AutoMapper;
using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models;
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
    [LegacyExceptionFilterAttribute]
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
        [ResponseType(typeof(RugbyMatchDetailsModel))]
        public IHttpActionResult GetMatchDetails(int id)
        {
            return Ok();
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
        /// Get Fixtures for Tournament
        /// </summary>
        /// <param name="id"></param>
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
                fixtures = _rugbyService.GetTournamentFixtures(category).Select(res => Mapper.Map<Fixture>(res));
                _cache.Add(cacheKey, fixtures);
            }

            return Ok(fixtures.ToList());
        }

        /// <summary>
        /// Get Results for Tournament
        /// </summary>
        /// <param name="id"></param>
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
                results = _rugbyService.GetTournamentResults(category).Select(res => Mapper.Map<Result>(res));
                _cache.Add(cacheKey, results);
            }

            return Ok(results);

        }

        /// <summary>
        /// Get Logs for Tournament
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{category}/logs")]
        [ResponseType(typeof(List<Log>))]
        public async Task<IHttpActionResult> GetLogs(string category)
        {
            var cacheKey = $"rugby/{category}/logs";
            var logs = await _cache.GetAsync<IEnumerable<Log>>(cacheKey);

            if (logs == null)
            {
                logs =  _rugbyService.GetLogs(category).Select(log => Mapper.Map<Log>(log));
                _cache.Add(cacheKey, logs);
            }
            return Ok(logs);
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
    }
}