namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Renamed_RugbyPlayerLineup_field_ShirtNumber_to_JerseyNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyPlayerLineups", "JerseyNumber", c => c.Int(nullable: false));
            DropColumn("dbo.RugbyPlayerLineups", "ShirtNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RugbyPlayerLineups", "ShirtNumber", c => c.Int(nullable: false));
            DropColumn("dbo.RugbyPlayerLineups", "JerseyNumber");
        }
    }
}
