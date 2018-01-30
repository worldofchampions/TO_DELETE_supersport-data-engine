using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyLogGroupsForHamiltonSevens2018
    {
        private const string SlugHierachyLevel1HamiltonSevens2018 = "Sevens-2018-HL1-HamiltonSevens";
        private const string SlugHierachyLevel2HamiltonSevens2018PoolA = "Sevens-2018-HL2-HamiltonSevensPoolA";
        private const string SlugHierachyLevel2HamiltonSevens2018PoolB = "Sevens-2018-HL2-HamiltonSevensPoolB";
        private const string SlugHierachyLevel2HamiltonSevens2018PoolC = "Sevens-2018-HL2-HamiltonSevensPoolC";
        private const string SlugHierachyLevel2HamiltonSevens2018PoolD = "Sevens-2018-HL2-HamiltonSevensPoolD";

        public static void Seed(PublicSportDataContext context)
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
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierachyLevel1HamiltonSevens2018, ProviderLogGroupId = null, ProviderGroupName = "Sydney 2018", GroupName = "Sydney 2018", GroupShortName = "Sydney 2018" },

                // LogGroups for "groupStandings" GroupHierachyLevel: 2
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2HamiltonSevens2018PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Sydney 2018 Group A", GroupName = "Sydney 2018 Group A", GroupShortName = "Sydney 2018 Group A" },
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2HamiltonSevens2018PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Sydney 2018 Group B", GroupName = "Sydney 2018 Group B", GroupShortName = "Sydney 2018 Group B" },
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2HamiltonSevens2018PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Sydney 2018 Group C", GroupName = "Sydney 2018 Group C", GroupShortName = "Sydney 2018 Group C" },
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, IsCoreGroup = true, Slug = SlugHierachyLevel2HamiltonSevens2018PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Sydney 2018 Group D", GroupName = "Sydney 2018 Group D", GroupShortName = "Sydney 2018 Group D" }
            );

            context.SaveChanges();

            context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1HamiltonSevens2018).ParentRugbyLogGroup = null;
            context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2HamiltonSevens2018PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1HamiltonSevens2018);
            context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2HamiltonSevens2018PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1HamiltonSevens2018);
            context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2HamiltonSevens2018PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1HamiltonSevens2018);
            context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel2HamiltonSevens2018PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierachyLevel1HamiltonSevens2018);

            context.SaveChanges();
        }
    }
}
