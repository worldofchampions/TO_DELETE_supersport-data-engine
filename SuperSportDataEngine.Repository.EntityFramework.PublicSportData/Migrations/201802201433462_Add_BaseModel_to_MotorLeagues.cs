namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_BaseModel_to_MotorLeagues : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MotorLeagues", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.MotorLeagues", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MotorLeagues", "TimestampUpdated");
            DropColumn("dbo.MotorLeagues", "TimestampCreated");
        }
    }
}
