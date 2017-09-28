namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_RugbyCommentary_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RugbyCommentaries",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        GameTimeRawSeconds = c.Int(nullable: false),
                        GameTimeRawMinutes = c.Int(nullable: false),
                        GameTimeDisplayHoursMinutesSeconds = c.String(nullable: false),
                        GameTimeDisplayMinutesSeconds = c.String(nullable: false),
                        CommentaryText = c.String(nullable: false),
                        ProviderEventTypeId = c.Int(),
                        DataProvider = c.Int(nullable: false),
                        RugbyFixture_Id = c.Guid(),
                        RugbyPlayer_Id = c.Guid(),
                        RugbyTeam_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RugbyFixtures", t => t.RugbyFixture_Id)
                .ForeignKey("dbo.RugbyPlayers", t => t.RugbyPlayer_Id)
                .ForeignKey("dbo.RugbyTeams", t => t.RugbyTeam_Id)
                .Index(t => t.RugbyFixture_Id)
                .Index(t => t.RugbyPlayer_Id)
                .Index(t => t.RugbyTeam_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyCommentaries", "RugbyTeam_Id", "dbo.RugbyTeams");
            DropForeignKey("dbo.RugbyCommentaries", "RugbyPlayer_Id", "dbo.RugbyPlayers");
            DropForeignKey("dbo.RugbyCommentaries", "RugbyFixture_Id", "dbo.RugbyFixtures");
            DropIndex("dbo.RugbyCommentaries", new[] { "RugbyTeam_Id" });
            DropIndex("dbo.RugbyCommentaries", new[] { "RugbyPlayer_Id" });
            DropIndex("dbo.RugbyCommentaries", new[] { "RugbyFixture_Id" });
            DropTable("dbo.RugbyCommentaries");
        }
    }
}
