using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Controllers
{
    public class RugbyController : ApiController
    {
        IRugbyCmsService _rugbyService;

        public RugbyController(IRugbyCmsService rugbyService)
        {
            _rugbyService = rugbyService;
        }

        [Route("tournaments")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTournaments(int pageIndex, int pageSize)
        {
            var tournaments = await _rugbyService.GetAllTournaments(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, tournaments);
        }

        [Route("tournaments")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTournamentById(int id)
        {
            var tournament = await _rugbyService.GetTournamentById(id);

            if (tournament != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, tournament);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Tournament Not Found");
            }

        }

        [Route("tournaments")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateTournament(int id, [FromBody] RugbyTournamentEntity rugbyTournamentEntity)
        {
            var tournament = await _rugbyService.UpdateTournament(id, rugbyTournamentEntity);

            if (tournament)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Tournament was updated successfully");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Tournament could not be updated");
            }

        }
    }
}
