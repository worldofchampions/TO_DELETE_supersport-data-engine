namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class SeedRugbyLogGroupsForTournamentSuperRugby2017
    {
        private const string SlugHierarchyLevel0Overall = "SuperRugby-2017-HL0-OverallStandings";
        private const string SlugHierarchyLevel1Australasian = "SuperRugby-2017-HL1-SecondaryGroupStandings-AustralasianConference";
        private const string SlugHierarchyLevel1SouthAfrican = "SuperRugby-2017-HL1-SecondaryGroupStandings-SouthAfricanConference";
        private const string SlugHierarchyLevel2Australian = "SuperRugby-2017-HL2-GroupStandings-AustralianConference";
        private const string SlugHierarchyLevel2Zealand = "SuperRugby-2017-HL2-GroupStandings-NewZealandConference";
        private const string SlugHierarchyLevel2Africa1 = "SuperRugby-2017-HL2-GroupStandings-Africa1Conference";
        private const string SlugHierarchyLevel2Africa2 = "SuperRugby-2017-HL2-GroupStandings-Africa2Conference";

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
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2017);

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0Overall, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Overall Standings", GroupShortName = "Overall Standings" },
                    // LogGroups for "SecondaryGroupStandings", GroupHierarchyLevel: 1.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Australasian, ProviderLogGroupId = 3, ProviderGroupName = "Australasian Conference", GroupName = "Australasian Group", GroupShortName = "Aus/NZ Group" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1SouthAfrican, ProviderLogGroupId = 4, ProviderGroupName = "South African Conference", GroupName = "South African Group", GroupShortName = "Africa Group" },
                    // LogGroups for "GroupStandings", GroupHierarchyLevel: 2.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2Australian, ProviderLogGroupId = 1, ProviderGroupName = "Australian Conference", GroupName = "Australian Conference", GroupShortName = "Aus Conference" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2Zealand, ProviderLogGroupId = 2, ProviderGroupName = "New Zealand Conference", GroupName = "New Zealand Conference", GroupShortName = "NZ Conference" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2Africa1, ProviderLogGroupId = 3, ProviderGroupName = "Africa 1 Conference", GroupName = "Africa Conference 1", GroupShortName = "Africa Con 1" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2Africa2, ProviderLogGroupId = 4, ProviderGroupName = "Africa 2 Conference", GroupName = "Africa Conference 2", GroupShortName = "Africa Con 2" }
                );

                context.SaveChanges();

                // Assign log groups parent-child relationships (hierarchy).
                // Relationships for "SecondaryGroupStandings", GroupHierarchyLevel: 1.
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Australasian).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Overall);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1SouthAfrican).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Overall);
                // Relationships for "GroupStandings", GroupHierarchyLevel: 2.
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Australian).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Australasian);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Zealand).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Australasian);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Africa1).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1SouthAfrican);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Africa2).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1SouthAfrican);

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
