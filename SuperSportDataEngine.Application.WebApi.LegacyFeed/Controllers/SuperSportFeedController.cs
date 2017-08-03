using SuperSportDataEngine.Application.WebApi.LegacyFeed.ViewModel;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers
{
    [RoutePrefix("{sport}")]
    public class SuperSportFeedController : ApiController
    {
        [HttpGet]
        [Route("matchdetails/{id:int}")]
        public IHttpActionResult MatchDetails(string sport, int id)
        {
            return Ok(new LogsViewModel());
        }


        [HttpGet]
        [Route("{category}/{type}")]
        public IHttpActionResult Get(string sport, string category, string type)
        {
            return Ok(new LogsViewModel());
        }

        
    }
}