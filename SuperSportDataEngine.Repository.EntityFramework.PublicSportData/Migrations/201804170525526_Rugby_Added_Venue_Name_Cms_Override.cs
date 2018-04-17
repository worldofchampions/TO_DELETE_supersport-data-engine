namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_Added_Venue_Name_Cms_Override : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyVenues", "NameCmsOverride", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyVenues", "NameCmsOverride");
        }
    }
}
