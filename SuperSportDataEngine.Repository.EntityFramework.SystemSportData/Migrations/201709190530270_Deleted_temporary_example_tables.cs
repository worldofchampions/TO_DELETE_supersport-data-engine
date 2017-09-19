namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Deleted_temporary_example_tables : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.SportTournaments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SportTournaments",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TournamentIndex = c.Int(nullable: false),
                        TournamentName = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
