namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updates_To_Dashboard_Users : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SchedulerDashboardUsers", "PasswordPlain", c => c.String());
            DropColumn("dbo.SchedulerDashboardUsers", "SHA1HashPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SchedulerDashboardUsers", "SHA1HashPassword", c => c.String());
            DropColumn("dbo.SchedulerDashboardUsers", "PasswordPlain");
        }
    }
}
