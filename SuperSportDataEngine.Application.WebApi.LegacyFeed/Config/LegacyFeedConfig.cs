using System;
using System.Configuration;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Config
{
    public static class LegacyFeedConfig
    {
        public static bool IsRequestHandlerEnabled
        {
            get
            {
                bool isRequestHandlerEnabled = Boolean.Parse(ConfigurationManager.AppSettings["IsRequestHandlerEnabled"]);

                return isRequestHandlerEnabled;
            }
        }
    }
}