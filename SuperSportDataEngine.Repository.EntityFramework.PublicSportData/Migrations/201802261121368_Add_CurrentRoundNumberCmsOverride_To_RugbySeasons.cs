namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CurrentRoundNumberCmsOverride_To_RugbySeasons : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbySeasons", "CurrentRoundNumberCmsOverride", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbySeasons", "CurrentRoundNumberCmsOverride");
        }
    }
}
