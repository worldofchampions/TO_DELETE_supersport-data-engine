using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    [RoutePrefix("Auth")]
    public class AuthImportsController : ApiController
    {
        private readonly ILegacyAuthService _authService;

        public AuthImportsController(ILegacyAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("ZoneSiteImport")]
        public async Task<IHttpActionResult> ImportZoneSiteRecords(IEnumerable<LegacyZoneSiteEntity> models)
        {
            return Ok(await _authService.ImportZoneSiteRecords(models));
        }

        [HttpPost]
        [Route("AuthFeedImport")]
        public async Task<IHttpActionResult> ImportFeedAuthConsumerRecords(IEnumerable<LegacyAuthFeedConsumerEntity> models)
        {
            await _authService.ImportAuthFeedRecords(models);
            return Ok();
        }
    }
}