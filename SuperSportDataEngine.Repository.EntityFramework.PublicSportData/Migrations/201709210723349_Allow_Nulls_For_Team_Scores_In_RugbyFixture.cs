namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Allow_Nulls_For_Team_Scores_In_RugbyFixture : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RugbyFixtures", "TeamAScore", c => c.Int());
            AlterColumn("dbo.RugbyFixtures", "TeamBScore", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RugbyFixtures", "TeamBScore", c => c.Int(nullable: false));
            AlterColumn("dbo.RugbyFixtures", "TeamAScore", c => c.Int(nullable: false));
        }
    }
}
