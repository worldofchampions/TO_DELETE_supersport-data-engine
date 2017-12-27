namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    internal static class ModelConfiguration
    {
        internal static void ApplyFluentApiConfigurations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LegacyAccessItem>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyAuthFeedConsumer>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyMethodAccess>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyZoneSite>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<SchedulerTrackingRugbyFixture>().HasKey(x => new { x.FixtureId, x.TournamentId });

            modelBuilder.Entity<SchedulerTrackingRugbySeason>().HasKey(x => new { x.SeasonId, x.TournamentId });

            modelBuilder.Entity<SchedulerTrackingRugbyTournament>().HasKey(x => new { x.TournamentId, x.SeasonId });

            modelBuilder.Entity<SchedulerDashboardUser>().HasKey(x => new { x.Username });

            ApplyMotorSportConfigurations(modelBuilder);
        }

        private static void ApplyMotorSportConfigurations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchedulerTrackingMotorSeason>().HasKey(x => new { SeasoId = x.SeasonId, x.LeagueId });
        }
    }
}