namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rugby_Add_ProviderNameAlternate_To_RugbyTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyTeams", "ProviderNameAlternate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyTeams", "ProviderNameAlternate");
        }
    }
}
