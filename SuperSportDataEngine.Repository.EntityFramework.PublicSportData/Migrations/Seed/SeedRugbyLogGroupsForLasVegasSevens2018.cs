using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyLogGroupsForLasVegasSevens2018
    {
        private const string SlugHierachyLevel1LasVegasSevens2018 = "Sevens-2018-HL1-LasVegasSevens";
        private const string SlugHierarchyLevel2LasVegasNonCoreGroup = "Sevens-2018-HL2-LasVegasSevensNonCoreGroup";
        private const string SlugHierachyLevel2LasVegasSevens2018PoolA = "Sevens-2018-HL2-LasVegasSevensPoolA";
        private const string SlugHierachyLevel2LasVegasSevens2018PoolB = "Sevens-2018-HL2-LasVegasSevensPoolB";
        private const string SlugHierachyLevel2LasVegasSevens2018PoolC = "Sevens-2018-HL2-LasVegasSevensPoolC";
        private const string SlugHierachyLevel2LasVegasSevens2018PoolD = "Sevens-2018-HL2-LasVegasSevensPoolD";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                      x.DataProvider == DataProvider.StatsProzone &&
                      x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdSevensRugby);

                var rugbySeason = context.RugbySeasons.Single(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "SecondaryStandings" GroupHierachyLevel: 1
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel1LasVegasSevens2018, ProviderLogGroupId = 0, ProviderGroupName = "Las Vegas 2018", GroupName = "Las Vegas 2018", GroupShortName = "Las Vegas 2018" },

                    // LogGroups for "groupStandings" GroupHierachyLevel: 2
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2LasVegasNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = false, Slug = SlugHierachyLevel2LasVegasSevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Las Vegas 2018 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = false, Slug = SlugHierachyLevel2LasVegasSevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Las Vegas 2018 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = false, Slug = SlugHierachyLevel2LasVegasSevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Las Vegas 2018 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = false, Slug = SlugHierachyLevel2LasVegasSevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Las Vegas 2018 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1LasVegasSevens2018).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SeedRugbyLogGroupsForSydneySevens2018.SlugHierachyLevel0Sevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2LasVegasNonCoreGroup).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1LasVegasSevens2018);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2LasVegasSevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1LasVegasSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2LasVegasSevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1LasVegasSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2LasVegasSevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1LasVegasSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2LasVegasSevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1LasVegasSevens2018);

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
