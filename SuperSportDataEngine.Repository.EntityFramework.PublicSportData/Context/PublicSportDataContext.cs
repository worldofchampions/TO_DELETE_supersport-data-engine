namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Data.Entity;

    public class PublicSportDataContext : DbContext
    {
        public PublicSportDataContext() : base("name=SqlDatabase_PublicSportData")
        {
        }

        public DbSet<DataProvider> DataProviders { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<RugbySeason> RugbySeasons { get; set; }
        public DbSet<RugbyTournament> RugbyTournaments { get; set; }
        public DbSet<Sport> Sports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfiguration.ApplyFluentApiConfigurations(modelBuilder);
        }
    }
}