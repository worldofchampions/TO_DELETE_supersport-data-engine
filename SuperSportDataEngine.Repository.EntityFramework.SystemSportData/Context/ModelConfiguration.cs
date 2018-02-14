using System.Data.Entity.Infrastructure.Annotations;

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
            modelBuilder.Entity<LegacyAuthFeedConsumer>().Property(x => x.AuthKey).IsRequired();
            modelBuilder.Entity<LegacyAuthFeedConsumer>().Property(x => x.AuthKey).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_AuthKey")));
            modelBuilder.Entity<LegacyAuthFeedConsumer>().Property(x => x.AuthKey).HasMaxLength(50);

            modelBuilder.Entity<LegacyMethodAccess>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyZoneSite>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<SchedulerTrackingRugbyFixture>().HasKey(x => new { x.FixtureId, x.TournamentId });

            modelBuilder.Entity<SchedulerTrackingRugbySeason>().HasKey(x => new { x.SeasonId, x.TournamentId });

            modelBuilder.Entity<SchedulerTrackingRugbyTournament>().HasKey(x => new { x.TournamentId, x.SeasonId });

            modelBuilder.Entity<SchedulerDashboardUser>().HasKey(x => new { x.Username });
        }
    }
}