namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Initial_Migration4 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MotorsportRaceEventResults", "MotorsportRaceEventId");
            AddForeignKey("dbo.MotorsportRaceEventResults", "MotorsportRaceEventId", "dbo.MotorsportRaceEvents", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportRaceEventResults", "MotorsportRaceEventId", "dbo.MotorsportRaceEvents");
            DropIndex("dbo.MotorsportRaceEventResults", new[] { "MotorsportRaceEventId" });
        }
    }
}
