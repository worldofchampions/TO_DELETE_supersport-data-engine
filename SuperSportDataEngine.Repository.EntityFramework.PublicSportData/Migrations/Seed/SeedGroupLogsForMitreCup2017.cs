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
    public static class SeedGroupLogsForMitreCup2017
    {
        private const string SlugHeirachyLevel0MitreCup = "MitreCup-2017-HL0-MitreCup";
        private const string SlugHeirachyLevel1Premiership = "MitreCup-2017-HL0-Premiership";
        private const string SlugHeirachyLevel1Championship = "MitreCup-2017-HL0-Championship";

        public static void Seed(PublicSportDataContext context)
        {
            var rugbyTournament = context.RugbyTournaments.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdMitreCup);

            var rugbySeason = context.RugbySeasons.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.RugbyTournament.Id == rugbyTournament.Id &&
                x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

            // Create log groups.
            context.RugbyLogGroups.AddOrUpdate(
                x => x.Slug,
                // LogGroups for "OverallStandings", GroupHierarchyLevel: 0.
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 0, IsConference = false, IsCoreGroup = true, Slug = SlugHeirachyLevel0MitreCup, ProviderLogGroupId = 0, ProviderGroupName = null, GroupName = "Pro 14", GroupShortName = "Pro 14" },
                // LogGroups for "GroupStandings", GroupHierarchyLevel: 1.
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHeirachyLevel1Premiership, ProviderLogGroupId = 1, ProviderGroupName = "Mitre 10 Cup Premiership", GroupName = "Mitre 10 Cup Premiership", GroupShortName = "Mitre 10 Cup Premiership" },
                new RugbyLogGroup { DataProvider = DataProvider.StatsProzone, RugbySeason = rugbySeason, GroupHierarchyLevel = 1, IsConference = true, IsCoreGroup = true, Slug = SlugHeirachyLevel1Championship, ProviderLogGroupId = 2, ProviderGroupName = "Mitre 10 Cup Championship", GroupName = "Mitre 10 Cup Championship", GroupShortName = "Mitre 10 Cup Championship" }
            );
        }
    }
}
