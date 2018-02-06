namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyFixture : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyFixtureId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public long ProviderFixtureId { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary> A provider driven value. </summary>
        public bool TeamAIsHomeTeam { get; set; }

        /// <summary> A provider driven value. </summary>
        public bool TeamBIsHomeTeam { get; set; }

        public RugbyFixtureStatus RugbyFixtureStatus { get; set; }

        public DataProvider DataProvider { get; set; }

        public virtual RugbyTournament RugbyTournament { get; set; }

        public virtual RugbyVenue RugbyVenue { get; set; }

        public virtual RugbyTeam TeamA { get; set; }

        public virtual RugbyTeam TeamB { get; set; }

        public int? TeamAScore { get; set; }

        public int? TeamBScore { get; set; }

        /// <summary>
        /// A CMS driven value to indicate that a fixture should not be served out, e.g. to cater for
        /// scenarios when encountering a problem with provider data. This field has no impact on the
        /// ingest process.
        /// </summary>
        public bool IsDisabledOutbound { get; set; }

        /// <summary>
        /// A CMS driven value to indicate that a fixture should not be queried from the provider, e.g.
        /// the provider has deleted this fixture from their database. This field has an impact on the
        /// ingest process.
        /// </summary>
        public bool IsDisabledInbound { get; set; }

        /// <summary> A CMS driven value to indicate if a fixture is live scored. </summary>
        public bool IsLiveScored { get; set; }

        /// <summary> 
        /// Provides a mode to manually handle fixture data e.g. to cater for scenarios when 
        /// temporarily encountering a problem with provider data. Whilst the CMS override mode is 
        /// active, the following values are controlled manually (i.e. not set by ingest): 
        /// - StartDateTime 
        /// - RugbyFixtureStatus 
        /// - TeamAScore 
        /// - TeamBScore 
        /// </summary> 
        public bool CmsOverrideModeIsActive { get; set; }

        public int GameTimeInSeconds { get; set; }

        public int RoundNumber { get; set; }
    }
}