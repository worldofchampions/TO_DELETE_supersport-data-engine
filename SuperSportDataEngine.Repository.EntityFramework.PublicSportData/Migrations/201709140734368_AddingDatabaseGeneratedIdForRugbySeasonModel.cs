namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDatabaseGeneratedIdForRugbySeasonModel : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.RugbySeasons");
            AlterColumn("dbo.RugbySeasons", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.RugbySeasons", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.RugbySeasons");
            AlterColumn("dbo.RugbySeasons", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.RugbySeasons", "Id");
        }
    }
}
