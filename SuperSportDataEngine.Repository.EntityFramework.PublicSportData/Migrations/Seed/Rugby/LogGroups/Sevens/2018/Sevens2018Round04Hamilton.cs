using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2018
{
    public static class Sevens2018Round04Hamilton
    {
        private const string SlugHierarchyLevel2HamiltonSevens2018      = "Sevens-2018-HL2-Round_4-Hamilton";

        private const string SlugHierarchyLevel3HamiltonNonCoreGroup    = "Sevens-2018-HL3-Hamilton-NonCoreGroup";
        private const string SlugHierarchyLevel3HamiltonSevens2018PoolA = "Sevens-2018-HL3-Hamilton-PoolA";
        private const string SlugHierarchyLevel3HamiltonSevens2018PoolB = "Sevens-2018-HL3-Hamilton-PoolB";
        private const string SlugHierarchyLevel3HamiltonSevens2018PoolC = "Sevens-2018-HL3-Hamilton-PoolC";
        private const string SlugHierarchyLevel3HamiltonSevens2018PoolD = "Sevens-2018-HL3-Hamilton-PoolD";

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
                    // LogGroups for "SecondaryStandings" GroupHierarchyLevel: 2 We do not ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2HamiltonSevens2018, ProviderLogGroupId = null, ProviderGroupName = "Hamilton 2018 Standings", GroupName = "Hamilton 2018 Standings", GroupShortName = "Hamilton 2018 Standings" },

                    // LogGroups for "GroupStandings" GroupHierarchyLevel: 3 We do ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, Slug = SlugHierarchyLevel3HamiltonNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Hamilton 2018 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Hamilton 2018 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Hamilton 2018 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3HamiltonSevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Hamilton 2018 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2018).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == Sevens2018Round03Sydney.SlugHierarchyLevel1Sevens2018Rounds);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonNonCoreGroup).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2018);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3HamiltonSevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2HamiltonSevens2018);

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
