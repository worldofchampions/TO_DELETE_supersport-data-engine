namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDatabaseGeneratedIdFor_RugbySeason_RugbyTournament : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments");
            DropPrimaryKey("dbo.RugbySeasons");
            DropPrimaryKey("dbo.RugbyTournaments");
            AlterColumn("dbo.RugbySeasons", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("dbo.RugbyTournaments", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.RugbySeasons", "Id");
            AddPrimaryKey("dbo.RugbyTournaments", "Id");
            AddForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments");
            DropPrimaryKey("dbo.RugbyTournaments");
            DropPrimaryKey("dbo.RugbySeasons");
            AlterColumn("dbo.RugbyTournaments", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.RugbySeasons", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.RugbyTournaments", "Id");
            AddPrimaryKey("dbo.RugbySeasons", "Id");
            AddForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments", "Id");
        }
    }
}
