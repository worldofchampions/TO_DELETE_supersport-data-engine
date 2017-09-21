namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changed_team_naming_on_RugbyFixture_table : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RugbyFixtures", name: "AwayTeam_Id", newName: "TeamA_Id");
            RenameColumn(table: "dbo.RugbyFixtures", name: "HomeTeam_Id", newName: "TeamB_Id");
            RenameIndex(table: "dbo.RugbyFixtures", name: "IX_AwayTeam_Id", newName: "IX_TeamA_Id");
            RenameIndex(table: "dbo.RugbyFixtures", name: "IX_HomeTeam_Id", newName: "IX_TeamB_Id");
            AddColumn("dbo.RugbyFixtures", "TeamAIsHomeTeam", c => c.Boolean(nullable: false));
            AddColumn("dbo.RugbyFixtures", "TeamBIsHomeTeam", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "TeamBIsHomeTeam");
            DropColumn("dbo.RugbyFixtures", "TeamAIsHomeTeam");
            RenameIndex(table: "dbo.RugbyFixtures", name: "IX_TeamB_Id", newName: "IX_HomeTeam_Id");
            RenameIndex(table: "dbo.RugbyFixtures", name: "IX_TeamA_Id", newName: "IX_AwayTeam_Id");
            RenameColumn(table: "dbo.RugbyFixtures", name: "TeamB_Id", newName: "HomeTeam_Id");
            RenameColumn(table: "dbo.RugbyFixtures", name: "TeamA_Id", newName: "AwayTeam_Id");
        }
    }
}
