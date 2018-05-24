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
