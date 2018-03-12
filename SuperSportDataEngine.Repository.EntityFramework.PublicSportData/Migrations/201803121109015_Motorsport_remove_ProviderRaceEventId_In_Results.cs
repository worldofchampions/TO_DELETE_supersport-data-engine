namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_remove_ProviderRaceEventId_In_Results : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MotorsportRaceEventResults", "ProviderRaceEventId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MotorsportRaceEventResults", "ProviderRaceEventId", c => c.Int(nullable: false));
        }
    }
}
