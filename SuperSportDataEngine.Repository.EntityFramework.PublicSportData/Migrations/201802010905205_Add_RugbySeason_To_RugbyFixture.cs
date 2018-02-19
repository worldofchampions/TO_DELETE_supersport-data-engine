namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_RugbySeason_To_RugbyFixture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "RugbySeason_Id", c => c.Guid());
            CreateIndex("dbo.RugbyFixtures", "RugbySeason_Id");
            AddForeignKey("dbo.RugbyFixtures", "RugbySeason_Id", "dbo.RugbySeasons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyFixtures", "RugbySeason_Id", "dbo.RugbySeasons");
            DropIndex("dbo.RugbyFixtures", new[] { "RugbySeason_Id" });
            DropColumn("dbo.RugbyFixtures", "RugbySeason_Id");
        }
    }
}
