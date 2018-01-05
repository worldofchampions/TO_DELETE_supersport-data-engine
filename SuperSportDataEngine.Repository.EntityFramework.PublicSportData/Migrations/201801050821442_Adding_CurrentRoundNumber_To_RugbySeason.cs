namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_CurrentRoundNumber_To_RugbySeason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbySeasons", "CurrentRoundNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbySeasons", "CurrentRoundNumber");
        }
    }
}
