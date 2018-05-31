namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_RaceEvents_Tracking_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerTrackingMotorsportRaceEvents",
                c => new
                    {
                        MotorsportRaceEventId = c.Guid(nullable: false),
                        MotorsportLeagueId = c.Guid(nullable: false),
                        StartDateTimeUtc = c.DateTimeOffset(precision: 7),
                        EndedDateTimeUtc = c.DateTimeOffset(precision: 7),
                        MotorsportRaceEventStatus = c.Int(nullable: false),
                        SchedulerStateForMotorsportRaceEventGridPolling = c.Int(nullable: false),
                        SchedulerStateForMotorsportRaceEventLivePolling = c.Int(nullable: false),
                        SchedulerStateForMotorsportRaceEventResultsPolling = c.Int(nullable: false),
                        SchedulerStateForMotorsportRaceEventStandingsPolling = c.Int(nullable: false),
                        SchedulerStateForMotorsportRaceEventCalendarPolling = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.MotorsportRaceEventId, t.MotorsportLeagueId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerTrackingMotorsportRaceEvents");
        }
    }
}
