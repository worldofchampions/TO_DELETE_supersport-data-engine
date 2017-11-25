namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_CmsOverrideModeIsActive_to_RugbyFixture_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "CmsOverrideModeIsActive", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "CmsOverrideModeIsActive");
        }
    }
}
