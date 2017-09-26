namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_grouped_logs_tables_and_other_related_changes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RugbyLogs", newName: "RugbyFlatLogs");
            CreateTable(
                "dbo.RugbyGroupedLogs",
                c => new
                    {
                        RugbyTournamentId = c.Guid(nullable: false),
                        RugbySeasonId = c.Guid(nullable: false),
                        RoundNumber = c.Int(nullable: false),
                        RugbyTeamId = c.Guid(nullable: false),
                        RugbyLogGroupId = c.Guid(nullable: false),
                        LogPosition = c.Int(nullable: false),
                        GamesPlayed = c.Int(nullable: false),
                        GamesWon = c.Int(nullable: false),
                        GamesDrawn = c.Int(nullable: false),
                        GamesLost = c.Int(nullable: false),
                        PointsFor = c.Int(nullable: false),
                        PointsAgainst = c.Int(nullable: false),
                        PointsDifference = c.Int(nullable: false),
                        TournamentPoints = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        TriesFor = c.Int(nullable: false),
                        TriesAgainst = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RugbyTournamentId, t.RugbySeasonId, t.RoundNumber, t.RugbyTeamId, t.RugbyLogGroupId })
                .ForeignKey("dbo.RugbyLogGroups", t => t.RugbyLogGroupId, cascadeDelete: true)
                .ForeignKey("dbo.RugbySeasons", t => t.RugbySeasonId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeamId, cascadeDelete: true)
                .ForeignKey("dbo.RugbyTournaments", t => t.RugbyTournamentId, cascadeDelete: true)
                .Index(t => t.RugbyTournamentId)
                .Index(t => t.RugbySeasonId)
                .Index(t => t.RugbyTeamId)
                .Index(t => t.RugbyLogGroupId);
            
            CreateTable(
                "dbo.RugbyLogGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProviderLogGroupId = c.Int(nullable: false),
                        DataProvider = c.Int(nullable: false),
                        GroupName = c.String(),
                        GroupHierarchyLevel = c.Int(nullable: false),
                        ParentRugbyLogGroup_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RugbyLogGroups", t => t.ParentRugbyLogGroup_Id)
                .Index(t => t.ProviderLogGroupId, name: "Seek_ProviderLogGroupId")
                .Index(t => t.ParentRugbyLogGroup_Id);
            
            AddColumn("dbo.RugbyFlatLogs", "LogPosition", c => c.Int(nullable: false));
            AddColumn("dbo.RugbySeasons", "RugbyLogType", c => c.Int());
            DropColumn("dbo.RugbyFlatLogs", "Position");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RugbyFlatLogs", "Position", c => c.Int(nullable: false));
            DropForeignKey("dbo.RugbyGroupedLogs", "RugbyTournamentId", "dbo.RugbyTournaments");
            DropForeignKey("dbo.RugbyGroupedLogs", "RugbyTeamId", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyGroupedLogs", "RugbySeasonId", "dbo.RugbySeasons");
            DropForeignKey("dbo.RugbyGroupedLogs", "RugbyLogGroupId", "dbo.RugbyLogGroups");
            DropForeignKey("dbo.RugbyLogGroups", "ParentRugbyLogGroup_Id", "dbo.RugbyLogGroups");
            DropIndex("dbo.RugbyLogGroups", new[] { "ParentRugbyLogGroup_Id" });
            DropIndex("dbo.RugbyLogGroups", "Seek_ProviderLogGroupId");
            DropIndex("dbo.RugbyGroupedLogs", new[] { "RugbyLogGroupId" });
            DropIndex("dbo.RugbyGroupedLogs", new[] { "RugbyTeamId" });
            DropIndex("dbo.RugbyGroupedLogs", new[] { "RugbySeasonId" });
            DropIndex("dbo.RugbyGroupedLogs", new[] { "RugbyTournamentId" });
            DropColumn("dbo.RugbySeasons", "RugbyLogType");
            DropColumn("dbo.RugbyFlatLogs", "LogPosition");
            DropTable("dbo.RugbyLogGroups");
            DropTable("dbo.RugbyGroupedLogs");
            RenameTable(name: "dbo.RugbyFlatLogs", newName: "RugbyLogs");
        }
    }
}
