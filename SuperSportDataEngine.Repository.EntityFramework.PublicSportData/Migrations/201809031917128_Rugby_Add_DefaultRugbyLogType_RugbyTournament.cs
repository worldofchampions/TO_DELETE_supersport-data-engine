namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_Add_DefaultRugbyLogType_RugbyTournament : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyTournaments", "DefaultRugbyLogType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyTournaments", "DefaultRugbyLogType");
        }
    }
}
