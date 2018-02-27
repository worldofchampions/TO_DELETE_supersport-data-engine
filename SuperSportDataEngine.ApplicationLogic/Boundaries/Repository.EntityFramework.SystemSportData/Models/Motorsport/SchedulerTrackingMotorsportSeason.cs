namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public class SchedulerTrackingMotorSeason: BaseModel
    {
        public Guid SeasonId { get; set; }

        public Guid LeagueId { get; set; }

        public MotorSeasonStatus MotorSeasonStatus { get; set; }

        public SchedulerStateForManagerJobPolling SchedulerStateForManagerJobPolling { get; set; }
    }
}