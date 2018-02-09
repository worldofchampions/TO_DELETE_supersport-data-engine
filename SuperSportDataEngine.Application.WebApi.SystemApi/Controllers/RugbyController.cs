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
    //[Authorize]
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
        /// Get paginated list of rugby fixtures
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        [ActionName("fixtures")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllFixtures(int pageIndex, int pageSize)
        {
            var fixtures = await _rugbyService.GetAllFixtures(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, fixtures);
        }

        /// <summary>
        /// Get paginated list of rugby seasons
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        
        [ActionName("seasons")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllSeasons(int pageIndex, int pageSize)
        {
            var seasons = await _rugbyService.GetAllSeasons(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, seasons);
        }

        /// <summary>
        /// Get paginated list of rugby teams
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        [ActionName("teams")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTeams(int pageIndex, int pageSize)
        {
            var teams = await _rugbyService.GetAllTeams(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, teams);
        }

        /// <summary>
        /// Get paginated list of rugby players
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        [ActionName("players")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllPlayers(int pageIndex, int pageSize)
        {
            var players = await _rugbyService.GetAllPlayers(pageIndex, pageSize);

            return Request.CreateResponse(HttpStatusCode.OK, players);
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
        /// Get single fixture retrieved by fixture Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("fixtures")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetFixtureById(int id)
        {
            var fixture = await _rugbyService.GetFixtureById(id);

            if (fixture != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, fixture);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Fixture Not Found");
            }

        }

        /// <summary>
        /// Get single season retrieved by ProviderSeasonId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("seasons")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetSeasonById(int id)
        {
            var season = await _rugbyService.GetSeasonById(id);

            if (season != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, season);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Season Not Found");
            }

        }

        /// <summary>
        /// Get single team retrieved by LegacyTeamId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("teams")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTeamById(int id)
        {
            var team = await _rugbyService.GetTeamById(id);

            if (team != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, team);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Team Not Found");
            }

        }

        /// <summary>
        /// Get single player retrieved by LegacyPlayerId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("players")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPlayerById(int id)
        {
            var player = await _rugbyService.GetPlayerById(id);

            if (player != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, player);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Player Not Found");
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
        public async Task<HttpResponseMessage> PutTournament(int id, [FromBody] RugbyTournamentEntity rugbyTournamentEntity)
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

        /// <summary>
        /// Update fixture
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyFixtureEntity"></param>
        /// <returns></returns>
        [ActionName("fixtures")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutFixture(int id, [FromBody] RugbyFixtureEntity rugbyFixtureEntity)
        {
            var fixture = await _rugbyService.UpdateFixture(id, rugbyFixtureEntity);

            if (fixture)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Fixture was updated successfully");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Fixture could not be updated");
            }

        }

        /// <summary>
        /// Update season
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyseasonEntity"></param>
        /// <returns></returns>
        [ActionName("seasons")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutSeason(int id, [FromBody] RugbySeasonEntity rugbyseasonEntity)
        {
            var season = await _rugbyService.UpdateSeason(id, rugbyseasonEntity);

            if (season)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Season was updated successfully");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Season could not be updated");
            }

        }


        /// <summary>
        /// Update Team
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyTeamEntity"></param>
        /// <returns></returns>
        [ActionName("teams")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutTeam(int id, [FromBody] RugbyTeamEntity rugbyTeamEntity)
        {
            var team = await _rugbyService.UpdateTeam(id, rugbyTeamEntity);

            if (team)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Team was updated successfully");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Team could not be updated");
            }

        }

        /// <summary>
        /// Update Player
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyPlayerEntity"></param>
        /// <returns></returns>
        [ActionName("players")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutPlayer(int id, [FromBody] RugbyPlayerEntity rugbyPlayerEntity)
        {
            var player = await _rugbyService.UpdatePlayer(id, rugbyPlayerEntity);

            if (player)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Player was updated successfully");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Player could not be updated");
            }

        }
    }
}
