namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_GroupShortName_and_IsConference_Into_RugbyLogGroup : DbMigration
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
