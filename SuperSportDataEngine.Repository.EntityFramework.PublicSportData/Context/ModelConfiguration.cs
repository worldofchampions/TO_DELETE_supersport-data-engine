namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Annotations;

    internal static class ModelConfiguration
    {
        internal static void ApplyFluentApiConfigurations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataProvider>().Property(x => x.Code).IsRequired();
            modelBuilder.Entity<DataProvider>().Property(x => x.Code).HasMaxLength(450);
            modelBuilder.Entity<DataProvider>().Property(x => x.Code).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Code") { IsUnique = true }));
            modelBuilder.Entity<DataProvider>().Property(x => x.Name).IsRequired();

            modelBuilder.Entity<Log>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Player>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<RugbySeason>().Property(x => x.ProviderSeasonId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderSeasonId")));

            modelBuilder.Entity<RugbyTournament>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<RugbyTournament>().Property(x => x.LegacyTournamentId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_LegacyTournamentId") { IsUnique = true }));
            modelBuilder.Entity<RugbyTournament>().Property(x => x.ProviderTournamentId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTournamentId")));
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<Sport>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}