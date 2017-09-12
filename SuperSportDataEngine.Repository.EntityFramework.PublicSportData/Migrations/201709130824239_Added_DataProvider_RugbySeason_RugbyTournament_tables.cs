namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_DataProvider_RugbySeason_RugbyTournament_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 450),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true, name: "Unique_Code");
            
            CreateTable(
                "dbo.RugbySeasons",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProviderSeasonId = c.Int(nullable: false),
                        Name = c.String(),
                        IsCurrent = c.Boolean(nullable: false),
                        StartDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        RugbyTournament_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RugbyTournaments", t => t.RugbyTournament_Id)
                .Index(t => t.ProviderSeasonId, name: "Seek_ProviderSeasonId")
                .Index(t => t.RugbyTournament_Id);
            
            CreateTable(
                "dbo.RugbyTournaments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LegacyTournamentId = c.Int(nullable: false),
                        ProviderTournamentId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 450),
                        Abbreviation = c.String(),
                        LogoUrl = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        DataProvider_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataProviders", t => t.DataProvider_Id)
                .Index(t => t.LegacyTournamentId, unique: true, name: "Unique_LegacyTournamentId")
                .Index(t => t.ProviderTournamentId, name: "Seek_ProviderTournamentId")
                .Index(t => t.Slug, unique: true, name: "Unique_Slug")
                .Index(t => t.DataProvider_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments");
            DropForeignKey("dbo.RugbyTournaments", "DataProvider_Id", "dbo.DataProviders");
            DropIndex("dbo.RugbyTournaments", new[] { "DataProvider_Id" });
            DropIndex("dbo.RugbyTournaments", "Unique_Slug");
            DropIndex("dbo.RugbyTournaments", "Seek_ProviderTournamentId");
            DropIndex("dbo.RugbyTournaments", "Unique_LegacyTournamentId");
            DropIndex("dbo.RugbySeasons", new[] { "RugbyTournament_Id" });
            DropIndex("dbo.RugbySeasons", "Seek_ProviderSeasonId");
            DropIndex("dbo.DataProviders", "Unique_Code");
            DropTable("dbo.RugbyTournaments");
            DropTable("dbo.RugbySeasons");
            DropTable("dbo.DataProviders");
        }
    }
}
