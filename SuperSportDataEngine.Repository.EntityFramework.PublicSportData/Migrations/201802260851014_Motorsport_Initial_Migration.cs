namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Initial_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MotorsportDrivers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyDriverId = c.Int(nullable: false, identity: true),
                        ProviderDriverId = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        FullName = c.String(),
                        FullNameCmsOverride = c.String(),
                        HeightInCentimeters = c.Double(nullable: false),
                        WeightInKilograms = c.Double(nullable: false),
                        CountryName = c.String(),
                        ProviderCarId = c.Int(nullable: false),
                        CarNumber = c.Int(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderDriverId, name: "Seek_ProviderDriverId");
            
            CreateTable(
                "dbo.MotorsportDriverStandings",
                c => new
                    {
                        MotorsportLeagueId = c.Guid(nullable: false),
                        MotorsportSeasonId = c.Guid(nullable: false),
                        MotorsportTeamId = c.Guid(nullable: false),
                        MotorsportDriverId = c.Guid(nullable: false),
                        Points = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.MotorsportLeagueId, t.MotorsportSeasonId, t.MotorsportTeamId, t.MotorsportDriverId })
                .ForeignKey("dbo.MotorsportDrivers", t => t.MotorsportDriverId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeagueId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeamId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportSeasons", t => t.MotorsportSeasonId, cascadeDelete: true)
                .Index(t => t.MotorsportLeagueId)
                .Index(t => t.MotorsportSeasonId)
                .Index(t => t.MotorsportTeamId)
                .Index(t => t.MotorsportDriverId);
            
            CreateTable(
                "dbo.MotorsportLeagues",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyLeagueId = c.Int(nullable: false, identity: true),
                        ProviderLeagueId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        NameCmsOverride = c.String(),
                        Slug = c.String(nullable: false, maxLength: 450),
                        ProviderSlug = c.String(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        MotorsportSportType = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Slug, unique: true, name: "Unique_Slug");
            
            CreateTable(
                "dbo.MotorsportTeams",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyTeamId = c.Int(nullable: false),
                        ProviderTeamId = c.Int(nullable: false),
                        Name = c.String(),
                        NameCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderTeamId, name: "Seek_ProviderTeamId");
            
            CreateTable(
                "dbo.MotorsportSeasons",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderSeasonId = c.Int(nullable: false),
                        Name = c.String(),
                        StartDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        IsActive = c.Boolean(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportLeague_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeague_Id)
                .Index(t => t.ProviderSeasonId, name: "Seek_ProviderSeasonId")
                .Index(t => t.MotorsportLeague_Id);
            
            CreateTable(
                "dbo.MotorsportRaceGrids",
                c => new
                    {
                        MotorsportRaceId = c.Guid(nullable: false),
                        MotorsportDriverId = c.Guid(nullable: false),
                        MotorsportTeamId = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        QualifyingTimeHours = c.Int(nullable: false),
                        QualifyingTimeMinutes = c.Int(nullable: false),
                        QualifyingTimeSeconds = c.Int(nullable: false),
                        QualifyingTimeMilliseconds = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.MotorsportRaceId, t.MotorsportDriverId })
                .ForeignKey("dbo.MotorsportDrivers", t => t.MotorsportDriverId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportRaces", t => t.MotorsportRaceId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeamId, cascadeDelete: true)
                .Index(t => t.MotorsportRaceId)
                .Index(t => t.MotorsportDriverId)
                .Index(t => t.MotorsportTeamId);
            
            CreateTable(
                "dbo.MotorsportRaces",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyRaceId = c.Int(nullable: false, identity: true),
                        ProviderRaceId = c.Int(nullable: false),
                        MotorsportRaceStatus = c.Int(nullable: false),
                        RaceName = c.String(nullable: false),
                        RaceNameCmsOverride = c.String(),
                        RaceNameAbbreviation = c.String(),
                        RaceNameAbbreviationCmsOverride = c.String(),
                        IsDisabledOutbound = c.Boolean(nullable: false),
                        IsDisabledInbound = c.Boolean(nullable: false),
                        IsLiveScored = c.Boolean(nullable: false),
                        CmsOverrideModeIsActive = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportLeague_Id = c.Guid(),
                        MotorsportRaceResult_MotorsportRaceId = c.Guid(),
                        MotorsportRaceResult_MotorsportDriverId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeague_Id)
                .ForeignKey("dbo.MotorsportRaceResults", t => new { t.MotorsportRaceResult_MotorsportRaceId, t.MotorsportRaceResult_MotorsportDriverId })
                .Index(t => t.MotorsportLeague_Id)
                .Index(t => new { t.MotorsportRaceResult_MotorsportRaceId, t.MotorsportRaceResult_MotorsportDriverId });
            
            CreateTable(
                "dbo.MotorsportRaceResults",
                c => new
                    {
                        MotorsportRaceId = c.Guid(nullable: false),
                        MotorsportDriverId = c.Guid(nullable: false),
                        CircuitName = c.String(),
                        Position = c.Int(nullable: false),
                        LapsCompleted = c.Int(nullable: false),
                        CompletedRace = c.Boolean(nullable: false),
                        OutReason = c.String(),
                        GridPosition = c.Int(nullable: false),
                        FinishingTimeHours = c.Int(nullable: false),
                        FinishingTimeMinutes = c.Int(nullable: false),
                        FinishingTimeSeconds = c.Int(nullable: false),
                        FinishingTimeMilliseconds = c.Int(nullable: false),
                        DriverTotalPoints = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportTeam_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.MotorsportRaceId, t.MotorsportDriverId })
                .ForeignKey("dbo.MotorsportDrivers", t => t.MotorsportDriverId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeam_Id)
                .Index(t => t.MotorsportDriverId)
                .Index(t => t.MotorsportTeam_Id);
            
            CreateTable(
                "dbo.MotorsportRaceCalendars",
                c => new
                    {
                        MotorsportRaceId = c.Guid(nullable: false),
                        MotorsportSeasonId = c.Guid(nullable: false),
                        StartDateTimeUtc = c.DateTimeOffset(precision: 7),
                        EndDateTimeUtc = c.DateTimeOffset(precision: 7),
                        CountryName = c.String(),
                        CountryAbbreviation = c.String(),
                        CityName = c.String(),
                        VenueName = c.String(),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.MotorsportRaceId, t.MotorsportSeasonId })
                .ForeignKey("dbo.MotorsportRaces", t => t.MotorsportRaceId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportSeasons", t => t.MotorsportSeasonId, cascadeDelete: true)
                .Index(t => t.MotorsportRaceId)
                .Index(t => t.MotorsportSeasonId);
            
            CreateTable(
                "dbo.MotorsportTeamStandings",
                c => new
                    {
                        MotorsportLeagueId = c.Guid(nullable: false),
                        MotorsportSeasonId = c.Guid(nullable: false),
                        MotorsportTeamId = c.Guid(nullable: false),
                        Points = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.MotorsportLeagueId, t.MotorsportSeasonId, t.MotorsportTeamId })
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeagueId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportSeasons", t => t.MotorsportSeasonId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeamId, cascadeDelete: true)
                .Index(t => t.MotorsportLeagueId)
                .Index(t => t.MotorsportSeasonId)
                .Index(t => t.MotorsportTeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportTeamStandings", "MotorsportTeamId", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportTeamStandings", "MotorsportSeasonId", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportTeamStandings", "MotorsportLeagueId", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportRaceCalendars", "MotorsportSeasonId", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportRaceCalendars", "MotorsportRaceId", "dbo.MotorsportRaces");
            DropForeignKey("dbo.MotorsportRaceGrids", "MotorsportTeamId", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportRaceGrids", "MotorsportRaceId", "dbo.MotorsportRaces");
            DropForeignKey("dbo.MotorsportRaces", new[] { "MotorsportRaceResult_MotorsportRaceId", "MotorsportRaceResult_MotorsportDriverId" }, "dbo.MotorsportRaceResults");
            DropForeignKey("dbo.MotorsportRaceResults", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportRaceResults", "MotorsportDriverId", "dbo.MotorsportDrivers");
            DropForeignKey("dbo.MotorsportRaces", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportRaceGrids", "MotorsportDriverId", "dbo.MotorsportDrivers");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportSeasonId", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportSeasons", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportTeamId", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportLeagueId", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportDriverId", "dbo.MotorsportDrivers");
            DropIndex("dbo.MotorsportTeamStandings", new[] { "MotorsportTeamId" });
            DropIndex("dbo.MotorsportTeamStandings", new[] { "MotorsportSeasonId" });
            DropIndex("dbo.MotorsportTeamStandings", new[] { "MotorsportLeagueId" });
            DropIndex("dbo.MotorsportRaceCalendars", new[] { "MotorsportSeasonId" });
            DropIndex("dbo.MotorsportRaceCalendars", new[] { "MotorsportRaceId" });
            DropIndex("dbo.MotorsportRaceResults", new[] { "MotorsportTeam_Id" });
            DropIndex("dbo.MotorsportRaceResults", new[] { "MotorsportDriverId" });
            DropIndex("dbo.MotorsportRaces", new[] { "MotorsportRaceResult_MotorsportRaceId", "MotorsportRaceResult_MotorsportDriverId" });
            DropIndex("dbo.MotorsportRaces", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportRaceGrids", new[] { "MotorsportTeamId" });
            DropIndex("dbo.MotorsportRaceGrids", new[] { "MotorsportDriverId" });
            DropIndex("dbo.MotorsportRaceGrids", new[] { "MotorsportRaceId" });
            DropIndex("dbo.MotorsportSeasons", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportSeasons", "Seek_ProviderSeasonId");
            DropIndex("dbo.MotorsportTeams", "Seek_ProviderTeamId");
            DropIndex("dbo.MotorsportLeagues", "Unique_Slug");
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportDriverId" });
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportTeamId" });
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportSeasonId" });
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportLeagueId" });
            DropIndex("dbo.MotorsportDrivers", "Seek_ProviderDriverId");
            DropTable("dbo.MotorsportTeamStandings");
            DropTable("dbo.MotorsportRaceCalendars");
            DropTable("dbo.MotorsportRaceResults");
            DropTable("dbo.MotorsportRaces");
            DropTable("dbo.MotorsportRaceGrids");
            DropTable("dbo.MotorsportSeasons");
            DropTable("dbo.MotorsportTeams");
            DropTable("dbo.MotorsportLeagues");
            DropTable("dbo.MotorsportDriverStandings");
            DropTable("dbo.MotorsportDrivers");
        }
    }
}
