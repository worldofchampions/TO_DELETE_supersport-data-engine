namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changed_PositionName_From_Int_To_String : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RugbyPlayerLineups", "PositionName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RugbyPlayerLineups", "PositionName", c => c.Int(nullable: false));
        }
    }
}
