namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_RugbyLog_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyLogs",
                c => new
                    {
                        RugbyTournamentId = c.Guid(nullable: false),
                        RugbySeasonId = c.Guid(nullable: false),
                        RoundNumber = c.Int(nullable: false),
                        RugbyTeamId = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        GamesPlayed = c.Int(nullable: false),
                        GamesWon = c.Int(nullable: false),
                        GamesDrawn = c.Int(nullable: false),
                        GamesLost = c.Int(nullable: false),
                        PointsFor = c.Int(nullable: false),
                        PointsAgainst = c.Int(nullable: false),
                        PointsDifference = c.Int(nullable: false),
                        TournamentPoints = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        TriesFor = c.Int(nullable: false),
                        TriesAgainst = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RugbyTournamentId, t.RugbySeasonId, t.RoundNumber, t.RugbyTeamId })
                .ForeignKey("dbo.RugbySeasons", t => t.RugbySeasonId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeamId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTournaments", t => t.RugbyTournamentId, cascadeDelete: true)
                .Index(t => t.RugbyTournamentId)
                .Index(t => t.RugbySeasonId)
                .Index(t => t.RugbyTeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyLogs", "RugbyTournamentId", "dbo.RugbyTournaments");
            DropForeignKey("dbo.RugbyLogs", "RugbyTeamId", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyLogs", "RugbySeasonId", "dbo.RugbySeasons");
            DropIndex("dbo.RugbyLogs", new[] { "RugbyTeamId" });
            DropIndex("dbo.RugbyLogs", new[] { "RugbySeasonId" });
            DropIndex("dbo.RugbyLogs", new[] { "RugbyTournamentId" });
            DropTable("dbo.RugbyLogs");
        }
    }
}
