namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using System;

    public class SchedulerTrackingRugbyFixture
    {
        public Guid FixtureId { get; set; }

        public Guid TournamentId { get; set; }

        public DateTimeOffset StartDateTime { get; set; }

        /// <summary> This value is set once we receive the corresponding status from the supplier data. </summary>
        public DateTimeOffset EndedDateTime { get; set; }

        public RugbyFixtureStatus RugbyFixtureStatus { get; set; }

        public SchedulerStateForRugbyFixturePolling SchedulerStateFixtures { get; set; }
    }
}