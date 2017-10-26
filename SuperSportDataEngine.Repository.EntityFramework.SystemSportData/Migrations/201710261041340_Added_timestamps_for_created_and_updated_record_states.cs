namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_timestamps_for_created_and_updated_record_states : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LegacyAuthFeedConsumers", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyAuthFeedConsumers", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyAccessItems", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyAccessItems", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyMethodAccesses", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyMethodAccesses", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyZoneSites", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.LegacyZoneSites", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.SchedulerTrackingRugbyFixtures", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.SchedulerTrackingRugbyFixtures", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.SchedulerTrackingRugbySeasons", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.SchedulerTrackingRugbySeasons", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.SchedulerTrackingRugbyTournaments", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.SchedulerTrackingRugbyTournaments", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SchedulerTrackingRugbyTournaments", "TimestampUpdated");
            DropColumn("dbo.SchedulerTrackingRugbyTournaments", "TimestampCreated");
            DropColumn("dbo.SchedulerTrackingRugbySeasons", "TimestampUpdated");
            DropColumn("dbo.SchedulerTrackingRugbySeasons", "TimestampCreated");
            DropColumn("dbo.SchedulerTrackingRugbyFixtures", "TimestampUpdated");
            DropColumn("dbo.SchedulerTrackingRugbyFixtures", "TimestampCreated");
            DropColumn("dbo.LegacyZoneSites", "TimestampUpdated");
            DropColumn("dbo.LegacyZoneSites", "TimestampCreated");
            DropColumn("dbo.LegacyMethodAccesses", "TimestampUpdated");
            DropColumn("dbo.LegacyMethodAccesses", "TimestampCreated");
            DropColumn("dbo.LegacyAccessItems", "TimestampUpdated");
            DropColumn("dbo.LegacyAccessItems", "TimestampCreated");
            DropColumn("dbo.LegacyAuthFeedConsumers", "TimestampUpdated");
            DropColumn("dbo.LegacyAuthFeedConsumers", "TimestampCreated");
        }
    }
}
