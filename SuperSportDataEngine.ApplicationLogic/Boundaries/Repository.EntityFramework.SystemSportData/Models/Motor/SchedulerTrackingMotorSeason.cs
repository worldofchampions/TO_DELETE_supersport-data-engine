using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    public class SchedulerTrackingMotorSeason: BaseModel
    {
        public Guid SeasonId { get; set; }

        public Guid LeagueId { get; set; }

        public MotorSeasonStatus MotorSeasonStatus { get; set; }

        public SchedulerStateForManagerJobPolling SchedulerStateForManagerJobPolling { get; set; }
    }
}