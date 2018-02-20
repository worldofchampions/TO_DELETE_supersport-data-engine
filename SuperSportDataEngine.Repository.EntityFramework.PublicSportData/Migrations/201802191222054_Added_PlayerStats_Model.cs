namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_PlayerStats_Model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyPlayerStatistics",
                c => new
                    {
                        RugbyTournamentId = c.Guid(nullable: false),
                        RugbySeasonId = c.Guid(nullable: false),
                        RugbyTeamId = c.Guid(nullable: false),
                        RugbyPlayerId = c.Guid(nullable: false),
                        Rank = c.Int(nullable: false),
                        TriesScored = c.Int(nullable: false),
                        PenaltiesScored = c.Int(nullable: false),
                        ConversionsScored = c.Int(nullable: false),
                        DropGoalsScored = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.RugbyTournamentId, t.RugbySeasonId, t.RugbyTeamId, t.RugbyPlayerId })
                .ForeignKey("dbo.RugbyPlayers", t => t.RugbyPlayerId, cascadeDelete: true)
                .ForeignKey("dbo.RugbySeasons", t => t.RugbySeasonId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeamId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTournaments", t => t.RugbyTournamentId, cascadeDelete: true)
                .Index(t => t.RugbyTournamentId)
                .Index(t => t.RugbySeasonId)
                .Index(t => t.RugbyTeamId)
                .Index(t => t.RugbyPlayerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyPlayerStatistics", "RugbyTournamentId", "dbo.RugbyTournaments");
            DropForeignKey("dbo.RugbyPlayerStatistics", "RugbyTeamId", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyPlayerStatistics", "RugbySeasonId", "dbo.RugbySeasons");
            DropForeignKey("dbo.RugbyPlayerStatistics", "RugbyPlayerId", "dbo.RugbyPlayers");
            DropIndex("dbo.RugbyPlayerStatistics", new[] { "RugbyPlayerId" });
            DropIndex("dbo.RugbyPlayerStatistics", new[] { "RugbyTeamId" });
            DropIndex("dbo.RugbyPlayerStatistics", new[] { "RugbySeasonId" });
            DropIndex("dbo.RugbyPlayerStatistics", new[] { "RugbyTournamentId" });
            DropTable("dbo.RugbyPlayerStatistics");
        }
    }
}
