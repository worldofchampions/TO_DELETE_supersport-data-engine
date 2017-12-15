using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Extensions
{
    public static class WebExtensions
    {
        public static async Task<WebResponse> GetResponseAsync(this WebRequest request, int timeoutInSeconds, ILoggingService logger)
        {
            var requestTask = Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
            var timeOutTask = Task.Delay(timeoutInSeconds * 1000);

            var result = await Task.WhenAny(requestTask, timeOutTask).ConfigureAwait(false);

            if (result != timeOutTask) return requestTask.Result;

            request.Abort();
            
            await logger.Error("HTTPRequestTimedOut." + request.RequestUri, "Request timed out. " + request.RequestUri, WebExceptionStatus.Timeout);

            return null;
        }
    }
}
