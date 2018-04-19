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
                        StartDateTime = c.DateTimeOffset(precision: 7),
                        EndedDateTime = c.DateTimeOffset(precision: 7),
                        MotorsportRaceEventStatus = c.Int(nullable: false),
                        IsJobRunning = c.Boolean(nullable: false),
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
