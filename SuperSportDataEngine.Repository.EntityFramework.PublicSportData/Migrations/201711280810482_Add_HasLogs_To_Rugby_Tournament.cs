namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_HasLogs_To_Rugby_Tournament : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyTournaments", "HasLogs", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyTournaments", "HasLogs");
        }
    }
}
