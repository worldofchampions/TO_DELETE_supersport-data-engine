namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTournamentIndexToSportTournament : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SportTournaments", "TournamentIndex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SportTournaments", "TournamentIndex");
        }
    }
}
