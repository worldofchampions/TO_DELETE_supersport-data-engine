using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2018
{
    public static class Round08Singapore
    {
        private const string SlugHierachyLevel2SingaporeSevens2018 = "Sevens-2018-HL2-SingaporeSevens";
        private const string SlugHierarchyLeve32SingaporeNonCoreGroup = "Sevens-2018-HL3-SingaporeSevensNonCoreGroup";
        private const string SlugHierachyLevel3SingaporeSevens2018PoolA = "Sevens-2018-HL3-SingaporeSevensPoolA";
        private const string SlugHierachyLevel3SingaporeSevens2018PoolB = "Sevens-2018-HL3-SingaporeSevensPoolB";
        private const string SlugHierachyLevel3SingaporeSevens2018PoolC = "Sevens-2018-HL3-SingaporeSevensPoolC";
        private const string SlugHierachyLevel3SingaporeSevens2018PoolD = "Sevens-2018-HL3-SingaporeSevensPoolD";

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
                    // LogGroups for "SecondaryStandings" GroupHierachyLevel: 2 We do not ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel2SingaporeSevens2018, ProviderLogGroupId = null, ProviderGroupName = "Singapore 2018 Standings", GroupName = "Singapore 2018 Standings", GroupShortName = "Singapore 2018 Standings" },

                    // LogGroups for "groupStandings" GroupHierachyLevel: 3 We do ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, Slug = SlugHierarchyLeve32SingaporeNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SingaporeSevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Singapore 2018 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SingaporeSevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Singapore 2018 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SingaporeSevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Singapore 2018 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel3SingaporeSevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Singapore 2018 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SingaporeSevens2018).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == Round03Sydney.SlugHierachyLevel1Sevens2018Rounds);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLeve32SingaporeNonCoreGroup).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SingaporeSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SingaporeSevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SingaporeSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SingaporeSevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SingaporeSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SingaporeSevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SingaporeSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel3SingaporeSevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2SingaporeSevens2018);

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
