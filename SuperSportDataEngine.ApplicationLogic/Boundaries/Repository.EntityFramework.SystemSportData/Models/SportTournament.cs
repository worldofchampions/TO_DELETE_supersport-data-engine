namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using System;

    // TODO: Temporary example reference code for team (implement all required fields correctly later etc.).
    public class SportTournament
    {
        public Guid Id { get; set; }
        public string TournamentName { get; set; }
        public bool IsEnabled { get; set; }
    }
}