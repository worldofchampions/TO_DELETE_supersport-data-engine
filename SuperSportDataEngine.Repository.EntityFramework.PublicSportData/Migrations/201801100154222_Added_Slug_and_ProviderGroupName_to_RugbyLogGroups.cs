namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Slug_and_ProviderGroupName_to_RugbyLogGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyLogGroups", "ProviderGroupName", c => c.String());
            AddColumn("dbo.RugbyLogGroups", "Slug", c => c.String(nullable: false, maxLength: 450));
            CreateIndex("dbo.RugbyLogGroups", "Slug", unique: true, name: "Unique_Slug");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RugbyLogGroups", "Unique_Slug");
            DropColumn("dbo.RugbyLogGroups", "Slug");
            DropColumn("dbo.RugbyLogGroups", "ProviderGroupName");
        }
    }
}
