using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2019;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2019
{
    public static class Sevens2019Round04Hamilton
    {
        private const string SlugHierarchyLevel2HamiltonSevens2019      = "Sevens-2019-HL2-Round_4-Hamilton";

        private const string SlugHierarchyLevel3HamiltonNonCoreGroup    = "Sevens-2019-HL3-Hamilton-NonCoreGroup";
        private const string SlugHierarchyLevel3HamiltonSevens2019PoolA = "Sevens-2019-HL3-Hamilton-PoolA";
        private const string SlugHierarchyLevel3HamiltonSevens2019PoolB = "Sevens-2019-HL3-Hamilton-PoolB";
        private const string SlugHierarchyLevel3HamiltonSevens2019PoolC = "Sevens-2019-HL3-Hamilton-PoolC";
        private const string SlugHierarchyLevel3HamiltonSevens2019PoolD = "Sevens-2019-HL3-Hamilton-PoolD";

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
                    // LogGroups for "SecondaryStandings" GroupHierarchyLevel: 2 We do not ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2HamiltonSevens2019, ProviderLogGroupId = null, ProviderGroupName = "Hamilton 2019 Standings", GroupName = "Hamilton 2019 Standings", GroupShortName = "Hamilton 2019 Standings" },

                    // LogGroups for "GroupStandings" GroupHierarchyLevel: 3 We do ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, Slug = SlugHierarchyLevel3HamiltonNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2019PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Hamilton 2019 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2019PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Hamilton 2019 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2019PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Hamilton 2019 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2019PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Hamilton 2019 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2019).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == Sevens2019Round01Dubai.SlugHierarchyLevel1Sevens2019Rounds);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonNonCoreGroup).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2019PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2019PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2019PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2019PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2019);

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
