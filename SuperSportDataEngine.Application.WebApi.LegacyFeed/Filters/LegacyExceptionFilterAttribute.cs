using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters
{
    public class LegacyExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Return response code 200 with message "error" to simulate the feed behavior on exception.
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("error")
            };
        }
    }
}