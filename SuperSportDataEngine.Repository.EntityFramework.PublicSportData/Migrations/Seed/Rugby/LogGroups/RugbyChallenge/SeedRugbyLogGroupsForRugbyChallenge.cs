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
    static class SeedRugbyLogGroupsForRugbyChallenge
    {
        private const string SlugHierarchyLevel0RugbyChallenge = "RugbyChallenge-2018-HL0-RugbyChallenge";
        private const string SlugHierarchyLevel1Central = "RugbyChallenge-2018-HL1-Central";
        private const string SlugHierarchyLevel1North = "RugbyChallenge-2018-HL1-North";
        private const string SlugHierarchyLevel1South = "RugbyChallenge-2018-HL1-South";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                        x.DataProvider == DataProvider.StatsProzone &&
                        x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdRugbyChallenge);

                var rugbySeason = context.RugbySeasons.FirstOrDefault(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0RugbyChallenge, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "SuperSport Challenge", GroupShortName = "SuperSport Challenge" },
                    // LogGroups for "GroupStandings", GroupHierarchyLevel: 1.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1North, ProviderLogGroupId = 1, ProviderGroupName = "Northern Section", GroupName = "North", GroupShortName = "North" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1Central, ProviderLogGroupId = 2, ProviderGroupName = "Central Section", GroupName = "Central", GroupShortName = "Central" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel1South, ProviderLogGroupId = 3, ProviderGroupName = "Southern Section", GroupName = "South", GroupShortName = "South" }
                );

                context.SaveChanges();

                // Parenting.
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Central).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0RugbyChallenge);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1North).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0RugbyChallenge);
                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1South).ParentRugbyLogGroup = context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0RugbyChallenge);

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
