using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyLogGroupsForPro142018
    {
        private const string SlugHierarchyLevel0Pro14 = "Pro14-2018-HL0-Pro14";
        private const string SlugHierarchyLevel1ConferenceA = "Pro14-2018-HL0-ConferenceA";
        private const string SlugHierarchyLevel1ConferenceB = "Pro14-2018-HL0-ConferenceB";

        public static void Seed(PublicSportDataContext context)
        {
            var rugbyTournament = context.RugbyTournaments.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdPro14);

            var rugbySeason = context.RugbySeasons.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.RugbyTournament.Id == rugbyTournament.Id &&
                x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

            // Create log groups.
            context.RugbyLogGroups.AddOrUpdate(
                x => x.Slug,
                // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0Pro14, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Pro 14", GroupShortName = "Pro 14" },
                // LogGroups for "GroupStandings", GroupHierarchyLevel: 1.
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel1ConferenceA, ProviderLogGroupId = 1, ProviderGroupName = "Pro 14 Conference A 2018", GroupName = "Conference A", GroupShortName = "Conference A" },
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel1ConferenceB, ProviderLogGroupId = 2, ProviderGroupName = "Pro 14 Conference B 2018", GroupName = "Conference B", GroupShortName = "Conference B" }
            );

            context.SaveChanges();

            // Parenting.
            context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1ConferenceA).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Pro14);
            context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1ConferenceB).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0Pro14);

            context.SaveChanges();
        }
    }
}
