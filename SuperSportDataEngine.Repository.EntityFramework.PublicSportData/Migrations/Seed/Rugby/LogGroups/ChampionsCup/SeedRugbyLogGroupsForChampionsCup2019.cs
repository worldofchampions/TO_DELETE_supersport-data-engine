using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.ChampionsCup
{
    public static class SeedRugbyLogGroupsForChampionsCup2019
    {
        private const string SlugHierarchyLevel0ChampionsCup = "ChampionsCup-2019-HL0-ChampionsCup";
        private const string SlugHierarchyLevel1Pool1 = "ChampionsCup-2019-HL1-Pool1";
        private const string SlugHierarchyLevel1Pool2 = "ChampionsCup-2019-HL1-Pool2";
        private const string SlugHierarchyLevel1Pool3 = "ChampionsCup-2019-HL1-Pool3";
        private const string SlugHierarchyLevel1Pool4 = "ChampionsCup-2019-HL1-Pool4";
        private const string SlugHierarchyLevel1Pool5 = "ChampionsCup-2019-HL1-Pool5";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                        x.DataProvider == DataProvider.StatsProzone &&
                        x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdChampionsCup);

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
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0ChampionsCup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Champions Cup", GroupShortName = "Champions Cup" },

                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Pool1, ProviderLogGroupId = 1, ProviderGroupName = "Pool 1", GroupName = "Pool 1", GroupShortName = "Pool 1" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Pool2, ProviderLogGroupId = 2, ProviderGroupName = "Pool 2", GroupName = "Pool 2", GroupShortName = "Pool 2" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Pool3, ProviderLogGroupId = 3, ProviderGroupName = "Pool 3", GroupName = "Pool 3", GroupShortName = "Pool 3" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Pool4, ProviderLogGroupId = 4, ProviderGroupName = "Pool 4", GroupName = "Pool 4", GroupShortName = "Pool 4" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Pool5, ProviderLogGroupId = 5, ProviderGroupName = "Pool 5", GroupName = "Pool 5", GroupShortName = "Pool 5" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Pool1).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0ChampionsCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Pool2).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0ChampionsCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Pool3).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0ChampionsCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Pool4).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0ChampionsCup);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Pool5).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0ChampionsCup);

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
