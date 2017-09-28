namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_ProviderLogGroupId_Nullable_Int_And_Optional : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RugbyLogGroups", "Seek_ProviderLogGroupId");
            AlterColumn("dbo.RugbyLogGroups", "ProviderLogGroupId", c => c.Int());
            CreateIndex("dbo.RugbyLogGroups", "ProviderLogGroupId", name: "Seek_ProviderLogGroupId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RugbyLogGroups", "Seek_ProviderLogGroupId");
            AlterColumn("dbo.RugbyLogGroups", "ProviderLogGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.RugbyLogGroups", "ProviderLogGroupId", name: "Seek_ProviderLogGroupId");
        }
    }
}
