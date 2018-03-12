namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_league_navigtion_for_standings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MotorsportDriverStandings", "MotorsportLeague_Id", c => c.Guid(nullable: false));
            AddColumn("dbo.MotorsportTeamStandings", "MotorsportLeague_Id", c => c.Guid(nullable: false));
            CreateIndex("dbo.MotorsportDriverStandings", "MotorsportLeague_Id");
            CreateIndex("dbo.MotorsportTeamStandings", "MotorsportLeague_Id");
            AddForeignKey("dbo.MotorsportDriverStandings", "MotorsportLeague_Id", "dbo.MotorsportLeagues", "Id");
            AddForeignKey("dbo.MotorsportTeamStandings", "MotorsportLeague_Id", "dbo.MotorsportLeagues", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportTeamStandings", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportLeague_Id", "dbo.MotorsportLeagues");
            DropIndex("dbo.MotorsportTeamStandings", new[] { "MotorsportLeague_Id" });
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportLeague_Id" });
            DropColumn("dbo.MotorsportTeamStandings", "MotorsportLeague_Id");
            DropColumn("dbo.MotorsportDriverStandings", "MotorsportLeague_Id");
        }
    }
}
