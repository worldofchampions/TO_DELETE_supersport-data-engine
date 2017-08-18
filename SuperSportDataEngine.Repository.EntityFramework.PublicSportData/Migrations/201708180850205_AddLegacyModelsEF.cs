namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLegacyModelsEF : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LegacyAuthFeedConsumers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        AllowAll = c.Boolean(nullable: false),
                        AuthKey = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LegacyAccessItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MethodAccess = c.String(),
                        Sport = c.String(),
                        Tournament = c.String(),
                        LegacyAuthFeedConsumer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LegacyAuthFeedConsumers", t => t.LegacyAuthFeedConsumer_Id)
                .Index(t => t.LegacyAuthFeedConsumer_Id);
            
            CreateTable(
                "dbo.LegacyMethodAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LegacyZoneSites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Sport = c.Int(nullable: false),
                        Feed = c.String(),
                        Url = c.String(),
                        Variable = c.Int(nullable: false),
                        Server = c.Int(nullable: false),
                        Folder = c.String(),
                        ImageType = c.String(),
                        FullUrl = c.String(),
                        Blog = c.Int(nullable: false),
                        Ss_folder = c.String(),
                        Display_Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LegacyAccessItems", "LegacyAuthFeedConsumer_Id", "dbo.LegacyAuthFeedConsumers");
            DropIndex("dbo.LegacyAccessItems", new[] { "LegacyAuthFeedConsumer_Id" });
            DropTable("dbo.LegacyZoneSites");
            DropTable("dbo.LegacyMethodAccesses");
            DropTable("dbo.LegacyAccessItems");
            DropTable("dbo.LegacyAuthFeedConsumers");
        }
    }
}
