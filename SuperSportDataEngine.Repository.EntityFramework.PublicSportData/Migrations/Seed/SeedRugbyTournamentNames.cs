using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyTournamentNames
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournaments = GetSeedingNamesForStatsTournaments();

                foreach (var tournament in rugbyTournaments)
                {
                    var dbTournament = context.RugbyTournaments.FirstOrDefault(
                        t => t.ProviderTournamentId == tournament.ProviderTournamentId
                             && t.DataProvider == DataProvider.StatsProzone
                             && t.Name != tournament.Name);

                    if (dbTournament == null) continue;
                    if (!string.IsNullOrEmpty(dbTournament.NameCmsOverride)) continue;

                    dbTournament.NameCmsOverride = tournament.NameCmsOverride;

                    context.RugbyTournaments.AddOrUpdate(dbTournament);
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<RugbyTournament> GetSeedingNamesForStatsTournaments()
        {
            return new List<RugbyTournament>
            {
                new RugbyTournament {NameCmsOverride = "Aviva Premiership", ProviderTournamentId = 201},
                new RugbyTournament {NameCmsOverride = "Mitre 10 Cup", ProviderTournamentId = 101},
                new RugbyTournament {NameCmsOverride = "French Top 14", ProviderTournamentId = 241},
                new RugbyTournament {NameCmsOverride = "Rugby Championship", ProviderTournamentId = 191},
                new RugbyTournament {NameCmsOverride = "HSBC Sevens World Series Sydney", ProviderTournamentId = 831},
                new RugbyTournament {NameCmsOverride = "Champions Cup", ProviderTournamentId = 291},
                new RugbyTournament {NameCmsOverride = "Guinness Pro14", ProviderTournamentId = 293},
                new RugbyTournament {NameCmsOverride = "RBS Six Nations", ProviderTournamentId = 301}
            };
        }
    }
}
