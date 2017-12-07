namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Dashboard_User_To_Database : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchedulerDashboardUsers",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        PasswordPlain = c.String(),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Username);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SchedulerDashboardUsers");
        }
    }
}
