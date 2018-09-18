using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;

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

            ApplyTennisConfiguration(modelBuilder);
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
            modelBuilder.Entity<RugbyLogGroup>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<RugbyLogGroup>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<RugbyLogGroup>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<RugbyMatchEvent>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<RugbyMatchStatistics>().HasKey(x => new { x.RugbyFixtureId, x.RugbyTeamId });

            modelBuilder.Entity<RugbyPlayer>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.LegacyPlayerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.ProviderPlayerId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderPlayerId")));
            modelBuilder.Entity<RugbyPlayer>().Property(x => x.FullName).IsRequired();

            modelBuilder.Entity<RugbyPlayerLineup>().HasKey(x => new { x.RugbyFixtureId, x.RugbyTeamId, x.RugbyPlayerId });
            modelBuilder.Entity<RugbyPlayerStatistics>().HasKey(x => new { x.RugbyTournamentId, x.RugbySeasonId, x.RugbyTeamId, x.RugbyPlayerId });

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
            modelBuilder.Entity<MotorsportDriver>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportDriver>().Property(x => x.LegacyDriverId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportDriver>().Property(x => x.ProviderDriverId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderDriverId")));
            modelBuilder.Entity<MotorsportDriver>().HasRequired(x => x.MotorsportLeague);

            modelBuilder.Entity<MotorsportDriverStanding>().HasKey(x => new {x.MotorsportLeagueId, x.MotorsportSeasonId, x.MotorsportDriverId });
            modelBuilder.Entity<MotorsportDriverStanding>().HasOptional(x => x.MotorsportTeam);

            modelBuilder.Entity<MotorsportLeague>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.LegacyLeagueId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.ProviderLeagueId).IsRequired();
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.ProviderSlug).IsRequired();
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<MotorsportLeague>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<MotorsportRace>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportRace>().Property(x => x.LegacyRaceId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportRace>().Property(x => x.ProviderRaceId).IsRequired();
            modelBuilder.Entity<MotorsportRace>().Property(x => x.RaceName).IsRequired();

            modelBuilder.Entity<MotorsportRaceEvent>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportRaceEvent>().Property(x => x.LegacyRaceEventId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportRaceEvent>().Property(x => x.ProviderRaceEventId).IsRequired();
            modelBuilder.Entity<MotorsportRaceEvent>().HasRequired(x => x.MotorsportRace);
            modelBuilder.Entity<MotorsportRaceEvent>().HasRequired(x => x.MotorsportSeason);

            modelBuilder.Entity<MotorsportRaceEventGrid>().HasKey(x => new { x.MotorsportRaceEventId, x.MotorsportDriverId });
            modelBuilder.Entity<MotorsportRaceEventGrid>().HasRequired(x => x.MotorsportRaceEvent);

            modelBuilder.Entity<MotorsportRaceEventResult>().HasKey(x => new { x.MotorsportRaceEventId, x.MotorsportDriverId });
            modelBuilder.Entity<MotorsportRaceEventResult>().HasRequired(x => x.MotorsportRaceEvent);

            modelBuilder.Entity<MotorsportSeason>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportSeason>().Property(x => x.ProviderSeasonId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderSeasonId")));

            modelBuilder.Entity<MotorsportTeam>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportTeam>().Property(x => x.LegacyTeamId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MotorsportTeam>().Property(x => x.ProviderTeamId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTeamId")));

            modelBuilder.Entity<MotorsportTeamStanding>().HasKey(x => new { x.MotorsportLeagueId, x.MotorsportSeasonId, x.MotorsportTeamId });
        }

        private static void ApplyTennisConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TennisLeague>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisLeague>().Property(x => x.LegacyLeagueId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisLeague>().Property(x => x.ProviderLeagueId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderLeagueId")));

            modelBuilder.Entity<TennisLeague>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<TennisLeague>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<TennisLeague>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<TennisLeague>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<TennisTournament>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisTournament>().Property(x => x.LegacyTournamentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisTournament>().Property(x => x.ProviderTournamentId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderTournamentId")));

            modelBuilder.Entity<TennisTournament>().Property(x => x.ProviderDisplayName).IsRequired();
            modelBuilder.Entity<TennisTournament>().Property(x => x.Slug).IsRequired();
            modelBuilder.Entity<TennisTournament>().Property(x => x.Slug).HasMaxLength(450);
            modelBuilder.Entity<TennisTournament>().Property(x => x.Slug).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Unique_Slug") { IsUnique = true }));

            modelBuilder.Entity<TennisSeason>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisSeason>().Property(x => x.ProviderSeasonId).IsRequired();
            modelBuilder.Entity<TennisSeason>().Property(x => x.ProviderSeasonId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderSeasonId")));

            modelBuilder.Entity<TennisSurfaceType>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TennisSurfaceType>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<TennisSurfaceType>().Property(x => x.Name).HasMaxLength(450);
            modelBuilder.Entity<TennisSurfaceType>().Property(x => x.ProviderSurfaceId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderSurfaceId"){ IsUnique = true }));

            modelBuilder.Entity<TennisVenue>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TennisVenue>().Property(x => x.LegacyVenueId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TennisVenue>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<TennisVenue>().Property(x => x.Name).HasMaxLength(450);
            modelBuilder.Entity<TennisVenue>().Property(x => x.ProviderVenueId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderVenueId") { IsUnique = true }));
            modelBuilder.Entity<TennisVenue>().Property(x => x.LegacyVenueId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_LegacyVenueId") { IsUnique = true }));

            modelBuilder.Entity<TennisCountry>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TennisCountry>().Property(x => x.ProviderCountryId).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderCountryId") { IsUnique = true }));

            modelBuilder.Entity<TennisPlayer>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisPlayer>().Property(x => x.LegacyPlayerId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisPlayer>().Property(x => x.ProviderPlayerId).IsRequired();
            modelBuilder.Entity<TennisPlayer>().Property(x => x.ProviderPlayerId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderPlayerId")));

            modelBuilder.Entity<TennisPlayer>().Property(x => x.LegacyPlayerId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_LegacyPlayerId")));

            modelBuilder.Entity<TennisEvent>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisEvent>().Property(x => x.LegacyEventId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisEvent>().Property(x => x.ProviderEventId).IsRequired();

            modelBuilder.Entity<TennisEvent>().Property(x => x.ProviderEventId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_ProviderEventId")));

            modelBuilder.Entity<TennisEvent>().Property(x => x.LegacyEventId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_LegacyEventId")));

            modelBuilder.Entity<TennisEvent>().Property(x => x.EventName).IsRequired();
            modelBuilder.Entity<TennisEvent>().Property(x => x.EventName).HasMaxLength(450);

            modelBuilder.Entity<TennisEventTennisLeagues>().HasKey(x => new { x.TennisLeagueId, x.TennisEventId });

            modelBuilder.Entity<TennisEventTennisLeagues>().Property(x => x.Prize).HasMaxLength(450);

            modelBuilder.Entity<TennisRanking>().HasKey(x => new { x.TennisPlayerId, x.TennisSeasonId, x.TennisLeagueId, x.TennisRankingType });

            modelBuilder.Entity<TennisMatch>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisMatch>().Property(x => x.LegacyMatchId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisMatch>().Property(x => x.LegacyMatchId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Seek_LegacyMatchId")));

            modelBuilder.Entity<TennisMatch>().Property(x => x.ProviderMatchId).IsRequired();

            modelBuilder.Entity<TennisEventSeed>().HasKey(x => new { x.TennisPlayerId, x.TennisEventId });

            modelBuilder.Entity<TennisSide>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TennisSet>().Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}