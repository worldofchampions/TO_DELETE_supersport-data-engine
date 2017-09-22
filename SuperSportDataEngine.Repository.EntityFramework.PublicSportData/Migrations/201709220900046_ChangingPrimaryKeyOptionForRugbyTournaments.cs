namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangingPrimaryKeyOptionForRugbyTournaments : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RugbyTournaments", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RugbyTournaments", "Id", c => c.Guid(nullable: false, identity: true));
        }
    }
}
