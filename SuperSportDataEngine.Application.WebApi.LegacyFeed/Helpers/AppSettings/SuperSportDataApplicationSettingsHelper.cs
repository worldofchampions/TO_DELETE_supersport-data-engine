using System.Configuration;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers.AppSettings
{
    public static class SuperSportDataApplicationSettingsHelper
    {
        private const string SUPERSPORTFEED_HOST = "SuperSportFeed.IpAddress";

        public static string GetSuperSportFeedHost(string defaultValue = null)
        {
            var superSportFeedHost = ConfigurationManager.AppSettings[SUPERSPORTFEED_HOST];

            if (string.IsNullOrWhiteSpace(superSportFeedHost) && !string.IsNullOrWhiteSpace(defaultValue))
                superSportFeedHost = defaultValue;

            return superSportFeedHost;
        }
    }

    
    
}