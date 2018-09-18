namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tennis_InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerTrackingTennisEvents",
                c => new
                    {
                        TennisEventId = c.Guid(nullable: false),
                        StartDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.TennisEventId);
            
            CreateTable(
                "dbo.SchedulerTrackingTennisMatches",
                c => new
                    {
                        TennisMatchId = c.Guid(nullable: false),
                        StartDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDateTime = c.DateTimeOffset(precision: 7),
                        SchedulerStateForTennisMatchPolling = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TennisMatchId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerTrackingTennisMatches");
            DropTable("dbo.SchedulerTrackingTennisEvents");
        }
    }
}
