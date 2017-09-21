namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyFixture
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public long LegacyFixtureId { get; set; }

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

        public int TeamAScore { get; set; }

        public int TeamBScore { get; set; }

        /// <summary> A CMS driven value to indicate if a fixture is live scored. </summary>
        public bool IsFixtureLiveScored { get; set; }
    }
}