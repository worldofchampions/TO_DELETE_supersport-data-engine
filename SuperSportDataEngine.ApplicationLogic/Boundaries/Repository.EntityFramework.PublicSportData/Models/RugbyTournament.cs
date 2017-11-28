namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyTournament : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyTournamentId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderTournamentId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string NameCmsOverride { get; set; }

        /// <summary> A CMS defined value to uniquely identify a tournament for URL purposes. </summary>
        public string Slug { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Abbreviation { get; set; }

        /// <summary> A provider driven value. </summary>
        public string LogoUrl { get; set; }

        /// <summary> A CMS driven value to indicate if a tournament is enabled for ingest. </summary>
        public bool IsEnabled { get; set; }

        /// <summary> A CMS driven value to indicate if a tournament is live scored. </summary>
        public bool IsLiveScored { get; set; }

        /// <summary> A CMS driven value to indicate if the tournament has logs/ladder standings. </summary>
        public bool HasLogs { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}