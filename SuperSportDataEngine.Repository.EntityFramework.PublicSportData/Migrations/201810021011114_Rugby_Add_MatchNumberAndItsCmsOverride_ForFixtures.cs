namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_Add_MatchNumberAndItsCmsOverride_ForFixtures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "MatchNumber", c => c.Int());
            AddColumn("dbo.RugbyFixtures", "MatchNumberCmsOverride", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "MatchNumberCmsOverride");
            DropColumn("dbo.RugbyFixtures", "MatchNumber");
        }
    }
}
