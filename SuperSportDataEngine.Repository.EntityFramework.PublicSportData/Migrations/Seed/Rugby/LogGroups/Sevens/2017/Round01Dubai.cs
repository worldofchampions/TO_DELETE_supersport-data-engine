using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2017
{
    public static class Round01Dubai
    {
        private const string SlugHierarchyLevel0 = "Sevens-2017-HL0";
        private const string SlugHierarchyLevel1Dubai = "Sevens-2017-HL1-Dubai";
        private const string SlugHierarchyLevel2DubaiNonCoreGroup = "Sevens-2017-HL2-Dubai-NonCoreGroup";
        private const string SlugHierarchyLevel2DubaiPoolA = "Sevens-2017-HL2-Dubai-PoolA";
        private const string SlugHierarchyLevel2DubaiPoolB = "Sevens-2017-HL2-Dubai-PoolB";
        private const string SlugHierarchyLevel2DubaiPoolC = "Sevens-2017-HL2-Dubai-PoolC";
        private const string SlugHierarchyLevel2DubaiPoolD = "Sevens-2017-HL2-Dubai-PoolD";

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
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2017);

                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, Slug = SlugHierarchyLevel0, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "", GroupShortName = "" },

                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, Slug = SlugHierarchyLevel1Dubai, ProviderLogGroupId = 1, ProviderGroupName = null, GroupName = "", GroupShortName = "" },

                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2DubaiNonCoreGroup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Non-Core Group", GroupShortName = "Non-Core Group", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2DubaiPoolA, ProviderLogGroupId = 1, ProviderGroupName = "Pool A", GroupName = "Pool A", GroupShortName = "Pool A", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2DubaiPoolB, ProviderLogGroupId = 2, ProviderGroupName = "Pool B", GroupName = "Pool B", GroupShortName = "Pool B", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2DubaiPoolC, ProviderLogGroupId = 3, ProviderGroupName = "Pool C", GroupName = "Pool C", GroupShortName = "Pool C", IsCoreGroup = false },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 2, IsConference = false, Slug = SlugHierarchyLevel2DubaiPoolD, ProviderLogGroupId = 4, ProviderGroupName = "Pool D", GroupName = "Pool D", GroupShortName = "Pool D", IsCoreGroup = false }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Dubai).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2DubaiNonCoreGroup).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Dubai);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2DubaiPoolA).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Dubai);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2DubaiPoolB).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Dubai);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2DubaiPoolC).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Dubai);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel2DubaiPoolD).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Dubai);

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
