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
            ApplyRugbyConfiguration(modelBuilder);

            ApplyMotorSportConfiguration(modelBuilder);
        }

        private static void ApplyRugbyConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RugbyCommentary>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyCommentary>().Property(x => x.GameTimeDisplayHoursMinutesSeconds).IsRequired();
            modelBuilder.Entity<RugbyCommentary>().Property(x => x.GameTimeDisplayMinutesSeconds).IsRequired();
            modelBuilder.Entity<RugbyCommentary>().Property(x => x.CommentaryText).IsRequired();

            modelBuilder.Entity<RugbyEventType>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyEventType>().Property(x => x.EventCode).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_EventCode") { IsUnique = true }));
            modelBuilder.Entity<RugbyEventType>().Property(x => x.EventName).IsRequired();

            modelBuilder.Entity<RugbyEventTypeProviderMapping>().HasKey(x => new { x.DataProvider, x.ProviderEventTypeId });
            modelBuilder.Entity<RugbyEventTypeProviderMapping>().Property(x => x.ProviderEventName).IsRequired();

            modelBuilder.Entity<RugbyFixture>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyFixture>().Property(x => x.LegacyFixtureId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyFixture>().Property(x => x.ProviderFixtureId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderFixtureId")));

            modelBuilder.Entity<RugbyFlatLog>().HasKey(x => new { x.RugbyTournamentId, x.RugbySeasonId, x.RoundNumber, x.RugbyTeamId });

            modelBuilder.Entity<RugbyGroupedLog>().HasKey(x => new { x.RugbyTournamentId, x.RugbySeasonId, x.RoundNumber, x.RugbyTeamId, x.RugbyLogGroupId });

            modelBuilder.Entity<RugbyLogGroup>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyLogGroup>().Property(x => x.ProviderLogGroupId).IsOptional();

            modelBuilder.Entity<RugbyMatchEvent>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<RugbyMatchStatistics>().HasKey(x => new { x.RugbyFixtureId, x.RugbyTeamId });

            modelBuilder.Entity<RugbyPlayer>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.LegacyPlayerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.ProviderPlayerId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderPlayerId")));
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.FullName).IsRequired();

            modelBuilder.Entity<RugbyPlayerLineup>().HasKey(x => new { x.RugbyFixtureId, x.RugbyTeamId, x.RugbyPlayerId });

            modelBuilder.Entity<RugbySeason>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbySeason>().Property(x => x.ProviderSeasonId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderSeasonId")));
            modelBuilder.Entity<RugbySeason>().Property(x => x.RugbyLogType).IsOptional();

            modelBuilder.Entity<RugbyTeam>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyTeam>().Property(x => x.LegacyTeamId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyTeam>().Property(x => x.ProviderTeamId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTeamId")));
            modelBuilder.Entity<RugbyTeam>().Property(x => x.Name).IsRequired();

            modelBuilder.Entity<RugbyTournament>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyTournament>().Property(x => x.LegacyTournamentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyTournament>().Property(x => x.ProviderTournamentId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTournamentId")));
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<RugbyTournament>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<RugbyVenue>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyVenue>().Property(x => x.ProviderVenueId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderVenueId")));
            modelBuilder.Entity<RugbyVenue>().Property(x => x.Name).IsRequired();
        }

        private static void ApplyMotorSportConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MotorLeague>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorLeague>().Property(x => x.LegacyLeagueId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorLeague>().Property(x => x.ProviderLeagueId).IsRequired();
            modelBuilder.Entity<MotorLeague>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<MotorLeague>().Property(x => x.ProviderSlug).IsRequired();
            modelBuilder.Entity<MotorLeague>().Property(x => x.Slug).IsRequired();

            modelBuilder.Entity<MotorGrid>().HasKey(x => new { x.RaceId, x.DriverId});
        }
    }
}