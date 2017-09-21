namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Team_Scores_to_RugbyFixture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "TeamAScore", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyFixtures", "TeamBScore", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyFixtures", "IsFixtureLiveScored", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "IsFixtureLiveScored");
            DropColumn("dbo.RugbyFixtures", "TeamBScore");
            DropColumn("dbo.RugbyFixtures", "TeamAScore");
        }
    }
}
