namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddLogsModel : DbMigration
    {
        public override void Up()
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
        }

        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}