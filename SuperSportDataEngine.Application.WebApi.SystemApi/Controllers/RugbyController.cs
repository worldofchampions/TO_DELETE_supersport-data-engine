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
    /// <summary>
    /// Rugby Controller to manage rugby related data
    /// </summary>
    public class RugbyController : ApiController
    {
        IRugbyCmsService _rugbyService;

        /// <summary>
        /// Rugby Constructor
        /// </summary>
        /// <param name="rugbyService"></param>
        public RugbyController(IRugbyCmsService rugbyService)
        {
            _rugbyService = rugbyService;
        }

        /// <summary>
        /// Get paginated list of rugby tournaments
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ActionName("tournaments")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTournaments(int pageIndex, int pageSize)
        {
            var tournaments = await _rugbyService.GetAllTournaments(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, tournaments);
        }

        /// <summary>
        /// Get single tournament by tournament Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("tournaments")]
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

        /// <summary>
        /// Update tournament
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyTournamentEntity"></param>
        /// <returns></returns>
        [ActionName("tournaments")]
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
