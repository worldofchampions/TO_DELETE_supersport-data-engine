using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Constants.Providers;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedGroupNamesForHamiltonSevens
    {
        public static void Seed(PublicSportDataContext context)
        {
            var rugbyTournament = context.RugbyTournaments.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.ProviderTournamentId == RugbyStatsProzoneConstants.ProviderTournamentIdSevensRugby);

            var rugbySeason = context.RugbySeasons.Single(x =>
                x.DataProvider == DataProvider.StatsProzone &&
                x.RugbyTournament.Id == rugbyTournament.Id &&
                x.ProviderSeasonId == RugbyStatsProzoneConstants.ProviderTournamentSeasonId2018);

            if (rugbySeason.CurrentRoundNumber != 4)
                return;

            var groups =
                context.RugbyLogGroups.Where(g => 
                    g.RugbySeason.ProviderSeasonId == rugbySeason.ProviderSeasonId);

            foreach (var group in groups)
            {
                group.GroupName = group.GroupName.Replace("Hamilton 2018 Group", "Group");
                group.GroupShortName = group.GroupShortName.Replace("Hamilton 2018 Group", "Group");
            }

            context.SaveChanges();
        }
    }
}
