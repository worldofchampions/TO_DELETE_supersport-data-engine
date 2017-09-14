namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemModelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerTrackingRugbySeasons", "SchedulerStateForManagerJobPolling", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchedulerTrackingRugbySeasons", "SchedulerStateForManagerJobPolling");
        }
    }
}
