using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2018
{
    public static class Round03Sydney
    {
        public const string SlugHierachyLevel0Sevens2018 = "Sevens-2018-HL0-Sevens2018";
        private const string SlugHierachyLevel1SydneySevens2018 = "Sevens-2018-HL1-SydneySevens";
        private const string SlugHierarchyLevel2SydneyNonCoreGroup = "Sevens-2018-HL2-SydneySevensNonCoreGroup";
        private const string SlugHierachyLevel2SydneySevens2018PoolA = "Sevens-2018-HL2-SydneySevensPoolA";
        private const string SlugHierachyLevel2SydneySevens2018PoolB = "Sevens-2018-HL2-SydneySevensPoolB";
        private const string SlugHierachyLevel2SydneySevens2018PoolC = "Sevens-2018-HL2-SydneySevensPoolC";
        private const string SlugHierachyLevel2SydneySevens2018PoolD = "Sevens-2018-HL2-SydneySevensPoolD";

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
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel0Sevens2018, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Sevens 2018", GroupShortName = "Sevens 2018" },

                    // LogGroups for "SecondaryStandings" GroupHierachyLevel: 1
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel1SydneySevens2018, ProviderLogGroupId = 0, ProviderGroupName = "Sydney 2018", GroupName = "Sydney 2018", GroupShortName = "Sydney 2018" },

                    // LogGroups for "groupStandings" GroupHierachyLevel: 2
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2SydneyNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2SydneySevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Sydney 2018 Group A", GroupName = "Sydney 2018 Group A", GroupShortName = "Sydney 2018 Group A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2SydneySevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Sydney 2018 Group B", GroupName = "Sydney 2018 Group B", GroupShortName = "Sydney 2018 Group B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2SydneySevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Sydney 2018 Group C", GroupName = "Sydney 2018 Group C", GroupShortName = "Sydney 2018 Group C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2SydneySevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Sydney 2018 Group D", GroupName = "Sydney 2018 Group D", GroupShortName = "Sydney 2018 Group D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1SydneySevens2018).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0Sevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2SydneyNonCoreGroup).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1SydneySevens2018);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SydneySevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1SydneySevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SydneySevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1SydneySevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SydneySevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1SydneySevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SydneySevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1SydneySevens2018);

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                //TODO: Add logging
                Console.WriteLine(exception);
            }
        }
    }
}
