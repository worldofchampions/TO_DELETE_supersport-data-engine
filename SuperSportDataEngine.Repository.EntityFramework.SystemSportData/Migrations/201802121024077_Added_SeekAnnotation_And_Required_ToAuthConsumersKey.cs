namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_SeekAnnotation_And_Required_ToAuthConsumersKey : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LegacyAuthFeedConsumers", "AuthKey", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.LegacyAuthFeedConsumers", "AuthKey", name: "Seek_AuthKey");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LegacyAuthFeedConsumers", "Seek_AuthKey");
            AlterColumn("dbo.LegacyAuthFeedConsumers", "AuthKey", c => c.String());
        }
    }
}
