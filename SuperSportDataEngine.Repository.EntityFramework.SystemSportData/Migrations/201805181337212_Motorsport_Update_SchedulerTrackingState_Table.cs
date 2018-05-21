namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Update_SchedulerTrackingState_Table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventGridPolling", c => c.Int(nullable: false));
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventLivePolling", c => c.Int(nullable: false));
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventResultsPolling", c => c.Int(nullable: false));
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventStandingsPolling", c => c.Int(nullable: false));
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "IsJobRunning");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "IsJobRunning", c => c.Boolean(nullable: false));
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventStandingsPolling");
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventResultsPolling");
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventLivePolling");
            DropColumn("dbo.SchedulerTrackingMotorsportRaceEvents", "SchedulerStateForMotorsportRaceEventGridPolling");
        }
    }
}
