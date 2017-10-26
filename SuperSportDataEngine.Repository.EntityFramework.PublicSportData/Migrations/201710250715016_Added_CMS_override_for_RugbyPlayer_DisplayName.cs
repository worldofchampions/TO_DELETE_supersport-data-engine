namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_CMS_override_for_RugbyPlayer_DisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyPlayers", "DisplayNameCmsOverride", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyPlayers", "DisplayNameCmsOverride");
        }
    }
}
