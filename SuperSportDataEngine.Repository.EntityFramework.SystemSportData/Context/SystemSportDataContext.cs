using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis;

namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.Common.Database;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;

    public class SystemSportDataContext : DbContext
    {
        public SystemSportDataContext() : base("name=SqlDatabase_SystemSportData")
        {
        }

        public DbSet<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; set; }
        public DbSet<LegacyZoneSite> LegacyZoneSites { get; set; }
        public DbSet<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; set; }
        public DbSet<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeason { get; set; }
        public DbSet<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournament { get; set; }
        public DbSet<SchedulerDashboardUser> SchedulingDashboardUsers { get; set; }
        public DbSet<SchedulerTrackingMotorsportRaceEvent> SchedulerTrackingMotorsportRaceEvents { get; set; }
        public DbSet<SchedulerTrackingTennisEvent> SchedulerTrackingTennisEvents { get; set; }
        public DbSet<SchedulerTrackingTennisMatch> SchedulerTrackingTennisMatches { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfiguration.ApplyFluentApiConfigurations(modelBuilder);
        }

        public override int SaveChanges()
        {
            TimestampHelper.ApplyTimestamps(this);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            TimestampHelper.ApplyTimestamps(this);
            return base.SaveChangesAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            TimestampHelper.ApplyTimestamps(this);
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}