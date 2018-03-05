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
                        FullNameCmsOverride = c.String(),
                        CountryName = c.String(),
                        ProviderCarId = c.Int(nullable: false),
                        CarNumber = c.Int(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportLeague_Id = c.Guid(nullable: false),
                        MotorsportTeam_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeague_Id, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeam_Id)
                .Index(t => t.ProviderDriverId, name: "Seek_ProviderDriverId")
                .Index(t => t.MotorsportLeague_Id)
                .Index(t => t.MotorsportTeam_Id);
            
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
                        LegacyTeamId = c.Int(nullable: false, identity: true),
                        ProviderTeamId = c.Int(nullable: false),
                        Name = c.String(),
                        NameCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportLeague_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeague_Id, cascadeDelete: true)
                .Index(t => t.ProviderTeamId, name: "Seek_ProviderTeamId")
                .Index(t => t.MotorsportLeague_Id);
            
            CreateTable(
                "dbo.MotorsportDriverStandings",
                c => new
                    {
                        MotorsportLeagueId = c.Guid(nullable: false),
                        MotorsportSeasonId = c.Guid(nullable: false),
                        MotorsportDriverId = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportTeam_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.MotorsportLeagueId, t.MotorsportSeasonId, t.MotorsportDriverId })
                .ForeignKey("dbo.MotorsportDrivers", t => t.MotorsportDriverId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportSeasons", t => t.MotorsportSeasonId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeam_Id)
                .Index(t => t.MotorsportSeasonId)
                .Index(t => t.MotorsportDriverId)
                .Index(t => t.MotorsportTeam_Id);
            
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
                "dbo.MotorsportRaces",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyRaceId = c.Int(nullable: false, identity: true),
                        ProviderRaceId = c.Int(nullable: false),
                        RaceName = c.String(nullable: false),
                        RaceNameCmsOverride = c.String(),
                        RaceNameAbbreviation = c.String(),
                        RaceNameAbbreviationCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportLeague_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorsportLeagues", t => t.MotorsportLeague_Id)
                .Index(t => t.MotorsportLeague_Id);
            
            CreateTable(
                "dbo.MotorsportRaceEvents",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyRaceEventId = c.Int(nullable: false, identity: true),
                        ProviderRaceEventId = c.Int(nullable: false),
                        StartDateTimeUtc = c.DateTimeOffset(precision: 7),
                        CountryName = c.String(),
                        CountryAbbreviation = c.String(),
                        CityName = c.String(),
                        CircuitName = c.String(),
                        MotorsportRaceEventStatus = c.Int(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        IsLiveScored = c.Boolean(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportRace_Id = c.Guid(nullable: false),
                        MotorsportSeason_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorsportRaces", t => t.MotorsportRace_Id, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportSeasons", t => t.MotorsportSeason_Id, cascadeDelete: true)
                .Index(t => t.MotorsportRace_Id)
                .Index(t => t.MotorsportSeason_Id);
            
            CreateTable(
                "dbo.MotorsportRaceEventGrids",
                c => new
                    {
                        MotorsportRaceEventId = c.Guid(nullable: false),
                        MotorsportDriverId = c.Guid(nullable: false),
                        GridPosition = c.Int(nullable: false),
                        QualifyingTimeHours = c.Int(nullable: false),
                        QualifyingTimeMinutes = c.Int(nullable: false),
                        QualifyingTimeSeconds = c.Int(nullable: false),
                        QualifyingTimeMilliseconds = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportTeam_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.MotorsportRaceEventId, t.MotorsportDriverId })
                .ForeignKey("dbo.MotorsportDrivers", t => t.MotorsportDriverId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportRaceEvents", t => t.MotorsportRaceEventId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeam_Id)
                .Index(t => t.MotorsportRaceEventId)
                .Index(t => t.MotorsportDriverId)
                .Index(t => t.MotorsportTeam_Id);
            
            CreateTable(
                "dbo.MotorsportRaceEventResults",
                c => new
                    {
                        MotorsportRaceEventId = c.Guid(nullable: false),
                        MotorsportDriverId = c.Guid(nullable: false),
                        ProviderRaceEventId = c.Int(nullable: false),
                        GridPosition = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        LapsCompleted = c.Int(nullable: false),
                        CompletedRace = c.Boolean(nullable: false),
                        OutReason = c.String(),
                        FinishingTimeHours = c.Int(nullable: false),
                        FinishingTimeMinutes = c.Int(nullable: false),
                        FinishingTimeSeconds = c.Int(nullable: false),
                        FinishingTimeMilliseconds = c.Int(nullable: false),
                        GapToLeaderTimeHours = c.Int(nullable: false),
                        GapToLeaderTimeMinutes = c.Int(nullable: false),
                        GapToLeaderTimeSeconds = c.Int(nullable: false),
                        GapToLeaderTimeMilliseconds = c.Int(nullable: false),
                        GapToCarInFrontTimeHours = c.Int(nullable: false),
                        GapToCarInFrontTimeMinutes = c.Int(nullable: false),
                        GapToCarInFrontTimeSeconds = c.Int(nullable: false),
                        GapToCarInFrontTimeMilliseconds = c.Int(nullable: false),
                        CmsOverrideModeIsActive = c.Boolean(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorsportTeam_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.MotorsportRaceEventId, t.MotorsportDriverId })
                .ForeignKey("dbo.MotorsportDrivers", t => t.MotorsportDriverId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportRaceEvents", t => t.MotorsportRaceEventId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeam_Id)
                .Index(t => t.MotorsportRaceEventId)
                .Index(t => t.MotorsportDriverId)
                .Index(t => t.MotorsportTeam_Id);
            
            CreateTable(
                "dbo.MotorsportTeamStandings",
                c => new
                    {
                        MotorsportLeagueId = c.Guid(nullable: false),
                        MotorsportSeasonId = c.Guid(nullable: false),
                        MotorsportTeamId = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.MotorsportLeagueId, t.MotorsportSeasonId, t.MotorsportTeamId })
                .ForeignKey("dbo.MotorsportSeasons", t => t.MotorsportSeasonId, cascadeDelete: true)
                .ForeignKey("dbo.MotorsportTeams", t => t.MotorsportTeamId, cascadeDelete: true)
                .Index(t => t.MotorsportSeasonId)
                .Index(t => t.MotorsportTeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportTeamStandings", "MotorsportTeamId", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportTeamStandings", "MotorsportSeasonId", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportRaceEventResults", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportRaceEventResults", "MotorsportRaceEventId", "dbo.MotorsportRaceEvents");
            DropForeignKey("dbo.MotorsportRaceEventResults", "MotorsportDriverId", "dbo.MotorsportDrivers");
            DropForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportRaceEventId", "dbo.MotorsportRaceEvents");
            DropForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportDriverId", "dbo.MotorsportDrivers");
            DropForeignKey("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportRaceEvents", "MotorsportRace_Id", "dbo.MotorsportRaces");
            DropForeignKey("dbo.MotorsportRaces", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportSeasonId", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportSeasons", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportDriverId", "dbo.MotorsportDrivers");
            DropForeignKey("dbo.MotorsportDrivers", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropForeignKey("dbo.MotorsportTeams", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportDrivers", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropIndex("dbo.MotorsportTeamStandings", new[] { "MotorsportTeamId" });
            DropIndex("dbo.MotorsportTeamStandings", new[] { "MotorsportSeasonId" });
            DropIndex("dbo.MotorsportRaceEventResults", new[] { "MotorsportTeam_Id" });
            DropIndex("dbo.MotorsportRaceEventResults", new[] { "MotorsportDriverId" });
            DropIndex("dbo.MotorsportRaceEventResults", new[] { "MotorsportRaceEventId" });
            DropIndex("dbo.MotorsportRaceEventGrids", new[] { "MotorsportTeam_Id" });
            DropIndex("dbo.MotorsportRaceEventGrids", new[] { "MotorsportDriverId" });
            DropIndex("dbo.MotorsportRaceEventGrids", new[] { "MotorsportRaceEventId" });
            DropIndex("dbo.MotorsportRaceEvents", new[] { "MotorsportSeason_Id" });
            DropIndex("dbo.MotorsportRaceEvents", new[] { "MotorsportRace_Id" });
            DropIndex("dbo.MotorsportRaces", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportSeasons", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportSeasons", "Seek_ProviderSeasonId");
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportTeam_Id" });
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportDriverId" });
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportSeasonId" });
            DropIndex("dbo.MotorsportTeams", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportTeams", "Seek_ProviderTeamId");
            DropIndex("dbo.MotorsportLeagues", "Unique_Slug");
            DropIndex("dbo.MotorsportDrivers", new[] { "MotorsportTeam_Id" });
            DropIndex("dbo.MotorsportDrivers", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportDrivers", "Seek_ProviderDriverId");
            DropTable("dbo.MotorsportTeamStandings");
            DropTable("dbo.MotorsportRaceEventResults");
            DropTable("dbo.MotorsportRaceEventGrids");
            DropTable("dbo.MotorsportRaceEvents");
            DropTable("dbo.MotorsportRaces");
            DropTable("dbo.MotorsportSeasons");
            DropTable("dbo.MotorsportDriverStandings");
            DropTable("dbo.MotorsportTeams");
            DropTable("dbo.MotorsportLeagues");
            DropTable("dbo.MotorsportDrivers");
        }
    }
}
