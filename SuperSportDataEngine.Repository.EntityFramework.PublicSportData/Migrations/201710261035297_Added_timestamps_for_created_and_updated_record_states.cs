namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_timestamps_for_created_and_updated_record_states : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RugbyCommentaries", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyCommentaries", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyEventTypes", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyEventTypes", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyFixtures", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyFixtures", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyTournaments", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyTournaments", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyVenues", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyVenues", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyTeams", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyTeams", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyPlayers", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyPlayers", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyEventTypeProviderMappings", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyEventTypeProviderMappings", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyFlatLogs", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyFlatLogs", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbySeasons", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbySeasons", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyGroupedLogs", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyGroupedLogs", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyLogGroups", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyLogGroups", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyMatchEvents", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyMatchEvents", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyMatchStatistics", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyMatchStatistics", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyPlayerLineups", "TimestampCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RugbyPlayerLineups", "TimestampUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RugbyPlayerLineups", "TimestampUpdated");
            DropColumn("dbo.RugbyPlayerLineups", "TimestampCreated");
            DropColumn("dbo.RugbyMatchStatistics", "TimestampUpdated");
            DropColumn("dbo.RugbyMatchStatistics", "TimestampCreated");
            DropColumn("dbo.RugbyMatchEvents", "TimestampUpdated");
            DropColumn("dbo.RugbyMatchEvents", "TimestampCreated");
            DropColumn("dbo.RugbyLogGroups", "TimestampUpdated");
            DropColumn("dbo.RugbyLogGroups", "TimestampCreated");
            DropColumn("dbo.RugbyGroupedLogs", "TimestampUpdated");
            DropColumn("dbo.RugbyGroupedLogs", "TimestampCreated");
            DropColumn("dbo.RugbySeasons", "TimestampUpdated");
            DropColumn("dbo.RugbySeasons", "TimestampCreated");
            DropColumn("dbo.RugbyFlatLogs", "TimestampUpdated");
            DropColumn("dbo.RugbyFlatLogs", "TimestampCreated");
            DropColumn("dbo.RugbyEventTypeProviderMappings", "TimestampUpdated");
            DropColumn("dbo.RugbyEventTypeProviderMappings", "TimestampCreated");
            DropColumn("dbo.RugbyPlayers", "TimestampUpdated");
            DropColumn("dbo.RugbyPlayers", "TimestampCreated");
            DropColumn("dbo.RugbyTeams", "TimestampUpdated");
            DropColumn("dbo.RugbyTeams", "TimestampCreated");
            DropColumn("dbo.RugbyVenues", "TimestampUpdated");
            DropColumn("dbo.RugbyVenues", "TimestampCreated");
            DropColumn("dbo.RugbyTournaments", "TimestampUpdated");
            DropColumn("dbo.RugbyTournaments", "TimestampCreated");
            DropColumn("dbo.RugbyFixtures", "TimestampUpdated");
            DropColumn("dbo.RugbyFixtures", "TimestampCreated");
            DropColumn("dbo.RugbyEventTypes", "TimestampUpdated");
            DropColumn("dbo.RugbyEventTypes", "TimestampCreated");
            DropColumn("dbo.RugbyCommentaries", "TimestampUpdated");
            DropColumn("dbo.RugbyCommentaries", "TimestampCreated");
        }
    }
}
