using SuperSportDataEngine.Common.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters
{
    public class LegacyExceptionFilter : ExceptionFilterAttribute
    {
        /// <inheritdoc />
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
            const string messageReturnedByOldFeedOnException = "error";

            context.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(messageReturnedByOldFeedOnException)
            };
        }

        private static void LogContextException(HttpActionExecutedContext context)
        {
            var loggerService = context.ActionContext.Request.GetDependencyScope().GetService(typeof(ILoggingService)) as ILoggingService;

            var requestUriString = context.Request.RequestUri.ToString();
            var indexOf = requestUriString.IndexOf("?", StringComparison.Ordinal);
            var uri = requestUriString.Substring(0, indexOf == -1 ? requestUriString.Length : indexOf);

            var queryDictionary = HttpUtility.ParseQueryString(context.Request.RequestUri.Query);
            var authKey = queryDictionary.Get("auth")?.Substring(0, 5);

            loggerService?.Fatal(
                "LegacyException." + context.Exception.Message,
                context.Exception,
                $"URL: {Environment.NewLine + uri} with auth key = {authKey} " +
                $"Message: {Environment.NewLine + context.Exception.Message} " +
                $"StackTrace: {Environment.NewLine + context.Exception.StackTrace}" +
                $"Inner Exception {Environment.NewLine + context.Exception.InnerException}");
        }
    }
}