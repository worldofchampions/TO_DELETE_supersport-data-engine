namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_tournament_level_IsLiveScored_flag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "IsLiveScored", c => c.Boolean(nullable: false));
            AddColumn("dbo.RugbyTournaments", "IsLiveScored", c => c.Boolean(nullable: false));
            DropColumn("dbo.RugbyFixtures", "IsFixtureLiveScored");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RugbyFixtures", "IsFixtureLiveScored", c => c.Boolean(nullable: false));
            DropColumn("dbo.RugbyTournaments", "IsLiveScored");
            DropColumn("dbo.RugbyFixtures", "IsLiveScored");
        }
    }
}
