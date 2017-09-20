namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_RugbyPlayerLineup_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyPlayerLineups",
                c => new
                    {
                        RugbyFixtureId = c.Guid(nullable: false),
                        RugbyTeamId = c.Guid(nullable: false),
                        RugbyPlayerId = c.Guid(nullable: false),
                        ShirtNumber = c.Int(nullable: false),
                        PositionName = c.Int(nullable: false),
                        IsCaptain = c.Boolean(nullable: false),
                        IsSubstitute = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.RugbyFixtureId, t.RugbyTeamId, t.RugbyPlayerId })
                .ForeignKey("dbo.RugbyFixtures", t => t.RugbyFixtureId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyPlayers", t => t.RugbyPlayerId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeamId, cascadeDelete: true)
                .Index(t => t.RugbyFixtureId)
                .Index(t => t.RugbyTeamId)
                .Index(t => t.RugbyPlayerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyPlayerLineups", "RugbyTeamId", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyPlayerLineups", "RugbyPlayerId", "dbo.RugbyPlayers");
            DropForeignKey("dbo.RugbyPlayerLineups", "RugbyFixtureId", "dbo.RugbyFixtures");
            DropIndex("dbo.RugbyPlayerLineups", new[] { "RugbyPlayerId" });
            DropIndex("dbo.RugbyPlayerLineups", new[] { "RugbyTeamId" });
            DropIndex("dbo.RugbyPlayerLineups", new[] { "RugbyFixtureId" });
            DropTable("dbo.RugbyPlayerLineups");
        }
    }
}
