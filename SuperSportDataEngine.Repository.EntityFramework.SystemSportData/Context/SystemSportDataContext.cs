namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using System.Data.Entity;

    public class SystemSportDataContext : DbContext
    {
        public SystemSportDataContext() : base("name=SqlDatabase_SystemSportData")
        {
        }

        public DbSet<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; set; }
        public DbSet<LegacyZoneSite> LegacyZoneSites { get; set; }
        public DbSet<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; set; }
        public DbSet<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournament { get; set; }
        public DbSet<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeason { get; set; }
        public DbSet<SportTournament> SportTournaments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfiguration.ApplyFluentApiConfigurations(modelBuilder);
        }
    }
}