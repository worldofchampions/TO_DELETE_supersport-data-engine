namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addding_MotorRaces_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MotorRaces",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MotorLeagueId = c.Guid(nullable: false),
                        LegacyRaceId = c.Int(nullable: false),
                        ProviderRaceId = c.Int(nullable: false),
                        Name = c.String(),
                        NameCmsOverride = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        Slug = c.String(),
                        Abbreviation = c.String(),
                        DisplayNameCmsOverride = c.String(),
                        DataProvider = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorLeagues", t => t.MotorLeagueId, cascadeDelete: true)
                .Index(t => t.MotorLeagueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorRaces", "MotorLeagueId", "dbo.MotorLeagues");
            DropIndex("dbo.MotorRaces", new[] { "MotorLeagueId" });
            DropTable("dbo.MotorRaces");
        }
    }
}
