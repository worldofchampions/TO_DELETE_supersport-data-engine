namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Motorsport_Add_IsDisabledInbound_For_Races : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MotorsportRaces", "IsDisabledInbound", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MotorsportRaces", "IsDisabledInbound");
        }
    }
}
