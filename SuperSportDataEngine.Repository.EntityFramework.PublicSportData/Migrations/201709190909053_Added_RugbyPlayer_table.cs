namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_RugbyPlayer_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyPlayers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyPlayerId = c.Int(nullable: false),
                        ProviderPlayerId = c.Int(nullable: false),
                        FullName = c.String(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        DataProvider = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProviderPlayerId, name: "Seek_ProviderPlayerId");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.RugbyPlayers", "Seek_ProviderPlayerId");
            DropTable("dbo.RugbyPlayers");
        }
    }
}
