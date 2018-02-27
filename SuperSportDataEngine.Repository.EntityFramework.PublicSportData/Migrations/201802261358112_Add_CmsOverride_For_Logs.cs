namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CmsOverride_For_Logs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFlatLogs", "IsCmsOverrideModeActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.RugbyGroupedLogs", "IsCmsOverrideModeActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyGroupedLogs", "IsCmsOverrideModeActive");
            DropColumn("dbo.RugbyFlatLogs", "IsCmsOverrideModeActive");
        }
    }
}
