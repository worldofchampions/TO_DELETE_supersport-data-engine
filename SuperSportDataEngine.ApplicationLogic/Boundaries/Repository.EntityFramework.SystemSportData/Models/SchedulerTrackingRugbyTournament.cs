namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using System;

    public class SchedulerTrackingRugbyTournament
    {
        public Guid TournamentId { get; set; }
        public Guid SeasonId { get; set; }
        public SchedulerStateForRugbyLogPolling SchedulerStateLogs { get; set; }
    }
}