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

        public MotorsportRaceStatus MotorsportRaceStatus { get; set; }

        /// <summary> A provider driven value. </summary>
        public string RaceName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string RaceNameCmsOverride { get; set; }

        /// <summary> A provider driven value. </summary>
        public string RaceNameAbbreviation { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string RaceNameAbbreviationCmsOverride { get; set; }

        /// <summary>
        /// A CMS driven value to indicate that a race should not be served out, e.g. to cater for
        /// scenarios when encountering a problem with provider data. This field has no impact on the
        /// ingest process.
        /// </summary>
        public bool IsDisabledOutbound { get; set; }

        /// <summary>
        /// A CMS driven value to indicate that a race should not be queried from the provider, e.g.
        /// the provider has deleted this race from their database. This field has an impact on the
        /// ingest process.
        /// </summary>
        public bool IsDisabledInbound { get; set; }

        /// <summary> A CMS driven value to indicate if a race is live scored. </summary>
        public bool IsLiveScored { get; set; }

        /// <summary> 
        /// Provides a mode to manually handle race data e.g. to cater for scenarios when 
        /// temporarily encountering a problem with provider data. Whilst the CMS override mode is 
        /// active, the following values are controlled manually (i.e. not set by ingest): 
        /// - TBC: StartDateTime 
        /// - TBC: RaceStatus
        /// </summary> 
        public bool CmsOverrideModeIsActive { get; set; }

        public DateTimeOffset? StartDateTimeUtc { get; set; }

        //TODO: Figure out how to calculate this. Confirm if provider does not serve it.
        public DateTimeOffset? EndDateTimeUtc { get; set; }

        public string CountryName { get; set; }

        public string CountryAbbreviation { get; set; }

        public string CityName { get; set; }

        public string VenueName { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }

        public virtual MotorsportRaceResult MotorsportRaceResult { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}