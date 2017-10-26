namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using System;

    public class SchedulerTrackingRugbySeason : BaseModel
    {
        public Guid SeasonId { get; set; }

        public Guid TournamentId { get; set; }

        public RugbySeasonStatus RugbySeasonStatus { get; set; }

        public SchedulerStateForManagerJobPolling SchedulerStateForManagerJobPolling { get; set; }
    }
}