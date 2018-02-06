namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_MotorRaces_Table : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MotorGrids", "MotorRace_Id", "dbo.MotorRaces");
            DropPrimaryKey("dbo.MotorRaces");
            AlterColumn("dbo.MotorRaces", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.MotorRaces", "LegacyRaceId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.MotorRaces", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.MotorRaces", "Slug", c => c.String(nullable: false, maxLength: 450));
            AddPrimaryKey("dbo.MotorRaces", "Id");
            CreateIndex("dbo.MotorRaces", "ProviderRaceId", name: "Seek_ProviderRaceId");
            CreateIndex("dbo.MotorRaces", "Slug", unique: true, name: "Unique_Slug");
            AddForeignKey("dbo.MotorGrids", "MotorRace_Id", "dbo.MotorRaces", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MotorGrids", "MotorRace_Id", "dbo.MotorRaces");
            DropIndex("dbo.MotorRaces", "Unique_Slug");
            DropIndex("dbo.MotorRaces", "Seek_ProviderRaceId");
            DropPrimaryKey("dbo.MotorRaces");
            AlterColumn("dbo.MotorRaces", "Slug", c => c.String());
            AlterColumn("dbo.MotorRaces", "Name", c => c.String());
            AlterColumn("dbo.MotorRaces", "LegacyRaceId", c => c.Int(nullable: false));
            AlterColumn("dbo.MotorRaces", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.MotorRaces", "Id");
            AddForeignKey("dbo.MotorGrids", "MotorRace_Id", "dbo.MotorRaces", "Id");
        }
    }
}
