namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.RugbyWorldCup
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class SeedRugbyLogGroupsForRugbyWorldCup2015
    {
        private const string SlugHierachyLevel0RugbyWorldCup = "RugbyWorldCup-2015-HL0-RugbyWorldCup";
        private const string SlugHierachyLevel1PoolA = "RugbyWorldCup-2015-HL1-PoolA";
        private const string SlugHierachyLevel1PoolB = "RugbyWorldCup-2015-HL1-PoolB";
        private const string SlugHierachyLevel1PoolC = "RugbyWorldCup-2015-HL1-PoolC";
        private const string SlugHierachyLevel1PoolD = "RugbyWorldCup-2015-HL1-PoolD";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                        x.DataProvider == DataProvider.StatsProzone &&
                        x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdRugbyWorldCup);

                var rugbySeason = context.RugbySeasons.FirstOrDefault(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2015);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel0RugbyWorldCup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Rugby World Cup", GroupShortName = "Rugby World Cup" },

                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel1PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Pool A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel1PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Pool B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel1PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Pool C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel1PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Pool D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0RugbyWorldCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0RugbyWorldCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0RugbyWorldCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0RugbyWorldCup);

                context.SaveChanges();

            }
            catch (Exception exception)
            {
                //TODO: Add logging.
                Console.WriteLine(exception);
            }
        }
    }
}
