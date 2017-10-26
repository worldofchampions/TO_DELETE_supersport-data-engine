namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using System;

    public class SchedulerTrackingRugbyTournament : BaseModel
    {
        public Guid TournamentId { get; set; }

        public Guid SeasonId { get; set; }

        public SchedulerStateForRugbyLogPolling SchedulerStateLogs { get; set; }

        public SchedulerStateForManagerJobPolling SchedulerStateForManagerJobPolling { get; set; }
    }
}