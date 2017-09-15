namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_RugbyFixture_RugbyTeam_RugbyVenue_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyFixtures",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyFixtureId = c.Int(nullable: false),
                        ProviderFixtureId = c.Int(nullable: false),
                        StartDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        RugbyFixtureStatus = c.Int(nullable: false),
                        AwayTeamTeam_Id = c.Guid(),
                        DataProvider_Id = c.Int(),
                        HomeTeam_Id = c.Guid(),
                        RugbyTournament_Id = c.Guid(),
                        RugbyVenue_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RugbyTeams", t => t.AwayTeamTeam_Id)
                .ForeignKey("dbo.DataProviders", t => t.DataProvider_Id)
                .ForeignKey("dbo.RugbyTeams", t => t.HomeTeam_Id)
                .ForeignKey("dbo.RugbyTournaments", t => t.RugbyTournament_Id)
                .ForeignKey("dbo.RugbyVenues", t => t.RugbyVenue_Id)
                .Index(t => t.LegacyFixtureId, unique: true, name: "Unique_LegacyFixtureId")
                .Index(t => t.ProviderFixtureId, name: "Seek_ProviderFixtureId")
                .Index(t => t.AwayTeamTeam_Id)
                .Index(t => t.DataProvider_Id)
                .Index(t => t.HomeTeam_Id)
                .Index(t => t.RugbyTournament_Id)
                .Index(t => t.RugbyVenue_Id);
            
            CreateTable(
                "dbo.RugbyTeams",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LegacyTeamId = c.Int(nullable: false),
                        ProviderTeamId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Abbreviation = c.String(),
                        LogoUrl = c.String(),
                        DataProvider_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataProviders", t => t.DataProvider_Id)
                .Index(t => t.LegacyTeamId, unique: true, name: "Unique_LegacyTeamId")
                .Index(t => t.ProviderTeamId, name: "Seek_ProviderTeamId")
                .Index(t => t.DataProvider_Id);
            
            CreateTable(
                "dbo.RugbyVenues",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderVenueId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DataProvider_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataProviders", t => t.DataProvider_Id)
                .Index(t => t.ProviderVenueId, name: "Seek_ProviderVenueId")
                .Index(t => t.DataProvider_Id);
            
            AddColumn("dbo.RugbySeasons", "DataProvider_Id", c => c.Int());
            CreateIndex("dbo.RugbySeasons", "DataProvider_Id");
            AddForeignKey("dbo.RugbySeasons", "DataProvider_Id", "dbo.DataProviders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbySeasons", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbyFixtures", "RugbyVenue_Id", "dbo.RugbyVenues");
            DropForeignKey("dbo.RugbyVenues", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbyFixtures", "RugbyTournament_Id", "dbo.RugbyTournaments");
            DropForeignKey("dbo.RugbyFixtures", "HomeTeam_Id", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyFixtures", "DataProvider_Id", "dbo.DataProviders");
            DropForeignKey("dbo.RugbyFixtures", "AwayTeamTeam_Id", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyTeams", "DataProvider_Id", "dbo.DataProviders");
            DropIndex("dbo.RugbySeasons", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyVenues", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyVenues", "Seek_ProviderVenueId");
            DropIndex("dbo.RugbyTeams", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyTeams", "Seek_ProviderTeamId");
            DropIndex("dbo.RugbyTeams", "Unique_LegacyTeamId");
            DropIndex("dbo.RugbyFixtures", new[] { "RugbyVenue_Id" });
            DropIndex("dbo.RugbyFixtures", new[] { "RugbyTournament_Id" });
            DropIndex("dbo.RugbyFixtures", new[] { "HomeTeam_Id" });
            DropIndex("dbo.RugbyFixtures", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyFixtures", new[] { "AwayTeamTeam_Id" });
            DropIndex("dbo.RugbyFixtures", "Seek_ProviderFixtureId");
            DropIndex("dbo.RugbyFixtures", "Unique_LegacyFixtureId");
            DropColumn("dbo.RugbySeasons", "DataProvider_Id");
            DropTable("dbo.RugbyVenues");
            DropTable("dbo.RugbyTeams");
            DropTable("dbo.RugbyFixtures");
        }
    }
}
