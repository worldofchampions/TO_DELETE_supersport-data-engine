using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

                await logger.Error("PROVIDERCALLS:REQUESTDURATION:" + request.GetBaseUri(), "Request timed out. " + request.RequestUri, WebExceptionStatus.Timeout);

                return null;
            }
            catch (Exception e)
            {
                await logger.Error("PROVIDERCALLS:EXCEPTION:" + request.GetBaseUri().Replace(":", ""), "Provider throwing exception for request to " + request.GetBaseUri() + ": " + e.StackTrace);
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
