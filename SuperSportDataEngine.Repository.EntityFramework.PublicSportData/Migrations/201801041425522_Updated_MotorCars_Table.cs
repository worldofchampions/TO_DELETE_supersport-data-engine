namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_MotorCars_Table : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.MotorCars");
            AlterColumn("dbo.MotorCars", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.MotorCars", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.MotorCars");
            AlterColumn("dbo.MotorCars", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.MotorCars", "Id");
        }
    }
}
