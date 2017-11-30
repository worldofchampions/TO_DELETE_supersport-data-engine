using System;
using System.Configuration;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Config
{
    public static class LegacyFeedConfig
    {
        public static bool IsRequestRedirectorEnabled
        {
            get
            {
                var isRequestHandlerEnabled = bool.Parse(ConfigurationManager.AppSettings["IsRequestRedirectorEnabled"]);

                return isRequestHandlerEnabled;
            }
        }
    }
}