namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

    public class SchedulerTrackingMotorsportRaceEvent : BaseModel
    {
        public Guid MotorsportRaceEventId { get; set; }

        public Guid MotorsportLeagueId { get; set; }

        public DateTimeOffset? StartDateTime { get; set; }

        /// <summary> This value is set once we receive the corresponding status from the supplier data. </summary>
        public DateTimeOffset? EndedDateTime { get; set; }

        public MotorsportRaceEventStatus MotorsportRaceEventStatus { get; set; }

        public bool IsJobRunning { get; set; }
    }
}