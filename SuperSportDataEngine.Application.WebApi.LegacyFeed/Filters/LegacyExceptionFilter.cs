using SuperSportDataEngine.Common.Logging;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters
{
    public class LegacyExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Return response code 200 with message "error" to simulate the feed behavior on exception.
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            CreateResponseSimilarToOldFeed(context);

            LogContextException(context);
        }

        private static void CreateResponseSimilarToOldFeed(HttpActionExecutedContext context)
        {
            string messageReturnedByOldFeedOnException = "error";

            context.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(messageReturnedByOldFeedOnException)
            };
        }

        private static void LogContextException(HttpActionExecutedContext context)
        {
            var loggerService = context.ActionContext.Request.GetDependencyScope().GetService(typeof(ILoggingService)) as ILoggingService;

            loggerService.Fatal(context.Exception);
        }
    }
}