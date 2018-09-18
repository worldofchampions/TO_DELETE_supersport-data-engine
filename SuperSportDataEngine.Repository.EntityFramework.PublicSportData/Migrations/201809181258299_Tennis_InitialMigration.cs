namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tennis_InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TennisEvents",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyEventId = c.Int(nullable: false, identity: true),
                        ProviderEventId = c.Int(nullable: false),
                        EventName = c.String(nullable: false, maxLength: 450),
                        EventNameCmsOverride = c.String(),
                        StartDateUtc = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDateUtc = c.DateTimeOffset(nullable: false, precision: 7),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        TennisSeason_Id = c.Guid(),
                        TennisSurfaceType_Id = c.Guid(),
                        TennisTournament_Id = c.Guid(),
                        TennisVenue_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisSeasons", t => t.TennisSeason_Id)
                .ForeignKey("dbo.TennisSurfaceTypes", t => t.TennisSurfaceType_Id)
                .ForeignKey("dbo.TennisTournaments", t => t.TennisTournament_Id)
                .ForeignKey("dbo.TennisVenues", t => t.TennisVenue_Id)
                .Index(t => t.LegacyEventId, name: "Seek_LegacyEventId")
                .Index(t => t.ProviderEventId, name: "Seek_ProviderEventId")
                .Index(t => t.TennisSeason_Id)
                .Index(t => t.TennisSurfaceType_Id)
                .Index(t => t.TennisTournament_Id)
                .Index(t => t.TennisVenue_Id);
            
            CreateTable(
                "dbo.TennisSeasons",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderSeasonId = c.Int(nullable: false),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        StartDateUtc = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDateUtc = c.DateTimeOffset(nullable: false, precision: 7),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        TennisLeague_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisLeagues", t => t.TennisLeague_Id)
                .Index(t => t.ProviderSeasonId, name: "Seek_ProviderSeasonId")
                .Index(t => t.TennisLeague_Id);
            
            CreateTable(
                "dbo.TennisLeagues",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyLeagueId = c.Int(nullable: false, identity: true),
                        ProviderLeagueId = c.Int(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 450),
                        ProviderSlug = c.String(),
                        Name = c.String(nullable: false),
                        NameCmsOverride = c.String(),
                        Abbreviation = c.String(),
                        IsDisabledInbound = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderLeagueId, name: "Seek_ProviderLeagueId")
                .Index(t => t.Slug, unique: true, name: "Unique_Slug");
            
            CreateTable(
                "dbo.TennisTournaments",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyTournamentId = c.Int(nullable: false, identity: true),
                        ProviderTournamentId = c.Int(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 450),
                        ProviderTournamentName = c.String(),
                        ProviderDisplayName = c.String(nullable: false),
                        NameCmsOverride = c.String(),
                        Abbreviation = c.String(),
                        TennisTournamentType = c.Int(nullable: false),
                        IsDisabledInbound = c.Boolean(nullable: false),
                        IsDisabledOutbound = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderTournamentId, name: "Seek_ProviderTournamentId")
                .Index(t => t.Slug, unique: true, name: "Unique_Slug");
            
            CreateTable(
                "dbo.TennisSurfaceTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderSurfaceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 450),
                        NameCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderSurfaceId, unique: true, name: "Seek_ProviderSurfaceId");
            
            CreateTable(
                "dbo.TennisVenues",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyVenueId = c.Int(nullable: false, identity: true),
                        ProviderVenueId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 450),
                        NameCmsOverride = c.String(),
                        City = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        TennisCountry_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisCountries", t => t.TennisCountry_Id)
                .Index(t => t.LegacyVenueId, unique: true, name: "Seek_LegacyVenueId")
                .Index(t => t.ProviderVenueId, unique: true, name: "Seek_ProviderVenueId")
                .Index(t => t.TennisCountry_Id);
            
            CreateTable(
                "dbo.TennisCountries",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderCountryId = c.Int(nullable: false),
                        Country = c.String(),
                        CountryAbbreviation = c.String(),
                        CountryLogoUrl = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderCountryId, unique: true, name: "Seek_ProviderCountryId");
            
            CreateTable(
                "dbo.TennisEventSeeds",
                c => new
                    {
                        TennisPlayerId = c.Guid(nullable: false),
                        TennisEventId = c.Guid(nullable: false),
                        SeedValue = c.Int(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.TennisPlayerId, t.TennisEventId })
                .ForeignKey("dbo.TennisEvents", t => t.TennisEventId, cascadeDelete: true)
                .ForeignKey("dbo.TennisPlayers", t => t.TennisPlayerId, cascadeDelete: true)
                .Index(t => t.TennisPlayerId)
                .Index(t => t.TennisEventId);
            
            CreateTable(
                "dbo.TennisPlayers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyPlayerId = c.Int(nullable: false, identity: true),
                        ProviderPlayerId = c.Int(nullable: false),
                        FirstName = c.String(),
                        FirstNameCmsOverride = c.String(),
                        LastName = c.String(),
                        LastNameCmsOverride = c.String(),
                        Handedness = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        TennisCountry_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisCountries", t => t.TennisCountry_Id)
                .Index(t => t.LegacyPlayerId, name: "Seek_LegacyPlayerId")
                .Index(t => t.ProviderPlayerId, name: "Seek_ProviderPlayerId")
                .Index(t => t.TennisCountry_Id);
            
            CreateTable(
                "dbo.TennisEventTennisLeagues",
                c => new
                    {
                        TennisLeagueId = c.Guid(nullable: false),
                        TennisEventId = c.Guid(nullable: false),
                        Prize = c.String(maxLength: 450),
                        PrizeCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.TennisLeagueId, t.TennisEventId })
                .ForeignKey("dbo.TennisEvents", t => t.TennisEventId, cascadeDelete: true)
                .ForeignKey("dbo.TennisLeagues", t => t.TennisLeagueId, cascadeDelete: true)
                .Index(t => t.TennisLeagueId)
                .Index(t => t.TennisEventId);
            
            CreateTable(
                "dbo.TennisMatches",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyMatchId = c.Int(nullable: false, identity: true),
                        ProviderMatchId = c.Int(nullable: false),
                        RoundNumber = c.Int(nullable: false),
                        RoundType = c.String(),
                        NumberOfSets = c.Int(nullable: false),
                        DrawNumber = c.Int(nullable: false),
                        TennisMatchStatus = c.Int(nullable: false),
                        StartDateTimeUtc = c.DateTimeOffset(nullable: false, precision: 7),
                        MakeupDateTimeUtc = c.DateTimeOffset(precision: 7),
                        CourtName = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        AssociatedTennisEventTennisLeague_TennisLeagueId = c.Guid(),
                        AssociatedTennisEventTennisLeague_TennisEventId = c.Guid(),
                        TennisSideA_Id = c.Guid(),
                        TennisSideB_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisEventTennisLeagues", t => new { t.AssociatedTennisEventTennisLeague_TennisLeagueId, t.AssociatedTennisEventTennisLeague_TennisEventId })
                .ForeignKey("dbo.TennisSides", t => t.TennisSideA_Id)
                .ForeignKey("dbo.TennisSides", t => t.TennisSideB_Id)
                .Index(t => t.LegacyMatchId, name: "Seek_LegacyMatchId")
                .Index(t => new { t.AssociatedTennisEventTennisLeague_TennisLeagueId, t.AssociatedTennisEventTennisLeague_TennisEventId })
                .Index(t => t.TennisSideA_Id)
                .Index(t => t.TennisSideB_Id);
            
            CreateTable(
                "dbo.TennisSets",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SetNumber = c.Int(nullable: false),
                        SideAHasWon = c.Boolean(nullable: false),
                        SideBHasWon = c.Boolean(nullable: false),
                        SideAGamesWon = c.Int(nullable: false),
                        SideBGamesWon = c.Int(nullable: false),
                        SetIsTieBreaker = c.Boolean(nullable: false),
                        SideATieBreakerPoints = c.Int(),
                        SideBTieBreakerPoints = c.Int(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        TennisSideA_Id = c.Guid(),
                        TennisSideB_Id = c.Guid(),
                        TennisMatch_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisSides", t => t.TennisSideA_Id)
                .ForeignKey("dbo.TennisSides", t => t.TennisSideB_Id)
                .ForeignKey("dbo.TennisMatches", t => t.TennisMatch_Id)
                .Index(t => t.TennisSideA_Id)
                .Index(t => t.TennisSideB_Id)
                .Index(t => t.TennisMatch_Id);
            
            CreateTable(
                "dbo.TennisSides",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SideNumber = c.Int(nullable: false),
                        HasSideWon = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        SideStatus = c.String(),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        TennisPlayerA_Id = c.Guid(),
                        TennisPlayerB_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisPlayers", t => t.TennisPlayerA_Id)
                .ForeignKey("dbo.TennisPlayers", t => t.TennisPlayerB_Id)
                .Index(t => t.TennisPlayerA_Id)
                .Index(t => t.TennisPlayerB_Id);
            
            CreateTable(
                "dbo.TennisRankings",
                c => new
                    {
                        TennisPlayerId = c.Guid(nullable: false),
                        TennisSeasonId = c.Guid(nullable: false),
                        TennisLeagueId = c.Guid(nullable: false),
                        TennisRankingType = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        Movement = c.Int(),
                        RankingValidLastAt = c.DateTimeOffset(nullable: false, precision: 7),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.TennisPlayerId, t.TennisSeasonId, t.TennisLeagueId, t.TennisRankingType })
                .ForeignKey("dbo.TennisLeagues", t => t.TennisLeagueId, cascadeDelete: true)
                .ForeignKey("dbo.TennisPlayers", t => t.TennisPlayerId, cascadeDelete: true)
                .ForeignKey("dbo.TennisSeasons", t => t.TennisSeasonId, cascadeDelete: true)
                .Index(t => t.TennisPlayerId)
                .Index(t => t.TennisSeasonId)
                .Index(t => t.TennisLeagueId);
            
            CreateTable(
                "dbo.TennisTournamentTennisLeagues",
                c => new
                    {
                        TennisTournament_Id = c.Guid(nullable: false),
                        TennisLeague_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.TennisTournament_Id, t.TennisLeague_Id })
                .ForeignKey("dbo.TennisTournaments", t => t.TennisTournament_Id, cascadeDelete: true)
                .ForeignKey("dbo.TennisLeagues", t => t.TennisLeague_Id, cascadeDelete: true)
                .Index(t => t.TennisTournament_Id)
                .Index(t => t.TennisLeague_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TennisRankings", "TennisSeasonId", "dbo.TennisSeasons");
            DropForeignKey("dbo.TennisRankings", "TennisPlayerId", "dbo.TennisPlayers");
            DropForeignKey("dbo.TennisRankings", "TennisLeagueId", "dbo.TennisLeagues");
            DropForeignKey("dbo.TennisMatches", "TennisSideB_Id", "dbo.TennisSides");
            DropForeignKey("dbo.TennisMatches", "TennisSideA_Id", "dbo.TennisSides");
            DropForeignKey("dbo.TennisSets", "TennisMatch_Id", "dbo.TennisMatches");
            DropForeignKey("dbo.TennisSets", "TennisSideB_Id", "dbo.TennisSides");
            DropForeignKey("dbo.TennisSets", "TennisSideA_Id", "dbo.TennisSides");
            DropForeignKey("dbo.TennisSides", "TennisPlayerB_Id", "dbo.TennisPlayers");
            DropForeignKey("dbo.TennisSides", "TennisPlayerA_Id", "dbo.TennisPlayers");
            DropForeignKey("dbo.TennisMatches", new[] { "AssociatedTennisEventTennisLeague_TennisLeagueId", "AssociatedTennisEventTennisLeague_TennisEventId" }, "dbo.TennisEventTennisLeagues");
            DropForeignKey("dbo.TennisEventTennisLeagues", "TennisLeagueId", "dbo.TennisLeagues");
            DropForeignKey("dbo.TennisEventTennisLeagues", "TennisEventId", "dbo.TennisEvents");
            DropForeignKey("dbo.TennisEventSeeds", "TennisPlayerId", "dbo.TennisPlayers");
            DropForeignKey("dbo.TennisPlayers", "TennisCountry_Id", "dbo.TennisCountries");
            DropForeignKey("dbo.TennisEventSeeds", "TennisEventId", "dbo.TennisEvents");
            DropForeignKey("dbo.TennisEvents", "TennisVenue_Id", "dbo.TennisVenues");
            DropForeignKey("dbo.TennisVenues", "TennisCountry_Id", "dbo.TennisCountries");
            DropForeignKey("dbo.TennisEvents", "TennisTournament_Id", "dbo.TennisTournaments");
            DropForeignKey("dbo.TennisEvents", "TennisSurfaceType_Id", "dbo.TennisSurfaceTypes");
            DropForeignKey("dbo.TennisEvents", "TennisSeason_Id", "dbo.TennisSeasons");
            DropForeignKey("dbo.TennisSeasons", "TennisLeague_Id", "dbo.TennisLeagues");
            DropForeignKey("dbo.TennisTournamentTennisLeagues", "TennisLeague_Id", "dbo.TennisLeagues");
            DropForeignKey("dbo.TennisTournamentTennisLeagues", "TennisTournament_Id", "dbo.TennisTournaments");
            DropIndex("dbo.TennisTournamentTennisLeagues", new[] { "TennisLeague_Id" });
            DropIndex("dbo.TennisTournamentTennisLeagues", new[] { "TennisTournament_Id" });
            DropIndex("dbo.TennisRankings", new[] { "TennisLeagueId" });
            DropIndex("dbo.TennisRankings", new[] { "TennisSeasonId" });
            DropIndex("dbo.TennisRankings", new[] { "TennisPlayerId" });
            DropIndex("dbo.TennisSides", new[] { "TennisPlayerB_Id" });
            DropIndex("dbo.TennisSides", new[] { "TennisPlayerA_Id" });
            DropIndex("dbo.TennisSets", new[] { "TennisMatch_Id" });
            DropIndex("dbo.TennisSets", new[] { "TennisSideB_Id" });
            DropIndex("dbo.TennisSets", new[] { "TennisSideA_Id" });
            DropIndex("dbo.TennisMatches", new[] { "TennisSideB_Id" });
            DropIndex("dbo.TennisMatches", new[] { "TennisSideA_Id" });
            DropIndex("dbo.TennisMatches", new[] { "AssociatedTennisEventTennisLeague_TennisLeagueId", "AssociatedTennisEventTennisLeague_TennisEventId" });
            DropIndex("dbo.TennisMatches", "Seek_LegacyMatchId");
            DropIndex("dbo.TennisEventTennisLeagues", new[] { "TennisEventId" });
            DropIndex("dbo.TennisEventTennisLeagues", new[] { "TennisLeagueId" });
            DropIndex("dbo.TennisPlayers", new[] { "TennisCountry_Id" });
            DropIndex("dbo.TennisPlayers", "Seek_ProviderPlayerId");
            DropIndex("dbo.TennisPlayers", "Seek_LegacyPlayerId");
            DropIndex("dbo.TennisEventSeeds", new[] { "TennisEventId" });
            DropIndex("dbo.TennisEventSeeds", new[] { "TennisPlayerId" });
            DropIndex("dbo.TennisCountries", "Seek_ProviderCountryId");
            DropIndex("dbo.TennisVenues", new[] { "TennisCountry_Id" });
            DropIndex("dbo.TennisVenues", "Seek_ProviderVenueId");
            DropIndex("dbo.TennisVenues", "Seek_LegacyVenueId");
            DropIndex("dbo.TennisSurfaceTypes", "Seek_ProviderSurfaceId");
            DropIndex("dbo.TennisTournaments", "Unique_Slug");
            DropIndex("dbo.TennisTournaments", "Seek_ProviderTournamentId");
            DropIndex("dbo.TennisLeagues", "Unique_Slug");
            DropIndex("dbo.TennisLeagues", "Seek_ProviderLeagueId");
            DropIndex("dbo.TennisSeasons", new[] { "TennisLeague_Id" });
            DropIndex("dbo.TennisSeasons", "Seek_ProviderSeasonId");
            DropIndex("dbo.TennisEvents", new[] { "TennisVenue_Id" });
            DropIndex("dbo.TennisEvents", new[] { "TennisTournament_Id" });
            DropIndex("dbo.TennisEvents", new[] { "TennisSurfaceType_Id" });
            DropIndex("dbo.TennisEvents", new[] { "TennisSeason_Id" });
            DropIndex("dbo.TennisEvents", "Seek_ProviderEventId");
            DropIndex("dbo.TennisEvents", "Seek_LegacyEventId");
            DropTable("dbo.TennisTournamentTennisLeagues");
            DropTable("dbo.TennisRankings");
            DropTable("dbo.TennisSides");
            DropTable("dbo.TennisSets");
            DropTable("dbo.TennisMatches");
            DropTable("dbo.TennisEventTennisLeagues");
            DropTable("dbo.TennisPlayers");
            DropTable("dbo.TennisEventSeeds");
            DropTable("dbo.TennisCountries");
            DropTable("dbo.TennisVenues");
            DropTable("dbo.TennisSurfaceTypes");
            DropTable("dbo.TennisTournaments");
            DropTable("dbo.TennisLeagues");
            DropTable("dbo.TennisSeasons");
            DropTable("dbo.TennisEvents");
        }
    }
}
