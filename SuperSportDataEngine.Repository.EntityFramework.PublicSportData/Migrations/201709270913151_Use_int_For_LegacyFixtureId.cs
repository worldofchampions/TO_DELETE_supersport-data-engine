namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Use_int_For_LegacyFixtureId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RugbyFixtures", "Unique_LegacyFixtureId");
            DropColumn("dbo.RugbyFixtures", "LegacyFixtureId");

            AddColumn("dbo.RugbyFixtures", "LegacyFixtureIdTmp", c => c.Int(nullable: false, identity: true));
            CreateIndex("dbo.RugbyFixtures", "LegacyFixtureIdTmp", unique: true, name: "Unique_LegacyFixtureId");

            RenameColumn("dbo.RugbyFixtures", "LegacyFixtureIdTmp", "LegacyFixtureId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RugbyFixtures", "Unique_LegacyFixtureId");
            AlterColumn("dbo.RugbyFixtures", "LegacyFixtureId", c => c.Long(nullable: false));
            CreateIndex("dbo.RugbyFixtures", "LegacyFixtureId", unique: true, name: "Unique_LegacyFixtureId");
        }
    }
}
