namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_Add_RoundNameAndItsCmsOverride_ForFixtures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "RoundName", c => c.String());
            AddColumn("dbo.RugbyFixtures", "RoundNameCmsOverride", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "RoundNameCmsOverride");
            DropColumn("dbo.RugbyFixtures", "RoundName");
        }
    }
}
