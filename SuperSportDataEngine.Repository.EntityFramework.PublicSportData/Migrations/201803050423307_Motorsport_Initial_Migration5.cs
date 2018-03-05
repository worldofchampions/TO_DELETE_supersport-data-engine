namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Initial_Migration5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MotorsportRaceEvents", "MotorsportRace_Id", "dbo.MotorsportRaces");
            DropForeignKey("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", "dbo.MotorsportSeasons");
            DropIndex("dbo.MotorsportRaceEvents", new[] { "MotorsportRace_Id" });
            DropIndex("dbo.MotorsportRaceEvents", new[] { "MotorsportSeason_Id" });
            AlterColumn("dbo.MotorsportRaceEvents", "MotorsportRace_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", c => c.Guid(nullable: false));
            CreateIndex("dbo.MotorsportRaceEvents", "MotorsportRace_Id");
            CreateIndex("dbo.MotorsportRaceEvents", "MotorsportSeason_Id");
            AddForeignKey("dbo.MotorsportRaceEvents", "MotorsportRace_Id", "dbo.MotorsportRaces", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", "dbo.MotorsportSeasons", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", "dbo.MotorsportSeasons");
            DropForeignKey("dbo.MotorsportRaceEvents", "MotorsportRace_Id", "dbo.MotorsportRaces");
            DropIndex("dbo.MotorsportRaceEvents", new[] { "MotorsportSeason_Id" });
            DropIndex("dbo.MotorsportRaceEvents", new[] { "MotorsportRace_Id" });
            AlterColumn("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", c => c.Guid());
            AlterColumn("dbo.MotorsportRaceEvents", "MotorsportRace_Id", c => c.Guid());
            CreateIndex("dbo.MotorsportRaceEvents", "MotorsportSeason_Id");
            CreateIndex("dbo.MotorsportRaceEvents", "MotorsportRace_Id");
            AddForeignKey("dbo.MotorsportRaceEvents", "MotorsportSeason_Id", "dbo.MotorsportSeasons", "Id");
            AddForeignKey("dbo.MotorsportRaceEvents", "MotorsportRace_Id", "dbo.MotorsportRaces", "Id");
        }
    }
}
