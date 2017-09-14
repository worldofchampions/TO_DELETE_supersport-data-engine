namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PubblicSportsDataModelChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments");
            DropPrimaryKey("dbo.RugbyTournaments");
            AlterColumn("dbo.RugbyTournaments", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AddPrimaryKey("dbo.RugbyTournaments", "Id");
            AddForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments");
            DropPrimaryKey("dbo.RugbyTournaments");
            AlterColumn("dbo.RugbyTournaments", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.RugbyTournaments", "Id");
            AddForeignKey("dbo.RugbySeasons", "RugbyTournament_Id", "dbo.RugbyTournaments", "Id");
        }
    }
}
