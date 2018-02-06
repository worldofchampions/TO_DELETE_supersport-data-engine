using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyEnabledTournaments
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var enabledTournaments = GetSeedingEnabledTournaments();

                foreach (var tournament in enabledTournaments)
                {
                    var dbTournament = context.RugbyTournaments.FirstOrDefault(
                        t => t.ProviderTournamentId == tournament.ProviderTournamentId
                             && t.DataProvider == DataProvider.StatsProzone);

                    if (dbTournament == null) continue;

                    dbTournament.IsEnabled = true;

                    context.RugbyTournaments.AddOrUpdate(dbTournament);
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                //TODO: Add logging 
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<RugbyTournament> GetSeedingEnabledTournaments()
        {
            return new List<RugbyTournament>
            {
                new RugbyTournament {ProviderTournamentId = 291},//Champions Cup
                new RugbyTournament {ProviderTournamentId = 121},//Currie Cup
                new RugbyTournament {ProviderTournamentId = 810},//Internationals
                new RugbyTournament {ProviderTournamentId = 293},//Pro 14 
                new RugbyTournament {ProviderTournamentId = 191},//Rugby Championship
                new RugbyTournament {ProviderTournamentId = 831},//Sevens 
                new RugbyTournament {ProviderTournamentId = 181},//Super Rugby
                new RugbyTournament {ProviderTournamentId = 241} //TOP 14
            };
        }
    }
}
