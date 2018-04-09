using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2018
{
    public static class Round10Paris
    {
        private const string SlugHierachyLevel1ParisSevens2018 = "Sevens-2018-HL1-ParisSevens";
        private const string SlugHierarchyLevel2ParisNonCoreGroup = "Sevens-2018-HL2-ParisSevensNonCoreGroup";
        private const string SlugHierachyLevel2ParisSevens2018PoolA = "Sevens-2018-HL2-ParisSevensPoolA";
        private const string SlugHierachyLevel2ParisSevens2018PoolB = "Sevens-2018-HL2-ParisSevensPoolB";
        private const string SlugHierachyLevel2ParisSevens2018PoolC = "Sevens-2018-HL2-ParisSevensPoolC";
        private const string SlugHierachyLevel2ParisSevens2018PoolD = "Sevens-2018-HL2-ParisSevensPoolD";

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
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel1ParisSevens2018, ProviderLogGroupId = 0, ProviderGroupName = "Paris 2018", GroupName = "Paris 2018", GroupShortName = "Paris 2018" },

                    // LogGroups for "groupStandings" GroupHierachyLevel: 2
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2ParisNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2ParisSevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Paris 2018 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2ParisSevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Paris 2018 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2ParisSevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Paris 2018 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2ParisSevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Paris 2018 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1ParisSevens2018).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == Round03Sydney.SlugHierachyLevel0Sevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2ParisNonCoreGroup).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1ParisSevens2018);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2ParisSevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1ParisSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2ParisSevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1ParisSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2ParisSevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1ParisSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2ParisSevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1ParisSevens2018);

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
