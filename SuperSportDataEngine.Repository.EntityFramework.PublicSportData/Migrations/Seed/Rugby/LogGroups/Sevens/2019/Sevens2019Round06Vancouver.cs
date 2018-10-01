using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2019
{
    public static class Sevens2019Round06Vancouver
    {
        private const string SlugHierarchyLevel2VancouverSevens2019      = "Sevens-2019-HL2-Round_6-Vancouver";

        private const string SlugHierarchyLevel3VancouverNonCoreGroup    = "Sevens-2019-HL3-Vancouver-NonCoreGroup";
        private const string SlugHierarchyLevel3VancouverSevens2019PoolA = "Sevens-2019-HL3-Vancouver-PoolA";
        private const string SlugHierarchyLevel3VancouverSevens2019PoolB = "Sevens-2019-HL3-Vancouver-PoolB";
        private const string SlugHierarchyLevel3VancouverSevens2019PoolC = "Sevens-2019-HL3-Vancouver-PoolC";
        private const string SlugHierarchyLevel3VancouverSevens2019PoolD = "Sevens-2019-HL3-Vancouver-PoolD";

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
                    // LogGroups for "SecondaryStandings" GroupHierarchyLevel: 1 We do not ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel2VancouverSevens2019, ProviderLogGroupId = null, ProviderGroupName = "Vancouver 2019", GroupName = "Vancouver 2019", GroupShortName = "Vancouver 2019" },

                    // LogGroups for "groupStandings" GroupHierarchyLevel: 2 We do ingest this.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, Slug = SlugHierarchyLevel3VancouverNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3VancouverSevens2019PoolA, ProviderLogGroupId = 1, ProviderGroupName = "Vancouver 2019 Group A", GroupName = "Pool A", GroupShortName = "Pool A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3VancouverSevens2019PoolB, ProviderLogGroupId = 2, ProviderGroupName = "Vancouver 2019 Group B", GroupName = "Pool B", GroupShortName = "Pool B" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3VancouverSevens2019PoolC, ProviderLogGroupId = 3, ProviderGroupName = "Vancouver 2019 Group C", GroupName = "Pool C", GroupShortName = "Pool C" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 3, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel3VancouverSevens2019PoolD, ProviderLogGroupId = 4, ProviderGroupName = "Vancouver 2019 Group D", GroupName = "Pool D", GroupShortName = "Pool D" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2VancouverSevens2019).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == Sevens2019Round01Dubai.SlugHierarchyLevel1Sevens2019Rounds);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3VancouverNonCoreGroup).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2VancouverSevens2019);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3VancouverSevens2019PoolA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2VancouverSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3VancouverSevens2019PoolB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2VancouverSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3VancouverSevens2019PoolC).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2VancouverSevens2019);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel3VancouverSevens2019PoolD).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2VancouverSevens2019);

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
