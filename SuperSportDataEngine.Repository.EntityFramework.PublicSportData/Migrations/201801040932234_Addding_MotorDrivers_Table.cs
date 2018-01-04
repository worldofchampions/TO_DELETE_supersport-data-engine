namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addding_MotorDrivers_Table : DbMigration
    {
        public override void Up()
        {
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
            
            CreateIndex("dbo.MotorLeagues", "Slug", unique: true, name: "Unique_Slug");
        }
        
        public override void Down()
        {
            DropIndex("dbo.MotorLeagues", "Unique_Slug");
            DropTable("dbo.MotorDrivers");
        }
    }
}
