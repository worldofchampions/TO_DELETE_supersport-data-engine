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

        public RugbyFixtureStatus RugbyFixtureStatus { get; set; }

        public virtual RugbyTournament RugbyTournament { get; set; }

        public virtual RugbyVenue RugbyVenue { get; set; }

        public virtual RugbyTeam HomeTeam { get; set; }

        public virtual RugbyTeam AwayTeam { get; set; }

        public virtual DataProvider DataProvider { get; set; }
    }
}