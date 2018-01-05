namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_MotorSeasonTracking_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerTrackingMotorSeasons",
                c => new
                    {
                        SeasonId = c.Guid(nullable: false),
                        LeagueId = c.Guid(nullable: false),
                        MotorSeasonStatus = c.Int(nullable: false),
                        SchedulerStateForManagerJobPolling = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.SeasonId, t.LeagueId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerTrackingMotorSeasons");
        }
    }
}
