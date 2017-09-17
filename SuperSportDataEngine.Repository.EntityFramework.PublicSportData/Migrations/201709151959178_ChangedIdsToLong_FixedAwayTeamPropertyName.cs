namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedIdsToLong_FixedAwayTeamPropertyName : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RugbyFixtures", "Unique_LegacyFixtureId");
            DropIndex("dbo.RugbyFixtures", "Seek_ProviderFixtureId");
            RenameColumn(table: "dbo.RugbyFixtures", name: "AwayTeamTeam_Id", newName: "AwayTeam_Id");
            RenameIndex(table: "dbo.RugbyFixtures", name: "IX_AwayTeamTeam_Id", newName: "IX_AwayTeam_Id");
            AlterColumn("dbo.RugbyFixtures", "LegacyFixtureId", c => c.Long(nullable: false));
            AlterColumn("dbo.RugbyFixtures", "ProviderFixtureId", c => c.Long(nullable: false));
            CreateIndex("dbo.RugbyFixtures", "LegacyFixtureId", unique: true, name: "Unique_LegacyFixtureId");
            CreateIndex("dbo.RugbyFixtures", "ProviderFixtureId", name: "Seek_ProviderFixtureId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RugbyFixtures", "Seek_ProviderFixtureId");
            DropIndex("dbo.RugbyFixtures", "Unique_LegacyFixtureId");
            AlterColumn("dbo.RugbyFixtures", "ProviderFixtureId", c => c.Int(nullable: false));
            AlterColumn("dbo.RugbyFixtures", "LegacyFixtureId", c => c.Int(nullable: false));
            RenameIndex(table: "dbo.RugbyFixtures", name: "IX_AwayTeam_Id", newName: "IX_AwayTeamTeam_Id");
            RenameColumn(table: "dbo.RugbyFixtures", name: "AwayTeam_Id", newName: "AwayTeamTeam_Id");
            CreateIndex("dbo.RugbyFixtures", "ProviderFixtureId", name: "Seek_ProviderFixtureId");
            CreateIndex("dbo.RugbyFixtures", "LegacyFixtureId", unique: true, name: "Unique_LegacyFixtureId");
        }
    }
}
