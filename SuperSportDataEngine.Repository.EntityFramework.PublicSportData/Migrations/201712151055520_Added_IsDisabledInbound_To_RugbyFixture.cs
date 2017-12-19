namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_IsDisabledInbound_To_RugbyFixture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "IsDisabledInbound", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "IsDisabledInbound");
        }
    }
}
