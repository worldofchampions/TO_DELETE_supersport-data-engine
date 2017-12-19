namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.ResponseModels;

    public interface IDeprecatedFeedService
    {
        Task<IEnumerable<HighlightVideosResponse>> GetHighlightVideos(string sportName, int legacyFixtureId);

        Task<IEnumerable<LiveVideosResponse>> GetLiveVideos(string sportName, int legacyFixtureId);

        Task<int> GetMatchDayBlogId(string sportName, int legacyFixtureId);

        Task<int> GetMatchPreviewId(string sportName, int legacyFixtureId);

        Task<int> GetMatchReportId(string sportName, int legacyFixtureId);
    }
}
