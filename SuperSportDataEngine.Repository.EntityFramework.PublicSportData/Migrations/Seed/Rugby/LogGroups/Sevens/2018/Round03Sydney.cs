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
        public const string SlugHierachyLevel0Sevens2018                  = "Sevens-2018-HL0-Sevens_2018/19";

        public const string SlugHierachyLevel1Sevens2018OverallStandings = "Sevens-2018-HL1-Overall_Standings";
        public const string SlugHierachyLevel1Sevens2018Rounds           = "Sevens-2018-HL1-Rounds";

        private const string SlugHierachyLevel2Sevens2018SydneyStandings  = "Sevens-2018-HL2-Round_3-Sydney";

        private const string SlugHierarchyLeve3SydneyNonCoreGroup         = "Sevens-2018-HL3-Sydney_NonCoreGroup";
        private const string SlugHierachyLevel3SydneySevens2018PoolA      = "Sevens-2018-HL3-Sydney-PoolA";
        private const string SlugHierachyLevel3SydneySevens2018PoolB      = "Sevens-2018-HL3-Sydney-PoolB";
        private const string SlugHierachyLevel3SydneySevens2018PoolC      = "Sevens-2018-HL3-Sydney-PoolC";
        private const string SlugHierachyLevel3SydneySevens2018PoolD      = "Sevens-2018-HL3-Sydney-PoolD";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                        x.DataProvider == DataProvider.StatsProzone &&
                        x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdSevensRugby);

                var rugbySeason = context.RugbySeasons.FirstOrDefault(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // HL0 Sevens 2017/2018 - No Data
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel0Sevens2018, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Sevens 2018", GroupShortName = "Sevens 2018" },

                    // HL1 Overall Standings
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel1Sevens2018OverallStandings, ProviderLogGroupId = 0, ProviderGroupName = "2018 Sevens Series", GroupName = "Sevens 2018", GroupShortName = "Sevens 2018" },
                    // HL1 Round Standings - No Data
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel1Sevens2018Rounds, ProviderLogGroupId = null, ProviderGroupName = "Sevens 2018 Round Standings", GroupName = "Sevens 2018 Round Standings", GroupShortName = "Sevens 2018 Round Standings" },

                    // LogGroups for "SecondaryStandings" GroupHierachyLevel: 2 We do not ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel2Sevens2018SydneyStandings, ProviderLogGroupId = null, ProviderGroupName = "Sydney 2018 Standings", GroupName = "Sydney 2018 Standings", GroupShortName = "Sydney 2018 Standings" },

                    // LogGroups for "GroupStandings" GroupHierachyLevel: 3 We do ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, Slug = SlugHierarchyLeve3SydneyNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SydneySevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Sydney 2018 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SydneySevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Sydney 2018 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SydneySevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Sydney 2018 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SydneySevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Sydney 2018 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1Sevens2018OverallStandings).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0Sevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1Sevens2018Rounds).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel0Sevens2018);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2Sevens2018SydneyStandings).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1Sevens2018Rounds);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLeve3SydneyNonCoreGroup).ParentRugbyLogGroup    = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2Sevens2018SydneyStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SydneySevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2Sevens2018SydneyStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SydneySevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2Sevens2018SydneyStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SydneySevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2Sevens2018SydneyStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SydneySevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2Sevens2018SydneyStandings);

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
