using System;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.Common.Database;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;

    public class PublicSportDataContext : DbContext
    {
        public PublicSportDataContext() : base("name=SqlDatabase_PublicSportData")
        {
        }

        public DbSet<RugbyCommentary> RugbyCommentaries { get; set; }
        public DbSet<RugbyEventType> RugbyEventTypes { get; set; }
        public DbSet<RugbyEventTypeProviderMapping> RugbyEventTypeProviderMappings { get; set; }
        public DbSet<RugbyFixture> RugbyFixtures { get; set; }
        public DbSet<RugbyFlatLog> RugbyFlatLogs { get; set; }
        public DbSet<RugbyGroupedLog> RugbyGroupedLogs { get; set; }
        public DbSet<RugbyLogGroup> RugbyLogGroups { get; set; }
        public DbSet<RugbyMatchEvent> RugbyMatchEvents { get; set; }
        public DbSet<RugbyMatchStatistics> RugbyMatchStatistics { get; set; }
        public DbSet<RugbyPlayer> RugbyPlayers { get; set; }
        public DbSet<RugbyPlayerLineup> RugbyPlayerLineups { get; set; }
        public DbSet<RugbySeason> RugbySeasons { get; set; }
        public DbSet<RugbyTeam> RugbyTeams { get; set; }
        public DbSet<RugbyTournament> RugbyTournaments { get; set; }
        public DbSet<RugbyVenue> RugbyVenues { get; set; }
        public DbSet<MotorDriver> MotorDrivers { get; set; }
        public DbSet<MotorCar> MotorCar { get; set; }
        public DbSet<MotorRace> MotorRaces { get; set; }


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