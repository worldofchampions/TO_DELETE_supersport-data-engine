namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.DeprecatedFeed
{
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System;
    using System.Threading.Tasks;

    public interface IDeprecatedFeedIntegrationServiceRugby
    {
        Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(int legacyFixtureId, DateTimeOffset fixtureDateTime);
    }
}
