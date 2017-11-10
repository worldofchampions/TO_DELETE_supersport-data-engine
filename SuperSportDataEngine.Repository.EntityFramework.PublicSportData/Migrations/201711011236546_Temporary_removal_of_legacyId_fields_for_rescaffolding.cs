namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Temporary_removal_of_legacyId_fields_for_rescaffolding : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RugbyTournaments", "Unique_LegacyTournamentId");
            DropIndex("dbo.RugbyTeams", "Unique_LegacyTeamId");
            DropColumn("dbo.RugbyTournaments", "LegacyTournamentId");
            DropColumn("dbo.RugbyTeams", "LegacyTeamId");
            DropColumn("dbo.RugbyPlayers", "LegacyPlayerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RugbyPlayers", "LegacyPlayerId", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyTeams", "LegacyTeamId", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyTournaments", "LegacyTournamentId", c => c.Int(nullable: false));
            CreateIndex("dbo.RugbyTeams", "LegacyTeamId", unique: true, name: "Unique_LegacyTeamId");
            CreateIndex("dbo.RugbyTournaments", "LegacyTournamentId", unique: true, name: "Unique_LegacyTournamentId");
        }
    }
}
