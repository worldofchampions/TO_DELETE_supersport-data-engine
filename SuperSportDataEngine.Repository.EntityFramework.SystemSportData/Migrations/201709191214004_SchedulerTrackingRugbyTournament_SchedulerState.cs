namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SchedulerTrackingRugbyTournament_SchedulerState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerTrackingRugbyTournaments", "SchedulerStateForManagerJobPolling", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchedulerTrackingRugbyTournaments", "SchedulerStateForManagerJobPolling");
        }
    }
}
