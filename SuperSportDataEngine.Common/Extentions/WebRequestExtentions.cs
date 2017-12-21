using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Common.Extentions
{
    public static class WebRequestExtentions
    {
        public static string GetBaseUri(this WebRequest webRequest)
        {
            var requestUriString = webRequest.RequestUri.ToString();
            return requestUriString.Substring(0, requestUriString.IndexOf("?", StringComparison.Ordinal));
        }
    }
}
