namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_Annotation_On_ProviderLogGroupId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RugbyLogGroups", "Seek_ProviderLogGroupId");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.RugbyLogGroups", "ProviderLogGroupId", name: "Seek_ProviderLogGroupId");
        }
    }
}
