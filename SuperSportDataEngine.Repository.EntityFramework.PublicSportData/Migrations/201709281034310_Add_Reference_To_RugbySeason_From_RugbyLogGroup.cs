namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Reference_To_RugbySeason_From_RugbyLogGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyLogGroups", "RugbySeason_Id", c => c.Guid());
            CreateIndex("dbo.RugbyLogGroups", "RugbySeason_Id");
            AddForeignKey("dbo.RugbyLogGroups", "RugbySeason_Id", "dbo.RugbySeasons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RugbyLogGroups", "RugbySeason_Id", "dbo.RugbySeasons");
            DropIndex("dbo.RugbyLogGroups", new[] { "RugbySeason_Id" });
            DropColumn("dbo.RugbyLogGroups", "RugbySeason_Id");
        }
    }
}
