using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.JuniorRugby
{
    public static class SeedRugbyLogGroupsForJuniorRugbyWorldCup2019
    {
        private const string SlugHierarchyLevel0JuniorRugby = "JuniorRugbyWorldCup-2019-HL0-JuniorRugby";
        private const string SlugHierarchyLevel1PoolA = "JuniorRugbyWorldCup-2019-HL1-PoolA";
        private const string SlugHierarchyLevel1PoolB = "JuniorRugbyWorldCup-2019-HL1-PoolB";
        private const string SlugHierarchyLevel1PoolC = "JuniorRugbyWorldCup-2019-HL1-PoolC";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                        x.DataProvider == DataProvider.StatsProzone &&
                        x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdJuniorRugbyWorldCup);

                var rugbySeason = context.RugbySeasons.FirstOrDefault(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2019);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0JuniorRugby, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Junior Rugby World Cup", GroupShortName = "Junior Rugby World Cup" },

                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Group C", GroupName = "Pool C", GroupShortName = "Pool C" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0JuniorRugby);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0JuniorRugby);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0JuniorRugby);

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
