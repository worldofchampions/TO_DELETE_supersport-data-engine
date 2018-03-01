namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public class SchedulerTrackingMotorsportRace
    {
        public Guid MotorsportRaceId { get; set; }

        public MotorsportRaceEventStatus MotorsportRaceEventStatus { get; set; }

        public SchedulerStateForMotorsportRacePolling SchedulerStateForMotorsportRacePolling { get; set; }

        public bool IsJobRunning { get; set; }
    }
}
