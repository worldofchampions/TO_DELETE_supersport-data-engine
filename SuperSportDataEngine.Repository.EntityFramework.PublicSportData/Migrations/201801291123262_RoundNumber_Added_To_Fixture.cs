namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoundNumber_Added_To_Fixture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "RoundNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "RoundNumber");
        }
    }
}
