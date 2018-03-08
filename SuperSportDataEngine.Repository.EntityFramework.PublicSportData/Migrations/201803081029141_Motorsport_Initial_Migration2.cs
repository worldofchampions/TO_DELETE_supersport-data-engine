namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Initial_Migration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MotorsportRaceEvents", "EventWinner_Id", c => c.Guid());
            CreateIndex("dbo.MotorsportRaceEvents", "EventWinner_Id");
            AddForeignKey("dbo.MotorsportRaceEvents", "EventWinner_Id", "dbo.MotorsportDrivers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportRaceEvents", "EventWinner_Id", "dbo.MotorsportDrivers");
            DropIndex("dbo.MotorsportRaceEvents", new[] { "EventWinner_Id" });
            DropColumn("dbo.MotorsportRaceEvents", "EventWinner_Id");
        }
    }
}
