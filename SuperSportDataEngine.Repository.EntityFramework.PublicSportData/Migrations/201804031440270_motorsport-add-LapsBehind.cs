namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class motorsportaddLapsBehind : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MotorsportRaceEventResults", "LapsBehind", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MotorsportRaceEventResults", "LapsBehind");
        }
    }
}
