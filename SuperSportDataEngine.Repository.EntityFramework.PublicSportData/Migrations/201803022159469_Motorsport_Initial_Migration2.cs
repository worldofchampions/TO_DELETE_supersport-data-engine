namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Initial_Migration2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportTeamId", "dbo.MotorsportTeams");
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportTeamId" });
            RenameColumn(table: "dbo.MotorsportDriverStandings", name: "MotorsportTeamId", newName: "MotorsportTeam_Id");
            DropPrimaryKey("dbo.MotorsportDriverStandings");
            AlterColumn("dbo.MotorsportDriverStandings", "MotorsportTeam_Id", c => c.Guid());
            AddPrimaryKey("dbo.MotorsportDriverStandings", new[] { "MotorsportLeagueId", "MotorsportSeasonId", "MotorsportDriverId" });
            CreateIndex("dbo.MotorsportDriverStandings", "MotorsportTeam_Id");
            AddForeignKey("dbo.MotorsportDriverStandings", "MotorsportTeam_Id", "dbo.MotorsportTeams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportDriverStandings", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropIndex("dbo.MotorsportDriverStandings", new[] { "MotorsportTeam_Id" });
            DropPrimaryKey("dbo.MotorsportDriverStandings");
            AlterColumn("dbo.MotorsportDriverStandings", "MotorsportTeam_Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.MotorsportDriverStandings", new[] { "MotorsportLeagueId", "MotorsportSeasonId", "MotorsportTeamId", "MotorsportDriverId" });
            RenameColumn(table: "dbo.MotorsportDriverStandings", name: "MotorsportTeam_Id", newName: "MotorsportTeamId");
            CreateIndex("dbo.MotorsportDriverStandings", "MotorsportTeamId");
            AddForeignKey("dbo.MotorsportDriverStandings", "MotorsportTeamId", "dbo.MotorsportTeams", "Id", cascadeDelete: true);
        }
    }
}
