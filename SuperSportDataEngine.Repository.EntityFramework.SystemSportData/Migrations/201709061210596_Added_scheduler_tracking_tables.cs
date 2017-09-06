namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_scheduler_tracking_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerTrackingRugbyFixtures",
                c => new
                    {
                        FixtureId = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        StartDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndedDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        RugbyFixtureStatus = c.Int(nullable: false),
                        SchedulerStateFixtures = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FixtureId, t.TournamentId });
            
            CreateTable(
                "dbo.SchedulerTrackingRugbyTournaments",
                c => new
                    {
                        TournamentId = c.Guid(nullable: false),
                        SeasonId = c.Guid(nullable: false),
                        SchedulerStateLogs = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TournamentId, t.SeasonId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerTrackingRugbyTournaments");
            DropTable("dbo.SchedulerTrackingRugbyFixtures");
        }
    }
}
