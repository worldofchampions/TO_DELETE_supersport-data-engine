using SuperSportDataEngine.Application.WebApi.SystemApi.Authentication;
using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using SuperSportDataEngine.Common.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Controllers
{
    /// <summary>
    /// Rugby Controller to manage rugby related data
    /// </summary>
    [BasicAuthentication]
    public class RugbyController : ApiController
    {
        IRugbyCmsService _rugbyService;
        private readonly ILoggingService _logger;
        private string path = HttpContext.Current.Request.Url.AbsolutePath;

        /// <summary>
        /// Rugby Constructor
        /// </summary>
        /// <param name="rugbyService"></param>
        /// <param name="logger"></param>
        public RugbyController(IRugbyCmsService rugbyService, ILoggingService logger)
        {
            _rugbyService = rugbyService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of rugby tournaments
        /// And searching by passing query parameter
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against tournament name
        /// </param>
        /// <returns></returns>

        [ActionName("tournaments")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTournaments(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var tournaments = await _rugbyService.GetAllTournaments(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, tournaments);
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving tournaments !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby fixtures and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against team names
        /// </param>
        /// <returns></returns>

        [ActionName("fixtures")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllFixtures(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var fixtures = await _rugbyService.GetAllFixtures(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, fixtures);
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving fixtures !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby seasons and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against season name
        /// </param>
        /// <returns></returns>

        [ActionName("seasons")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllSeasons(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var seasons = await _rugbyService.GetAllSeasons(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, seasons);
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving seasons !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby teams and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against team name and team abbreviation
        /// </param>
        /// <returns></returns>

        [ActionName("teams")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTeams(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var teams = await _rugbyService.GetAllTeams(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, teams);
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving teams !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby players and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against player's name
        /// </param>
        /// <returns></returns>

        [ActionName("players")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllPlayers(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var players = await _rugbyService.GetAllPlayers(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, players);
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving players !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby tournament seasons and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against season name for tournamentId
        /// </param>
        /// <returns></returns>

        [Route("api/Rugby/tournament/{tournamentId:guid}/seasons")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTournamentSeasons(Guid tournamentId, int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var tournamentSeasons = await _rugbyService.GetSeasonsForTournament(tournamentId, pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, tournamentSeasons);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving seasons for tournament !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby season fixtures and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="seasonId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against team names for seasonId
        /// </param>
        /// <returns></returns>

        [Route("api/Rugby/season/{seasonId:guid}/fixtures")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllFixturesForTournamentSeason(Guid seasonId, int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var seasonFixtures = await _rugbyService.GetFixturesForTournamentSeason(seasonId, pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, seasonFixtures);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving fixtures for season !");
            }
        }

        /// <summary>
        /// Get paginated list of rugby tournament fixtures and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against fixtures team names
        /// </param>
        /// <returns></returns>

        [Route("api/Rugby/tournament/{tournamentId:guid}/fixtures")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllFixturesForTournament(Guid tournamentId, int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var tournamentFixtures = await _rugbyService.GetTournamentFixtures(tournamentId, pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, tournamentFixtures);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving tournament fixtures !");
            }
        }

        /// <summary>
        /// Get single tournament by tournament Id
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("tournaments")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTournamentById(Guid id)
        {
            try
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
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing tournament !");
            }

        }

        /// <summary>
        /// Get single fixture retrieved by fixture Id
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("fixtures")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetFixtureById(Guid id)
        {
            try
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
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing fixture !");
            }
        }

        /// <summary>
        /// Get single season retrieved by ProviderSeasonId
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("seasons")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetSeasonById(Guid id)
        {
            try
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
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing season !");
            }
        }

        /// <summary>
        /// Get single team retrieved by LegacyTeamId
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("teams")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTeamById(Guid id)
        {
            try
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
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing team !");
            }

        }

        /// <summary>
        /// Get single player retrieved by LegacyPlayerId
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("players")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPlayerById(Guid id)
        {
            try
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
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing player !");
            }
        }

        /// <summary>
        /// Update tournament
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyTournamentEntity"></param>
        /// <returns></returns>
        [ActionName("tournaments")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutTournament(Guid id, [FromBody] RugbyTournamentEntity rugbyTournamentEntity)
        {
            try
            {
                var tournament = await _rugbyService.UpdateTournament(id, rugbyTournamentEntity);

                if (tournament)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Tournament was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Tournament could not be updated");
                }
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating tournament !");
            }
        }

        /// <summary>
        /// Update fixture
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyFixtureEntity"></param>
        /// <returns></returns>
        [ActionName("fixtures")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutFixture(Guid id, [FromBody] RugbyFixtureEntity rugbyFixtureEntity)
        {
            try
            {
                var fixture = await _rugbyService.UpdateFixture(id, rugbyFixtureEntity);

                if (fixture)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Fixture was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Fixture could not be updated");
                }
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating fixtures !");
            }
        }

        /// <summary>
        /// Update season
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyseasonEntity"></param>
        /// <returns></returns>
        [ActionName("seasons")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutSeason(Guid id, [FromBody] RugbySeasonEntity rugbyseasonEntity)
        {
            try
            {
                var season = await _rugbyService.UpdateSeason(id, rugbyseasonEntity);

                if (season)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Season was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Season could not be updated");
                }
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating season !");
            }
        }


        /// <summary>
        /// Update Team
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyTeamEntity"></param>
        /// <returns></returns>
        [ActionName("teams")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutTeam(Guid id, [FromBody] RugbyTeamEntity rugbyTeamEntity)
        {
            try
            {
                var team = await _rugbyService.UpdateTeam(id, rugbyTeamEntity);

                if (team)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Team was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Team could not be updated");
                }
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating team !");
            }

        }

        /// <summary>
        /// Update Player
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rugbyPlayerEntity"></param>
        /// <returns></returns>
        [ActionName("players")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutPlayer(Guid id, [FromBody] RugbyPlayerEntity rugbyPlayerEntity)
        {
            try
            {
                var player = await _rugbyService.UpdatePlayer(id, rugbyPlayerEntity);

                if (player)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Player was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Player could not be updated");
                }
            }
            catch(Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating player !");
            }
        }

        protected async void LogException(Exception exception)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(0);

            MethodBase currentMethodName = sf.GetMethod();

            await _logger.Error(string.Format("{0}/{1}", currentMethodName.ReflectedType.FullName, HttpContext.Current.Request.Url.PathAndQuery),
                    "Message: \n" + exception.Message +
                    "StackTrace: \n" + exception.StackTrace +
                    "Inner Exception \n" + exception.InnerException);
        }
    }
}
