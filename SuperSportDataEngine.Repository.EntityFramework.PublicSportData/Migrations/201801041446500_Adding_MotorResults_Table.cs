namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_MotorResults_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MotorRaceResults",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DriverId = c.Guid(nullable: false),
                        LapsCompleted = c.Int(nullable: false),
                        LapsLed = c.Int(nullable: false),
                        LapsBehind = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        StartingPosition = c.Int(nullable: false),
                        IsFastest = c.Boolean(nullable: false),
                        FinishingTime_Hours = c.Int(nullable: false),
                        FinishingTime_Minutes = c.Int(nullable: false),
                        FinishingTime_Seconds = c.Int(nullable: false),
                        FinishingTime_Milliseconds = c.Int(nullable: false),
                        DriverTotalPoints = c.Int(nullable: false),
                        DriverBonusPoints = c.Int(nullable: false),
                        DriverPenaltyPoints = c.Int(nullable: false),
                        OwnerTotalPoints = c.Int(nullable: false),
                        OwnerBonusPoints = c.Int(nullable: false),
                        OwnerPenaltyPoints = c.Int(nullable: false),
                        TimestampCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        TimestampUpdated = c.DateTimeOffset(nullable: false, precision: 7),
                        MotorDriver_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotorDrivers", t => t.MotorDriver_Id)
                .Index(t => t.MotorDriver_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorRaceResults", "MotorDriver_Id", "dbo.MotorDrivers");
            DropIndex("dbo.MotorRaceResults", new[] { "MotorDriver_Id" });
            DropTable("dbo.MotorRaceResults");
        }
    }
}
