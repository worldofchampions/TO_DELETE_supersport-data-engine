namespace SuperSportDataEngine.ApplicationLogic.Services.DeprecatedFeed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.DeprecatedFeed;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using SuperSportDataEngine.ApplicationLogic.Services.DeprecatedFeed.Constants;
    using SuperSportDataEngine.Common.Interfaces;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    public class DeprecatedFeedIntegrationServiceMotorsport : IDeprecatedFeedIntegrationServiceMotorsport
    {
        private readonly IDeprecatedFeedService _deprecatedFeedService;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;
        private readonly bool _serviceIsEnabled;

        public DeprecatedFeedIntegrationServiceMotorsport(
            IDeprecatedFeedService deprecatedFeedService,
            ICache cache,
            ILoggingService logger)
        {
            _deprecatedFeedService = deprecatedFeedService;
            _cache = cache;
            _logger = logger;
            _serviceIsEnabled = bool.Parse(ConfigurationManager.AppSettings["DeprecatedFeedIntegrationServiceMotorsportIsEnabled"]);
        }

        public async Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(int legacyRaceEventId, DateTimeOffset raceDateTime)
        {
            var result = new DeprecatedArticlesAndVideosEntity();

            if (!_serviceIsEnabled)
                return result;

            var cacheKey = "DeprecatedFeedIntegrationService:Motorsport:LegacyRaceEventId=" + legacyRaceEventId;
            var resultFromCache = await GetFromCache<DeprecatedArticlesAndVideosEntity>(cacheKey);
            if (resultFromCache != null)
                return resultFromCache;

            const string sportName = DeprecatedFeedSportNames.Motorsport;

            var taskHighlightVideos = Task.Run(async () =>
            {
                result.HighlightVideosResponse = await _deprecatedFeedService.GetHighlightVideos(sportName, legacyRaceEventId);
            });

            var taskLiveVideos = Task.Run(async () =>
            {
                result.LiveVideosResponse = await _deprecatedFeedService.GetLiveVideos(sportName, legacyRaceEventId);
            });

            await Task.WhenAll(new Task[] { taskHighlightVideos, taskLiveVideos });

            PersistToCache(cacheKey, result, raceDateTime);

            return result;
        }

        private void PersistToCache<T>(string cacheKey, T cacheData, DateTimeOffset eventDateTime) where T : class
        {
            try
            {
                var duration = (eventDateTime - DateTimeOffset.Now).Duration();

                TimeSpan ttl;
                if (duration.Days < 1) ttl = TimeSpan.FromMinutes(5);
                else if (duration.Days < 7) ttl = TimeSpan.FromMinutes(15);
                else ttl = TimeSpan.FromHours(4);

                _cache?.Add(cacheKey, cacheData, ttl);
            }
            catch (Exception exception)
            {
                _logger?.Error("LOGGING:PersistToCache." + cacheKey, exception, 
                    $"key = {cacheKey}\n" +
                    $"Message: {exception.Message}\n" +
                    $"Stack Trace: {exception.StackTrace}\n" +
                    $"Inner Exception: {exception.InnerException}");
            }
        }

        private async Task<T> GetFromCache<T>(string key) where T : class
        {
            try
            {
                if (_cache != null)
                    return await _cache.GetAsync<T>(key);

                return null;
            }
            catch (Exception exception)
            {
                _logger?.Error(
                    "LOGGING:GetFromCache." + key, 
                    exception, 
                    $"key = {key}\n" +
                    $"Message: {exception.Message}\n" +
                    $"Stack Trace: {exception.StackTrace}\n" +
                    $"Inner Exception: {exception.InnerException}");
                return null;
            }
        }
    }
}
