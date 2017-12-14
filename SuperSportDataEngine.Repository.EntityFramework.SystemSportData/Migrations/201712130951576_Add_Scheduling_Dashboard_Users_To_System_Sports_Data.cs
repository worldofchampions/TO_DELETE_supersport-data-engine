namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Scheduling_Dashboard_Users_To_System_Sports_Data : DbMigration
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
