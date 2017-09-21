namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Data.Entity;

    public class PublicSportDataContext : DbContext
    {
        public PublicSportDataContext() : base("name=SqlDatabase_PublicSportData")
        {
        }

        public DbSet<RugbyFixture> RugbyFixtures { get; set; }
        public DbSet<RugbyLog> RugbyLogs { get; set; }
        public DbSet<RugbyPlayer> RugbyPlayers { get; set; }
        public DbSet<RugbyPlayerLineup> RugbyPlayerLineups { get; set; }
        public DbSet<RugbySeason> RugbySeasons { get; set; }
        public DbSet<RugbyTeam> RugbyTeams { get; set; }
        public DbSet<RugbyTournament> RugbyTournaments { get; set; }
        public DbSet<RugbyVenue> RugbyVenues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfiguration.ApplyFluentApiConfigurations(modelBuilder);
        }
    }
}