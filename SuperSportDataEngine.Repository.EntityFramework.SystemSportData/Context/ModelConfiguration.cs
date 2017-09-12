namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    internal static class ModelConfiguration
    {
        internal static void ApplyFluentApiConfigurations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchedulerTrackingRugbyFixture>().HasKey(x => new { x.FixtureId, x.TournamentId });

            modelBuilder.Entity<SchedulerTrackingRugbyTournament>().HasKey(x => new { x.TournamentId, x.SeasonId });

            modelBuilder.Entity<SportTournament>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyAccessItem>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyAuthFeedConsumer>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyMethodAccess>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyZoneSite>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}