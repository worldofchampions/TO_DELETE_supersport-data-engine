namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System.Threading.Tasks;

    public class DeprecatedFeedIntegrationService : IDeprecatedFeedIntegrationService
    {
        private readonly IDeprecatedFeedService _deprecatedFeedService;

        public DeprecatedFeedIntegrationService(IDeprecatedFeedService deprecatedFeedService)
        {
            _deprecatedFeedService = deprecatedFeedService;
        }

        public async Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(string sportName, int legacyFixtureId)
        {
            var result = new DeprecatedArticlesAndVideosEntity();

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
                result.MatchDayBlogId = await _deprecatedFeedService.GetMatchDayBlogId(sportName, legacyFixtureId).ConfigureAwait(false);
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

            return result;
        }
    }
}
