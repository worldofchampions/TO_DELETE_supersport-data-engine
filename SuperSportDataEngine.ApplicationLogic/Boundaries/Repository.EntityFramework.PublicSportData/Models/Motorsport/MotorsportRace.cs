namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

    public class MotorsportRace: BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyRaceId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderRaceId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string RaceName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string RaceNameCmsOverride { get; set; }

        /// <summary> A provider driven value. </summary>
        public string RaceNameAbbreviation { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string RaceNameAbbreviationCmsOverride { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}