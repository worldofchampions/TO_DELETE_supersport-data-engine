namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    internal static class ModelConfiguration
    {
        internal static void ApplyFluentApiConfigurations(DbModelBuilder modelBuilder)
        {
#region LegacyEF
            modelBuilder.Entity<LegacyAuthFeedConsumer>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyAccessItem>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyMethodAccess>()
                            .Property(x => x.Id)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LegacyZoneSite>()
                            .Property(x => x.Id)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            #endregion LegacyEF

            modelBuilder.Entity<Player>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Sport>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}