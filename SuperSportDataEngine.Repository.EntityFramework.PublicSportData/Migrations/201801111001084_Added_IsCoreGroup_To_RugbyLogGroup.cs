namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_IsCoreGroup_To_RugbyLogGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyLogGroups", "IsCoreGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyLogGroups", "IsCoreGroup");
        }
    }
}
