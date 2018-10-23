namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_Add_HasFixtureRoundNameAndHasFixtureMatchNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyTournaments", "HasFixtureRoundName", c => c.Boolean(nullable: false));
            AddColumn("dbo.RugbyTournaments", "HasFixtureMatchNumber", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyTournaments", "HasFixtureMatchNumber");
            DropColumn("dbo.RugbyTournaments", "HasFixtureRoundName");
        }
    }
}
