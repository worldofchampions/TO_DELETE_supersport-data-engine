namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dummymigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyLogGroups", "GroupShortName", c => c.String());
            AddColumn("dbo.RugbyLogGroups", "IsConference", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyLogGroups", "IsConference");
            DropColumn("dbo.RugbyLogGroups", "GroupShortName");
        }
    }
}
