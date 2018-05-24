namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_EventTrackingTbl_Remove_Unused_Column : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventPolling");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventPolling", c => c.Int(nullable: false));
        }
    }
}
