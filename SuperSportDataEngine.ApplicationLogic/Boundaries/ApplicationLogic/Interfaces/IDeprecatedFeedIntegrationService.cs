namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using System.Threading.Tasks;

    public interface IDeprecatedFeedIntegrationService
    {
        Task<DeprecatedArticlesAndVideosEntity> GetArticlesAndVideos(string sportName, int legacyFixtureId);
    }

    /// <summary>
    /// String constants for sport names to be passed for Deprecated Feed Integration Article/Video calls.
    /// </summary>
    public static class DeprecatedFeedSportNames
    {
        public const string Cricket = "cricket";
        public const string Football = "football";
        public const string Golf = "golf";
        public const string Motorsport = "motorsport";
        public const string Rugby = "rugby";
        public const string Tennis = "tennis";
    }
}
