namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addding_MotorCar_Table : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.MotorLeagues", "Unique_Slug");
            CreateTable(
                "dbo.MotorCars",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProviderCarId = c.Int(nullable: false),
                        CarNumber = c.Int(nullable: false),
                        CarDisplayNumber = c.Int(nullable: false),
                        MakeId = c.Int(nullable: false),
                        MakeName = c.String(),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.MotorLeagues", "Slug", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MotorLeagues", "Slug", c => c.String());
            DropTable("dbo.MotorCars");
            CreateIndex("dbo.MotorLeagues", "Slug", unique: true, name: "Unique_Slug");
        }
    }
}
