namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_CMS_override_for_RugbyTournament_Name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyTournaments", "NameCmsOverride", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyTournaments", "NameCmsOverride");
        }
    }
}
