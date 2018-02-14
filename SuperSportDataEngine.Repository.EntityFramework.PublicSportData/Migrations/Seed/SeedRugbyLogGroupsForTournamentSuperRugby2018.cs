namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class SeedRugbyLogGroupsForTournamentSuperRugby2018
    {
        private const string SlugHierarchyLevel0Overall = "SuperRugby-2018-HL0-OverallStandings";
        private const string SlugHierarchyLevel1Australian = "SuperRugby-2018-HL1-AustralianConference";
        private const string SlugHierarchyLevel1NewZealand = "SuperRugby-2018-HL1-NewZealandConference";
        private const string SlugHierarchyLevel1SouthAfrican = "SuperRugby-2018-HL1-SouthAfricanConference";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdSuperRugby);

                var rugbySeason = context.RugbySeasons.Single(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0Overall, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Overall Standings", GroupShortName = "Overall Standings" },
                    // LogGroups for "GroupStandings", GroupHierarchyLevel: 1.
                    // Although this group structure for Super Rugby 2018 season consists of conferences,
                    // the children groups are marked as NOT conferences on purpose.
                    // Due to the logic on HD Active decoders 
                    // (if IsConference == true use CombinedRank else use Position)
                    // We want HD Active to use Position, so we mark it as false.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Australian, ProviderLogGroupId = 1, ProviderGroupName = "Australian Conference", GroupName = "Australian Conference", GroupShortName = "Aus Group" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1NewZealand, ProviderLogGroupId = 2, ProviderGroupName = "New Zealand Conference", GroupName = "New Zealand Conference", GroupShortName = "NZ Group" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1SouthAfrican, ProviderLogGroupId = 3, ProviderGroupName = "South African Conference", GroupName = "South African Conference", GroupShortName = "Africa Group" }
                );

                context.SaveChanges();

                // Assign log groups parent-child relationships (hierarchy).
                // Relationships for "GroupStandings", GroupHierarchyLevel: 1.
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Australian).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Overall);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1NewZealand).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Overall);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1SouthAfrican).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Overall);

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }
    }
}
