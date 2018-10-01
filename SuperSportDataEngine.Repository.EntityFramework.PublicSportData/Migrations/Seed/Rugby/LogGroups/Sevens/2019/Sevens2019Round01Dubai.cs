using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2019
{
    public static class Sevens2019Round01Dubai
    {
        public const string SlugHierarchyLevel0Sevens2019 = "Sevens-2019-HL0-Sevens_2018/19";

        public const string SlugHierarchyLevel1Sevens2019OverallStandings = "Sevens-2019-HL1-Overall_Standings";
        public const string SlugHierarchyLevel1Sevens2019Rounds = "Sevens-2019-HL1-Rounds";

        private const string SlugHierarchyLevel2Sevens2019DubaiStandings = "Sevens-2019-HL2-Round_3-Dubai";
    
        private const string SlugHierarchyLevel3DubaiNonCoreGroup = "Sevens-2019-HL3-Dubai-NonCoreGroup";
        private const string SlugHierarchyLevel3DubaiSevens2019PoolA = "Sevens-2019-HL3-Dubai-PoolA";
        private const string SlugHierarchyLevel3DubaiSevens2019PoolB = "Sevens-2019-HL3-Dubai-PoolB";
        private const string SlugHierarchyLevel3DubaiSevens2019PoolC = "Sevens-2019-HL3-Dubai-PoolC";
        private const string SlugHierarchyLevel3DubaiSevens2019PoolD = "Sevens-2019-HL3-Dubai-PoolD";

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
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2019);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // HL0 Sevens 2017/2019 - No Data
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0Sevens2019, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Sevens 2019", GroupShortName = "Sevens 2019" },

                    // HL1 Overall Standings
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel1Sevens2019OverallStandings, ProviderLogGroupId = 0, ProviderGroupName = "2019 Sevens Series", GroupName = "Sevens 2019", GroupShortName = "Sevens 2019" },
                    // HL1 Round Standings - No Data
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel1Sevens2019Rounds, ProviderLogGroupId = null, ProviderGroupName = "Sevens 2019 Round Standings", GroupName = "Sevens 2019 Round Standings", GroupShortName = "Sevens 2019 Round Standings" },

                    // LogGroups for "SecondaryStandings" GroupHierarchyLevel: 2 We do not ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2Sevens2019DubaiStandings, ProviderLogGroupId = null, ProviderGroupName = "Dubai 2019 Standings", GroupName = "Dubai 2019 Standings", GroupShortName = "Dubai 2019 Standings" },

                    // LogGroups for "GroupStandings" GroupHierarchyLevel: 3 We do ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, Slug = SlugHierarchyLevel3DubaiNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3DubaiSevens2019PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Dubai 2019 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3DubaiSevens2019PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Dubai 2019 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3DubaiSevens2019PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Dubai 2019 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3DubaiSevens2019PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Dubai 2019 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );


                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Sevens2019OverallStandings).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Sevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Sevens2019Rounds).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Sevens2019);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Sevens2019DubaiStandings).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Sevens2019Rounds);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3DubaiNonCoreGroup).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Sevens2019DubaiStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3DubaiSevens2019PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Sevens2019DubaiStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3DubaiSevens2019PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Sevens2019DubaiStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3DubaiSevens2019PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Sevens2019DubaiStandings);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3DubaiSevens2019PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2Sevens2019DubaiStandings);

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
