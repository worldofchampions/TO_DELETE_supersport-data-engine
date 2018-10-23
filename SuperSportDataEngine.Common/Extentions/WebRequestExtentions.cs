using System;
using System.Net;
using System.Threading.Tasks;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Common.Extentions
{
    public static class WebRequestExtentions
    {
        public static async Task<WebResponse> GetResponseAsync(this WebRequest request, int timeoutInMilliseconds, ILoggingService logger)
        {
            try
            {
                var requestTask = Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
                var timeOutTask = Task.Delay(timeoutInMilliseconds);

                var result = await Task.WhenAny(requestTask, timeOutTask).ConfigureAwait(false);

                if (result != timeOutTask) return requestTask.Result;

                request.Abort();

                await logger.Error("PROVIDERCALLS:REQUESTDURATION:" + request.GetBaseUri(),
                    null, 
                    $"Request timed out. Timeout length is {timeoutInMilliseconds / 1000.0} seconds. Requested Uri: {request.GetBaseUri()}");

                return null;
            }
            catch (Exception e)
            {
                await logger.Error("PROVIDERCALLS:EXCEPTION:" + request.GetBaseUri().Replace(":", ""),
                    e, $"Provider throwing exception for request to {request.GetBaseUri()}, Stack Trace: {e.StackTrace}");
                return null;
            }
        }

        public static string GetBaseUri(this WebRequest webRequest)
        {
            var requestUriString = webRequest.RequestUri.ToString();
            var indexOf = requestUriString.IndexOf("?", StringComparison.Ordinal);
            return requestUriString.Substring(0, indexOf == -1 ? requestUriString.Length : indexOf );
        }
    }
}
