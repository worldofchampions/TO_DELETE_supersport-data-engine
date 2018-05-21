namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Update_SchedulerTrackingState_Table_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventCalendarPolling", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventCalendarPolling");
        }
    }
}
