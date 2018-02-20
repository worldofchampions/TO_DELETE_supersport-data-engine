namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_MotorsportMigrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MotorCars",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderCarId = c.Int(nullable: false),
                        CarNumber = c.Int(nullable: false),
                        CarDisplayNumber = c.Int(nullable: false),
                        MakeId = c.Int(nullable: false),
                        MakeName = c.String(),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MotorDrivers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LegacyDriverId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        FullName = c.String(),
                        HeightInCentimeters = c.Double(nullable: false),
                        WeightInKilograms = c.Double(nullable: false),
                        DisplayNameCmsOverride = c.String(),
                        CountryName = c.String(),
                        ProviderCarId = c.Int(nullable: false),
                        CarNumber = c.Int(),
                        CarDisplayNumber = c.Int(),
                        CarName = c.String(),
                        CarDisplayName = c.String(),
                        TeamName = c.String(),
                        ProviderTeamId = c.Int(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MotorGrids",
                c => new
                    {
                        RaceId = c.Guid(nullable: false),
                        DriverId = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        QualifyingTime_Hours = c.Int(nullable: false),
                        QualifyingTime_Minutes = c.Int(nullable: false),
                        QualifyingTime_Seconds = c.Int(nullable: false),
                        QualifyingTime_Milliseconds = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorDriver_Id = c.Guid(),
                        MotorRace_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.RaceId, t.DriverId })
                .ForeignKey("dbo.MotorDrivers", t => t.MotorDriver_Id)
                .ForeignKey("dbo.MotorRaces", t => t.MotorRace_Id)
                .Index(t => t.MotorDriver_Id)
                .Index(t => t.MotorRace_Id);
            
            CreateTable(
                "dbo.MotorRaces",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MotorLeagueId = c.Guid(nullable: false),
                        LegacyRaceId = c.Int(nullable: false, identity: true),
                        ProviderRaceId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        NameCmsOverride = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 450),
                        Abbreviation = c.String(),
                        DisplayNameCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorLeagues", t => t.MotorLeagueId, cascadeDelete: true)
                .Index(t => t.MotorLeagueId)
                .Index(t => t.ProviderRaceId, name: "Seek_ProviderRaceId")
                .Index(t => t.Slug, unique: true, name: "Unique_Slug");
            
            CreateTable(
                "dbo.MotorLeagues",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyLeagueId = c.Int(nullable: false, identity: true),
                        ProviderLeagueId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        ProviderSlug = c.String(nullable: false),
                        NameCmsOverride = c.String(),
                        Slug = c.String(nullable: false),
                        Abbreviation = c.String(),
                        DisplayName = c.String(),
                        DisplayNameCmsOverride = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        DataProvider = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MotorRaceResults",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DriverId = c.Guid(nullable: false),
                        LapsCompleted = c.Int(nullable: false),
                        LapsLed = c.Int(nullable: false),
                        LapsBehind = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        StartingPosition = c.Int(nullable: false),
                        IsFastest = c.Boolean(nullable: false),
                        FinishingTime_Hours = c.Int(nullable: false),
                        FinishingTime_Minutes = c.Int(nullable: false),
                        FinishingTime_Seconds = c.Int(nullable: false),
                        FinishingTime_Milliseconds = c.Int(nullable: false),
                        DriverTotalPoints = c.Int(nullable: false),
                        DriverBonusPoints = c.Int(nullable: false),
                        DriverPenaltyPoints = c.Int(nullable: false),
                        OwnerTotalPoints = c.Int(nullable: false),
                        OwnerBonusPoints = c.Int(nullable: false),
                        OwnerPenaltyPoints = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorDriver_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorDrivers", t => t.MotorDriver_Id)
                .Index(t => t.MotorDriver_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorRaceResults", "MotorDriver_Id", "dbo.MotorDrivers");
            DropForeignKey("dbo.MotorGrids", "MotorRace_Id", "dbo.MotorRaces");
            DropForeignKey("dbo.MotorRaces", "MotorLeagueId", "dbo.MotorLeagues");
            DropForeignKey("dbo.MotorGrids", "MotorDriver_Id", "dbo.MotorDrivers");
            DropIndex("dbo.MotorRaceResults", new[] { "MotorDriver_Id" });
            DropIndex("dbo.MotorRaces", "Unique_Slug");
            DropIndex("dbo.MotorRaces", "Seek_ProviderRaceId");
            DropIndex("dbo.MotorRaces", new[] { "MotorLeagueId" });
            DropIndex("dbo.MotorGrids", new[] { "MotorRace_Id" });
            DropIndex("dbo.MotorGrids", new[] { "MotorDriver_Id" });
            DropTable("dbo.MotorRaceResults");
            DropTable("dbo.MotorLeagues");
            DropTable("dbo.MotorRaces");
            DropTable("dbo.MotorGrids");
            DropTable("dbo.MotorDrivers");
            DropTable("dbo.MotorCars");
        }
    }
}
