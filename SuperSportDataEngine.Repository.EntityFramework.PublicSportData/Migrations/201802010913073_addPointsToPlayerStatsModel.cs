namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPointsToPlayerStatsModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyPlayerStatistics", "TotalPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyPlayerStatistics", "TotalPoints");
        }
    }
}
