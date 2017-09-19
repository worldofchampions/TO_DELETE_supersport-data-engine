namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Deleted_temporary_example_tables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Players", "Sport_Id", "dbo.Sports");
            DropIndex("dbo.Players", new[] { "Sport_Id" });
            DropTable("dbo.Players");
            DropTable("dbo.Sports");
            DropTable("dbo.Logs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        GroupShortName = c.String(),
                        LogName = c.String(),
                        LeagueName = c.String(),
                        TeamShortName = c.String(),
                        Team = c.String(),
                        TeamID = c.String(),
                        Position = c.Int(nullable: false),
                        Played = c.Int(nullable: false),
                        Won = c.Int(nullable: false),
                        Drew = c.Int(nullable: false),
                        Lost = c.Int(nullable: false),
                        PointsFor = c.Double(nullable: false),
                        PointsAgainst = c.Double(nullable: false),
                        BonusPoints = c.Double(nullable: false),
                        PointsDifference = c.Double(nullable: false),
                        Points = c.Double(nullable: false),
                        Sport = c.Int(nullable: false),
                        NetRunRate = c.String(),
                        Batting = c.String(),
                        Bowling = c.String(),
                        CricketBonus = c.String(),
                        NoResult = c.String(),
                        rank = c.Int(nullable: false),
                        ConferenceRank = c.Int(nullable: false),
                        CombinedRank = c.Int(nullable: false),
                        HomeGroup = c.String(),
                        IsConference = c.Int(nullable: false),
                        TriesFor = c.Int(nullable: false),
                        TriesAgainst = c.Int(nullable: false),
                        TriesBonusPoints = c.Int(nullable: false),
                        LossBonusPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sports",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        DisplayName = c.String(),
                        Sport_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Players", "Sport_Id");
            AddForeignKey("dbo.Players", "Sport_Id", "dbo.Sports", "Id");
        }
    }
}
