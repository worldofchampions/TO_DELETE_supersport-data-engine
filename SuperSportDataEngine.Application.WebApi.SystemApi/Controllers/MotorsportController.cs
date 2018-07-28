using SuperSportDataEngine.Application.WebApi.SystemApi.Authentication;
using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport;
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
    /// Motorsport Controller to manage motorsport related data
    /// </summary>
    [BasicAuthentication]
    public class MotorsportController : ApiController
    {
        IMotorsportCmsService _motorsportService;
        private readonly ILoggingService _logger;
        private string path = HttpContext.Current.Request.Url.AbsolutePath;

        /// <summary>
        /// Motorsport Constructor
        /// </summary>
        /// <param name="motorsportService"></param>
        /// <param name="logger"></param>
        public MotorsportController(IMotorsportCmsService motorsportService, ILoggingService logger)
        {
            _motorsportService = motorsportService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of motorsport leagues
        /// And searching by passing query parameter
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against league name
        /// </param>
        /// <returns></returns>

        [ActionName("leagues")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllLeagues(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var leagues = await _motorsportService.GetAllLeagues(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, leagues);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving leagues !");
            }
        }

        /// <summary>
        /// Get paginated list of motorsport teams and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against team name
        /// </param>
        /// <returns></returns>

        [ActionName("teams")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTeams(int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var teams = await _motorsportService.GetAllTeams(pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, teams);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving teams !");
            }
        }

        /// <summary>
        /// Get paginated list of motorsport league seasons and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against season name for leagueId
        /// </param>
        /// <returns></returns>

        [Route("api/Motorsport/league/{leagueId:guid}/seasons")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllLeagueSeasons(Guid leagueId, int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var leagueSeasons = await _motorsportService.GetSeasonsForLeague(leagueId, pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, leagueSeasons);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving seasons for league !");
            }
        }

        /// <summary>
        /// Get paginated list of motorsport league races and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against race name for leagueId
        /// </param>
        /// <returns></returns>

        [Route("api/Motorsport/league/{leagueId:guid}/races")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllLeagueRaces(Guid leagueId, int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var leagueRaces = await _motorsportService.GetRacesForLeague(leagueId, pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, leagueRaces);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving races for league !");
            }
        }

        /// <summary>
        /// Get paginated list of motorsport league drivers and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="query">
        /// Search against driver name for leagueId
        /// </param>
        /// <returns></returns>

        [Route("api/Motorsport/league/{leagueId:guid}/drivers")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllLeagueDrivers(Guid leagueId, int pageIndex, int pageSize, string query = null)
        {
            try
            {
                var leagueDrivers = await _motorsportService.GetDriversForLeague(leagueId, pageIndex, pageSize, path, query);
                return Request.CreateResponse(HttpStatusCode.OK, leagueDrivers);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving drivers for league !");
            }
        }

        /// <summary>
        /// Get paginated list of motorsport race events and return a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="raceId"></param>
        /// <param name="pageIndex">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Size of records to be returned
        /// </param>
        /// <param name="seasonId">
        /// SeasonId for race event
        /// </param>
        /// <param name="query">
        /// Search against race events CircuitName, CountryName, CityName
        /// </param>
        /// <param name="status">
        /// Specify game status E.g. Results, Today, Coming Up
        /// </param>
        /// <returns></returns>

        [Route("api/Motorsport/race/{raceId:guid}/events")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllFixturesForTournament(Guid raceId, int pageIndex, int pageSize, Guid? seasonId = null, string query = null, string status = null)
        {
            try
            {
                var raceEvents = await _motorsportService.GetRaceEvents(raceId, seasonId, pageIndex, pageSize, path, query, status);
                return Request.CreateResponse(HttpStatusCode.OK, raceEvents);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while retrieving race events !");
            }
        }

        /// <summary>
        /// Get motorsport league by league Id
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("leagues")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetLeagueById(Guid id)
        {
            try
            {
                var league = await _motorsportService.GetLeagueById(id);

                if (league != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, league);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "League Not Found");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing league !");
            }

        }

        /// <summary>
        /// Get motorsport season by league Id
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
                var season = await _motorsportService.GetSeasonById(id);

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
        /// Get single team retrieved by team Id
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
                var team = await _motorsportService.GetTeamById(id);

                if (team != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, team);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Team Not Found");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing team !");
            }

        }

        /// <summary>
        /// Get single race retrieved by race Id
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("races")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetRaceById(Guid id)
        {
            try
            {
                var race = await _motorsportService.GetRaceById(id);

                if (race != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, race);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Race Not Found");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing race !");
            }
        }

        /// <summary>
        /// Get single driver retrieved by driver Id
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("drivers")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDriverById(Guid id)
        {
            try
            {
                var driver = await _motorsportService.GetDriverById(id);

                if (driver != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, driver);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Driver Not Found");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing driver !");
            }
        }

        /// <summary>
        /// Get single race event retrieved by raceEvent Id
        /// Return 404 if not found and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("raceevents")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetRaceEventById(Guid id)
        {
            try
            {
                var raceEvent = await _motorsportService.GetRaceEventById(id);

                if (raceEvent != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, raceEvent);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Race Event Not Found");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while processing race event !");
            }
        }

        /// <summary>
        /// Update league
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motorsportLeagueEntity"></param>
        /// <returns></returns>
        [ActionName("leagues")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutLeague(Guid id, [FromBody] MotorsportLeagueEntity motorsportLeagueEntity)
        {
            try
            {
                var league = await _motorsportService.UpdateLeague(id, motorsportLeagueEntity);

                if (league)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "League was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "League could not be updated");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating league !");
            }
        }

        /// <summary>
        /// Update season
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="leagueId"></param>
        /// <param name="motorsportseasonEntity"></param>
        /// <returns></returns>
        [ActionName("seasons")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutSeason(Guid id, Guid leagueId, [FromBody] MotorsportSeasonEntity motorsportseasonEntity)
        {
            try
            {
                var season = await _motorsportService.UpdateSeason(id, leagueId, motorsportseasonEntity);

                if (season)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Season was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Season could not be updated");
                }
            }
            catch (Exception exception)
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
        /// <param name="motorsportTeamEntity"></param>
        /// <returns></returns>
        [ActionName("teams")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutTeam(Guid id, [FromBody] MotorsportTeamEntity motorsportTeamEntity)
        {
            try
            {
                var team = await _motorsportService.UpdateTeam(id, motorsportTeamEntity);

                if (team)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Team was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Team could not be updated");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating team !");
            }

        }

        /// <summary>
        /// Update Race
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motorsportRaceEntity"></param>
        /// <returns></returns>
        [ActionName("races")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutRace(Guid id, [FromBody] MotorsportRaceEntity motorsportRaceEntity)
        {
            try
            {
                var race = await _motorsportService.UpdateRace(id, motorsportRaceEntity);

                if (race)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Race was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Race could not be updated");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating race !");
            }

        }

        /// <summary>
        /// Update Driver
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motorsportDriverEntity"></param>
        /// <returns></returns>
        [ActionName("drivers")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutDriver(Guid id, [FromBody] MotorsportDriverEntity motorsportDriverEntity)
        {
            try
            {
                var driver = await _motorsportService.UpdateDriver(id, motorsportDriverEntity);

                if (driver)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Driver was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Driver could not be updated");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating driver !");
            }
        }

        /// <summary>
        /// Update RaceEvent
        /// Return 406 if update doesn't succeed and a 500 error response if something failed while doing it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motorsportRaceEventEntity"></param>
        /// <returns></returns>
        [ActionName("raceevents")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutRaceEvent(Guid id, [FromBody] MotorsportRaceEventEntity motorsportRaceEventEntity)
        {
            try
            {
                var driver = await _motorsportService.UpdateRaceEvent(id, motorsportRaceEventEntity);

                if (driver)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Race Event was updated successfully");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Race Event could not be updated");
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while updating race event !");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
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
