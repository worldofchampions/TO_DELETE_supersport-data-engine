namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.ResponseModels;
    using System.Collections.Generic;

    public class DeprecatedArticlesAndVideosEntity
    {
        public IEnumerable<HighlightVideosResponse> HighlightVideosResponse { get; set; }

        public IEnumerable<LiveVideosResponse> LiveVideosResponse { get; set; }

        public int MatchDayBlogId { get; set; }

        public int MatchPreviewId { get; set; }

        public int MatchReportId { get; set; }
    }
}
