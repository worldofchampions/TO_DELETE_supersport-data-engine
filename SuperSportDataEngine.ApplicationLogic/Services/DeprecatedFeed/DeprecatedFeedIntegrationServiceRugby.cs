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

    public class DeprecatedFeedIntegrationServiceRugby : IDeprecatedFeedIntegrationServiceRugby
    {
        private readonly IDeprecatedFeedService _deprecatedFeedService;
        private readonly ICache _cache;
        private readonly ILoggingService _logger;
        private readonly bool _serviceIsEnabled;

        public DeprecatedFeedIntegrationServiceRugby(
            IDeprecatedFeedService deprecatedFeedService,
            ICache cache,
            ILoggingService logger)
        {
            _deprecatedFeedService = deprecatedFeedService;
            _cache = cache;
            _logger = logger;
            _serviceIsEnabled = bool.Parse(ConfigurationManager.AppSettings["DeprecatedFeedIntegrationServiceRugbyIsEnabled"]);
        }

        public async Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(int legacyFixtureId, DateTimeOffset fixtureDateTime)
        {
            var result = new DeprecatedArticlesAndVideosEntity();

            if (!_serviceIsEnabled)
                return result;

            var cacheKey = "DeprecatedFeedIntegrationService:Rugby:LegacyFixtureId=" + legacyFixtureId;
            var resultFromCache = await GetFromCache<DeprecatedArticlesAndVideosEntity>(cacheKey);
            if (resultFromCache != null)
                return resultFromCache;

            const string sportName = DeprecatedFeedSportNames.Rugby;

            var taskHighlightVideos = Task.Run(async () =>
            {
                result.HighlightVideosResponse = await _deprecatedFeedService.GetHighlightVideos(sportName, legacyFixtureId);
            });

            var taskLiveVideos = Task.Run(async () =>
            {
                result.LiveVideosResponse = await _deprecatedFeedService.GetLiveVideos(sportName, legacyFixtureId);
            });

            var taskMatchDayBlog = Task.Run(async () =>
            {
                result.MatchDayBlogId = await _deprecatedFeedService.GetMatchDayBlogId(sportName, legacyFixtureId);
            });

            var taskMatchPreview = Task.Run(async () =>
            {
                result.MatchPreviewId = await _deprecatedFeedService.GetMatchPreviewId(sportName, legacyFixtureId);
            });

            var taskMatchReport = Task.Run(async () =>
            {
                result.MatchReportId = await _deprecatedFeedService.GetMatchReportId(sportName, legacyFixtureId);
            });

            await Task.WhenAll(new Task[] { taskHighlightVideos, taskLiveVideos, taskMatchDayBlog, taskMatchPreview, taskMatchReport });

            PersistToCache(cacheKey, result, fixtureDateTime);

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
                _logger?.Error(
                    "LOGGING:PersistToCache." + cacheKey,
                    exception,
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
