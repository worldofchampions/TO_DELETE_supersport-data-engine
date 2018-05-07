namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Add_SchedulerTrackingState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventPolling", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventPolling");
        }
    }
}
