namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

    public class MotorRace: BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorLeagueId { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyRaceId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderRaceId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string NameCmsOverride { get; set; }

        /// <summary> A provider driven value. </summary>
        public string NameAbbreviation { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string RaceNameShortCmsOverride { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string DisplayNameCmsOverride { get; set; }

        /// <summary> A CMS driven value. </summary>
        public bool IsEnabled { get; set; }

        public string CountryName { get; set; }

        public string CountryAbbreviation { get; set; }

        public string CityName { get; set; }

        public string VenueName { get; set; }

        public virtual MotorLeague MotorLeague { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}