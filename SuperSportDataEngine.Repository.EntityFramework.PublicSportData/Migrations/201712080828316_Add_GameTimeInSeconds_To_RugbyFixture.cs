namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_GameTimeInSeconds_To_RugbyFixture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyFixtures", "GameTimeInSeconds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyFixtures", "GameTimeInSeconds");
        }
    }
}
