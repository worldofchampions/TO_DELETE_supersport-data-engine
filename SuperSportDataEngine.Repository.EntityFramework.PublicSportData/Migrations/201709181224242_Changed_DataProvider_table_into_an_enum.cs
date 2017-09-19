namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changed_DataProvider_table_into_an_enum : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RugbyTeams", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbyFixtures", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbyTournaments", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbyVenues", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbySeasons", "DataProvider_Id", "dbo.DataProviders");
            DropIndex("dbo.DataProviders", "Unique_Code");
            DropIndex("dbo.RugbyFixtures", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyTeams", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyTournaments", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyVenues", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbySeasons", new[] { "DataProvider_Id" });
            AddColumn("dbo.RugbyFixtures", "DataProvider", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyTeams", "DataProvider", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyTournaments", "DataProvider", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyVenues", "DataProvider", c => c.Int(nullable: false));
            AddColumn("dbo.RugbySeasons", "DataProvider", c => c.Int(nullable: false));
            DropColumn("dbo.RugbyFixtures", "DataProvider_Id");
            DropColumn("dbo.RugbyTeams", "DataProvider_Id");
            DropColumn("dbo.RugbyTournaments", "DataProvider_Id");
            DropColumn("dbo.RugbyVenues", "DataProvider_Id");
            DropColumn("dbo.RugbySeasons", "DataProvider_Id");
            DropTable("dbo.DataProviders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DataProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 450),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.RugbySeasons", "DataProvider_Id", c => c.Int());
            AddColumn("dbo.RugbyVenues", "DataProvider_Id", c => c.Int());
            AddColumn("dbo.RugbyTournaments", "DataProvider_Id", c => c.Int());
            AddColumn("dbo.RugbyTeams", "DataProvider_Id", c => c.Int());
            AddColumn("dbo.RugbyFixtures", "DataProvider_Id", c => c.Int());
            DropColumn("dbo.RugbySeasons", "DataProvider");
            DropColumn("dbo.RugbyVenues", "DataProvider");
            DropColumn("dbo.RugbyTournaments", "DataProvider");
            DropColumn("dbo.RugbyTeams", "DataProvider");
            DropColumn("dbo.RugbyFixtures", "DataProvider");
            CreateIndex("dbo.RugbySeasons", "DataProvider_Id");
            CreateIndex("dbo.RugbyVenues", "DataProvider_Id");
            CreateIndex("dbo.RugbyTournaments", "DataProvider_Id");
            CreateIndex("dbo.RugbyTeams", "DataProvider_Id");
            CreateIndex("dbo.RugbyFixtures", "DataProvider_Id");
            CreateIndex("dbo.DataProviders", "Code", unique: true, name: "Unique_Code");
            AddForeignKey("dbo.RugbySeasons", "DataProvider_Id", "dbo.DataProviders", "Id");
            AddForeignKey("dbo.RugbyVenues", "DataProvider_Id", "dbo.DataProviders", "Id");
            AddForeignKey("dbo.RugbyTournaments", "DataProvider_Id", "dbo.DataProviders", "Id");
            AddForeignKey("dbo.RugbyFixtures", "DataProvider_Id", "dbo.DataProviders", "Id");
            AddForeignKey("dbo.RugbyTeams", "DataProvider_Id", "dbo.DataProviders", "Id");
        }
    }
}
