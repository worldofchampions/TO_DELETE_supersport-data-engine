namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        DisplayName = c.String(),
                        Sport_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sports", t => t.Sport_Id)
                .Index(t => t.Sport_Id);
            
            CreateTable(
                "dbo.Sports",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "Sport_Id", "dbo.Sports");
            DropIndex("dbo.Players", new[] { "Sport_Id" });
            DropTable("dbo.Sports");
            DropTable("dbo.Players");
        }
    }
}
