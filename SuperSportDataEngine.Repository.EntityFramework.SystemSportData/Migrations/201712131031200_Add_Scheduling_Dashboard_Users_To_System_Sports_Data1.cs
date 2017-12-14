namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Scheduling_Dashboard_Users_To_System_Sports_Data1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerDashboardUsers", "SHA1HashPassword", c => c.String());
            DropColumn("dbo.SchedulerDashboardUsers", "PasswordPlain");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SchedulerDashboardUsers", "PasswordPlain", c => c.String());
            DropColumn("dbo.SchedulerDashboardUsers", "SHA1HashPassword");
        }
    }
}
