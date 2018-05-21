namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public class SchedulerTrackingMotorsportRaceEvent : BaseModel
    {
        public Guid MotorsportRaceEventId { get; set; }

        public Guid MotorsportLeagueId { get; set; }

        public DateTimeOffset? StartDateTimeUtc { get; set; }

        /// <summary> This value is set once we receive the corresponding status from the supplier data. </summary>
        public DateTimeOffset? EndedDateTimeUtc { get; set; }

        public MotorsportRaceEventStatus MotorsportRaceEventStatus { get; set; }

        public SchedulerStateForMotorsportRaceEventGridPolling SchedulerStateForMotorsportRaceEventGridPolling { get; set; }

        public SchedulerStateForMotorsportRaceEventLivePolling SchedulerStateForMotorsportRaceEventLivePolling { get; set; }

        public SchedulerStateForMotorsportRaceEventResultsPolling SchedulerStateForMotorsportRaceEventResultsPolling { get; set; }

        public SchedulerStateForMotorsportRaceEventStandingsPolling SchedulerStateForMotorsportRaceEventStandingsPolling { get; set; }

        public SchedulerStateForMotorsportRaceEventPolling SchedulerStateForMotorsportRaceEventPolling { get; set; }

        public SchedulerStateForMotorsportRaceEventCalendarPolling SchedulerStateForMotorsportRaceEventCalendarPolling { get; set; }
    }
}