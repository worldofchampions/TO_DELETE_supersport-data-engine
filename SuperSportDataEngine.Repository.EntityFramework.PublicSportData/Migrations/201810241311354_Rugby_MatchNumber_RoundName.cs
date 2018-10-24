namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_MatchNumber_RoundName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "MatchNumber", c => c.Int());
            AddColumn("dbo.RugbyFixtures", "MatchNumberCmsOverride", c => c.Int());
            AddColumn("dbo.RugbyFixtures", "RoundName", c => c.String());
            AddColumn("dbo.RugbyFixtures", "RoundNameCmsOverride", c => c.String());
            AddColumn("dbo.RugbyTournaments", "HasFixtureRoundName", c => c.Boolean(nullable: false));
            AddColumn("dbo.RugbyTournaments", "HasFixtureMatchNumber", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyTournaments", "HasFixtureMatchNumber");
            DropColumn("dbo.RugbyTournaments", "HasFixtureRoundName");
            DropColumn("dbo.RugbyFixtures", "RoundNameCmsOverride");
            DropColumn("dbo.RugbyFixtures", "RoundName");
            DropColumn("dbo.RugbyFixtures", "MatchNumberCmsOverride");
            DropColumn("dbo.RugbyFixtures", "MatchNumber");
        }
    }
}
