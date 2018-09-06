using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Pro14
{
    public static class SeedRugbyLogGroupsForPro142019
    {
        private const string SlugHierarchyLevel0Pro14 = "Pro14-2019-HL0-Pro14";
        private const string SlugHierarchyLevel1ConferenceA = "Pro14-2019-HL1-ConferenceA";
        private const string SlugHierarchyLevel1ConferenceB = "Pro14-2019-HL1-ConferenceB";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                        x.DataProvider == DataProvider.StatsProzone &&
                        x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdPro14);

                var rugbySeason = context.RugbySeasons.FirstOrDefault(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2019);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0Pro14, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Pro 14", GroupShortName = "Pro 14" },
                    // LogGroups for "GroupStandings", GroupHierarchyLevel: 1.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1ConferenceA, ProviderLogGroupId = 1, ProviderGroupName = "Pro 14 Conference A 2019", GroupName = "Conference A", GroupShortName = "Conference A" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1ConferenceB, ProviderLogGroupId = 2, ProviderGroupName = "Pro 14 Conference B 2019", GroupName = "Conference B", GroupShortName = "Conference B" }
                );

                context.SaveChanges();

                // Parenting.
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1ConferenceA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Pro14);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1ConferenceB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Pro14);

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
