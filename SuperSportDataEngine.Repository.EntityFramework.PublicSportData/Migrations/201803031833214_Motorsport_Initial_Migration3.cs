namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Initial_Migration3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportTeamId", "dbo.MotorsportTeams");
            DropIndex("dbo.MotorsportRaceEventGrids", new[] { "MotorsportTeamId" });
            RenameColumn(table: "dbo.MotorsportRaceEventGrids", name: "MotorsportTeamId", newName: "MotorsportTeam_Id");
            AlterColumn("dbo.MotorsportRaceEventGrids", "MotorsportTeam_Id", c => c.Guid());
            CreateIndex("dbo.MotorsportRaceEventGrids", "MotorsportTeam_Id");
            AddForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportTeam_Id", "dbo.MotorsportTeams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportTeam_Id", "dbo.MotorsportTeams");
            DropIndex("dbo.MotorsportRaceEventGrids", new[] { "MotorsportTeam_Id" });
            AlterColumn("dbo.MotorsportRaceEventGrids", "MotorsportTeam_Id", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.MotorsportRaceEventGrids", name: "MotorsportTeam_Id", newName: "MotorsportTeamId");
            CreateIndex("dbo.MotorsportRaceEventGrids", "MotorsportTeamId");
            AddForeignKey("dbo.MotorsportRaceEventGrids", "MotorsportTeamId", "dbo.MotorsportTeams", "Id", cascadeDelete: true);
        }
    }
}
