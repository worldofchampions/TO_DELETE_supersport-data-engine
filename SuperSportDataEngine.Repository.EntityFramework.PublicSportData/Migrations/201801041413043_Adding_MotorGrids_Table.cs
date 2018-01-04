namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_MotorGrids_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MotorGrids",
                c => new
                    {
                        RaceId = c.Guid(nullable: false),
                        DriverId = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        QualifyingTime_Hours = c.Int(nullable: false),
                        QualifyingTime_Minutes = c.Int(nullable: false),
                        QualifyingTime_Seconds = c.Int(nullable: false),
                        QualifyingTime_Milliseconds = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorDriver_Id = c.Guid(),
                        MotorRace_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.RaceId, t.DriverId })
                .ForeignKey("dbo.MotorDrivers", t => t.MotorDriver_Id)
                .ForeignKey("dbo.MotorRaces", t => t.MotorRace_Id)
                .Index(t => t.MotorDriver_Id)
                .Index(t => t.MotorRace_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorGrids", "MotorRace_Id", "dbo.MotorRaces");
            DropForeignKey("dbo.MotorGrids", "MotorDriver_Id", "dbo.MotorDrivers");
            DropIndex("dbo.MotorGrids", new[] { "MotorRace_Id" });
            DropIndex("dbo.MotorGrids", new[] { "MotorDriver_Id" });
            DropTable("dbo.MotorGrids");
        }
    }
}
