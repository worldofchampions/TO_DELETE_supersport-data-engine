namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.DeprecatedFeed
{
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System;
    using System.Threading.Tasks;

    public interface IDeprecatedFeedIntegrationServiceMotorsport
    {
        Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(int legacyRaceEventId, DateTimeOffset raceDateTime);
    }
}
