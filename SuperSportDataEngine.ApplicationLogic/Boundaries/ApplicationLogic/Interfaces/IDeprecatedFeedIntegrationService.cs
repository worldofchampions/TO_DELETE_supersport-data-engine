namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System.Threading.Tasks;

    public interface IDeprecatedFeedIntegrationService
    {
        Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(string sportName, int legacyFixtureId);
    }
}
