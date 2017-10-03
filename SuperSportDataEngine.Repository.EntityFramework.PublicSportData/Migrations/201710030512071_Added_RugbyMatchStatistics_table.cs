namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_RugbyMatchStatistics_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyMatchStatistics",
                c => new
                    {
                        RugbyFixtureId = c.Guid(nullable: false),
                        RugbyTeamId = c.Guid(nullable: false),
                        CarriesCrossedGainLine = c.Int(nullable: false),
                        CleanBreaks = c.Int(nullable: false),
                        ConversionAttempts = c.Int(nullable: false),
                        Conversions = c.Int(nullable: false),
                        ConversionsMissed = c.Int(nullable: false),
                        DefendersBeaten = c.Int(nullable: false),
                        DropGoalAttempts = c.Int(nullable: false),
                        DropGoals = c.Int(nullable: false),
                        DropGoalsMissed = c.Int(nullable: false),
                        LineOutsLost = c.Int(nullable: false),
                        LineOutsWon = c.Int(nullable: false),
                        Offloads = c.Int(nullable: false),
                        Passes = c.Int(nullable: false),
                        Penalties = c.Int(nullable: false),
                        PenaltiesConceded = c.Int(nullable: false),
                        PenaltiesMissed = c.Int(nullable: false),
                        PenaltyAttempts = c.Int(nullable: false),
                        PenaltyTries = c.Int(nullable: false),
                        Possession = c.Int(nullable: false),
                        RedCards = c.Int(nullable: false),
                        ScrumsLost = c.Int(nullable: false),
                        ScrumsWon = c.Int(nullable: false),
                        Tackles = c.Int(nullable: false),
                        TacklesMissed = c.Int(nullable: false),
                        Territory = c.Int(nullable: false),
                        Tries = c.Int(nullable: false),
                        YellowCards = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RugbyFixtureId, t.RugbyTeamId })
                .ForeignKey("dbo.RugbyFixtures", t => t.RugbyFixtureId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeamId, cascadeDelete: true)
                .Index(t => t.RugbyFixtureId)
                .Index(t => t.RugbyTeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyMatchStatistics", "RugbyTeamId", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyMatchStatistics", "RugbyFixtureId", "dbo.RugbyFixtures");
            DropIndex("dbo.RugbyMatchStatistics", new[] { "RugbyTeamId" });
            DropIndex("dbo.RugbyMatchStatistics", new[] { "RugbyFixtureId" });
            DropTable("dbo.RugbyMatchStatistics");
        }
    }
}
