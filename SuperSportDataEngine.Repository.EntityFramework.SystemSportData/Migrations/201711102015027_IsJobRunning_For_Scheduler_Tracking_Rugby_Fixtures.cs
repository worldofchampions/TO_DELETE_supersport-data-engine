namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsJobRunning_For_Scheduler_Tracking_Rugby_Fixtures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerTrackingRugbyFixtures", "IsJobRunning", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchedulerTrackingRugbyFixtures", "IsJobRunning");
        }
    }
}
