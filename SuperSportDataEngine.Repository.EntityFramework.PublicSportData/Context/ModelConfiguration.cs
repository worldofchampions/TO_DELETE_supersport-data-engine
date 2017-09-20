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
            modelBuilder.Entity<RugbyFixture>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyFixture>().Property(x => x.LegacyFixtureId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_LegacyFixtureId") { IsUnique = true }));
            modelBuilder.Entity<RugbyFixture>().Property(x => x.ProviderFixtureId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderFixtureId")));

            modelBuilder.Entity<RugbyLog>().HasKey(x => new { x.RugbyTournamentId, x.RugbySeasonId, x.RoundNumber, x.RugbyTeamId });

            modelBuilder.Entity<RugbyPlayer>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.ProviderPlayerId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderPlayerId")));
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.FullName).IsRequired();

            modelBuilder.Entity<RugbyPlayerLineup>().HasKey(x => new { x.RugbyFixtureId, x.RugbyTeamId, x.RugbyPlayerId});

            modelBuilder.Entity<RugbySeason>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbySeason>().Property(x => x.ProviderSeasonId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderSeasonId")));

            modelBuilder.Entity<RugbyTeam>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyTeam>().Property(x => x.LegacyTeamId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_LegacyTeamId") { IsUnique = true }));
            modelBuilder.Entity<RugbyTeam>().Property(x => x.ProviderTeamId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTeamId")));
            modelBuilder.Entity<RugbyTeam>().Property(x => x.Name).IsRequired();

            modelBuilder.Entity<RugbyTournament>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<RugbyTournament>().Property(x => x.LegacyTournamentId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_LegacyTournamentId") { IsUnique = true }));
            modelBuilder.Entity<RugbyTournament>().Property(x => x.ProviderTournamentId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTournamentId")));
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<RugbyVenue>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyVenue>().Property(x => x.ProviderVenueId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderVenueId")));
            modelBuilder.Entity<RugbyVenue>().Property(x => x.Name).IsRequired();
        }
    }
}