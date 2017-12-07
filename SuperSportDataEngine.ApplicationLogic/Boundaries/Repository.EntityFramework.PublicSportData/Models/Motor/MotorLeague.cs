using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorLeague
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyLeagueId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderLeagueId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A provider driven value. </summary>
        public string ProviderSlug { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string NameCmsOverride { get; set; }

        /// <summary> A CMS defined value to uniquely identify a league for URL purposes. </summary>
        public string Slug { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Abbreviation { get; set; }

        /// <summary> A provider driven value. </summary>
        public string DisplayName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string DisplayNameCmsOverride { get; set; }

        /// <summary> A CMS driven value to indicate if a league is enabled for ingest. </summary>
        public bool IsEnabled { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}