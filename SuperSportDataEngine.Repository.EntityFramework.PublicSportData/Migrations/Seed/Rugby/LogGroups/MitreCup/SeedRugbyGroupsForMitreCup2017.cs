using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.MitreCup
{
    public static class SeedRugbyGroupsForMitreCup2017
    {
        private const string SlugHierarchyLevel0MitreCup = "MitreCup-2017-HL0-MitreCup";
        private const string SlugHierarchyLevel1Premiership = "MitreCup-2017-HL1-Premiership";
        private const string SlugHierarchyLevel1Championship = "MitreCup-2017-HL1-Championship";

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournament = context.RugbyTournaments.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdMitreCup);

                var rugbySeason = context.RugbySeasons.FirstOrDefault(x =>
                    x.DataProvider == DataProvider.StatsProzone &&
                    x.RugbyTournament.Id == rugbyTournament.Id &&
                    x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2017);

                if (rugbySeason == null)
                    return;

                // Create log groups.
                context.RugbyLogGroups.AddOrUpdate(
                    x => x.Slug,
                    // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHierarchyLevel0MitreCup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Mitre Cup", GroupShortName = "Mitre Cup" },
                    // LogGroups for "GroupStandings", GroupHierarchyLevel: 1.
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel1Premiership, ProviderLogGroupId = 1, ProviderGroupName = "Mitre 10 Cup Premiership", GroupName = "Mitre 10 Cup Premiership", GroupShortName = "Mitre 10 Cup Premiership" },
                    new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHierarchyLevel1Championship, ProviderLogGroupId = 2, ProviderGroupName = "Mitre 10 Cup Championship", GroupName = "Mitre 10 Cup Championship", GroupShortName = "Mitre 10 Cup Championship" }
                );

                context.SaveChanges();

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Premiership).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0MitreCup);

                context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel1Championship).ParentRugbyLogGroup =
                    context.RugbyLogGroups.Single(x => x.Slug == SlugHierarchyLevel0MitreCup);

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
