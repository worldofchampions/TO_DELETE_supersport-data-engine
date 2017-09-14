namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Scheduler_Tracking_Rugby_Season_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerTrackingRugbySeasons",
                c => new
                    {
                        SeasonId = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        RugbySeasonStatus = c.Int(nullable: false),
                        SchedulerStateForManagerJobPolling = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.SeasonId, t.TournamentId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerTrackingRugbySeasons");
        }
    }
}
