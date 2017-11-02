namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixes_for_legacyId_fields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RugbyFixtures", "Unique_LegacyFixtureId");
            AddColumn("dbo.RugbyTournaments", "LegacyTournamentId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.RugbyTeams", "LegacyTeamId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.RugbyPlayers", "LegacyPlayerId", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyPlayers", "LegacyPlayerId");
            DropColumn("dbo.RugbyTeams", "LegacyTeamId");
            DropColumn("dbo.RugbyTournaments", "LegacyTournamentId");
            CreateIndex("dbo.RugbyFixtures", "LegacyFixtureId", unique: true, name: "Unique_LegacyFixtureId");
        }
    }
}
