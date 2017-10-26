namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rescaffolded_RugbyLogGroup_and_rugby_events_related_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyEventTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EventCode = c.Int(nullable: false),
                        EventName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.EventCode, unique: true, name: "Unique_EventCode");
            
            CreateTable(
                "dbo.RugbyEventTypeProviderMappings",
                c => new
                    {
                        DataProvider = c.Int(nullable: false),
                        ProviderEventTypeId = c.Int(nullable: false),
                        ProviderEventName = c.String(nullable: false),
                        RugbyEventTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.DataProvider, t.ProviderEventTypeId })
                .ForeignKey("dbo.RugbyEventTypes", t => t.RugbyEventTypeId, cascadeDelete: true)
                .Index(t => t.RugbyEventTypeId);
            
            CreateTable(
                "dbo.RugbyMatchEvents",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        RugbyFixtureId = c.Guid(nullable: false),
                        RugbyEventTypeId = c.Guid(nullable: false),
                        RugbyTeamId = c.Guid(nullable: false),
                        GameTimeInSeconds = c.Int(nullable: false),
                        GameTimeInMinutes = c.Int(nullable: false),
                        EventValue = c.Single(nullable: false),
                        RugbyPlayer1_Id = c.Guid(),
                        RugbyPlayer2_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RugbyEventTypes", t => t.RugbyEventTypeId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyFixtures", t => t.RugbyFixtureId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyPlayers", t => t.RugbyPlayer1_Id)
                .ForeignKey("dbo.RugbyPlayers", t => t.RugbyPlayer2_Id)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeamId, cascadeDelete: true)
                .Index(t => t.RugbyFixtureId)
                .Index(t => t.RugbyEventTypeId)
                .Index(t => t.RugbyTeamId)
                .Index(t => t.RugbyPlayer1_Id)
                .Index(t => t.RugbyPlayer2_Id);
            
            AddColumn("dbo.RugbyCommentaries", "RugbyEventType_Id", c => c.Guid());
            AddColumn("dbo.RugbyLogGroups", "GroupShortName", c => c.String());
            AddColumn("dbo.RugbyLogGroups", "IsConference", c => c.Boolean(nullable: false));
            CreateIndex("dbo.RugbyCommentaries", "RugbyEventType_Id");
            AddForeignKey("dbo.RugbyCommentaries", "RugbyEventType_Id", "dbo.RugbyEventTypes", "Id");
            DropColumn("dbo.RugbyCommentaries", "ProviderEventTypeId");
            DropColumn("dbo.RugbyCommentaries", "DataProvider");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RugbyCommentaries", "DataProvider", c => c.Int(nullable: false));
            AddColumn("dbo.RugbyCommentaries", "ProviderEventTypeId", c => c.Int());
            DropForeignKey("dbo.RugbyMatchEvents", "RugbyTeamId", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyMatchEvents", "RugbyPlayer2_Id", "dbo.RugbyPlayers");
            DropForeignKey("dbo.RugbyMatchEvents", "RugbyPlayer1_Id", "dbo.RugbyPlayers");
            DropForeignKey("dbo.RugbyMatchEvents", "RugbyFixtureId", "dbo.RugbyFixtures");
            DropForeignKey("dbo.RugbyMatchEvents", "RugbyEventTypeId", "dbo.RugbyEventTypes");
            DropForeignKey("dbo.RugbyEventTypeProviderMappings", "RugbyEventTypeId", "dbo.RugbyEventTypes");
            DropForeignKey("dbo.RugbyCommentaries", "RugbyEventType_Id", "dbo.RugbyEventTypes");
            DropIndex("dbo.RugbyMatchEvents", new[] { "RugbyPlayer2_Id" });
            DropIndex("dbo.RugbyMatchEvents", new[] { "RugbyPlayer1_Id" });
            DropIndex("dbo.RugbyMatchEvents", new[] { "RugbyTeamId" });
            DropIndex("dbo.RugbyMatchEvents", new[] { "RugbyEventTypeId" });
            DropIndex("dbo.RugbyMatchEvents", new[] { "RugbyFixtureId" });
            DropIndex("dbo.RugbyEventTypeProviderMappings", new[] { "RugbyEventTypeId" });
            DropIndex("dbo.RugbyEventTypes", "Unique_EventCode");
            DropIndex("dbo.RugbyCommentaries", new[] { "RugbyEventType_Id" });
            DropColumn("dbo.RugbyLogGroups", "IsConference");
            DropColumn("dbo.RugbyLogGroups", "GroupShortName");
            DropColumn("dbo.RugbyCommentaries", "RugbyEventType_Id");
            DropTable("dbo.RugbyMatchEvents");
            DropTable("dbo.RugbyEventTypeProviderMappings");
            DropTable("dbo.RugbyEventTypes");
        }
    }
}
