using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyLiveScoredTournaments
    {
        public static void Seed(PublicSportDataContext context)
        {
            var notLiveScoredTournaments = GetNotLiveScoredTournaments();

            foreach (var tournament in notLiveScoredTournaments)
            {
                var dbTournament =
                    context.RugbyTournaments.FirstOrDefault(t =>
                        t.ProviderTournamentId == tournament.ProviderTournamentId);

                if (dbTournament is null) continue;

                dbTournament.IsLiveScored = true;

                context.RugbyTournaments.AddOrUpdate(dbTournament);
            }

            context.SaveChanges();
        }

        private static IEnumerable<RugbyTournament> GetNotLiveScoredTournaments()
        {
            return new List<RugbyTournament>
                {
                    new RugbyTournament {ProviderTournamentId = 121},//currie-cup
                    new RugbyTournament {ProviderTournamentId = 181},//super-rugby 
                    new RugbyTournament {ProviderTournamentId = 191},//the-rugby-championship
                    new RugbyTournament {ProviderTournamentId = 241},//TOP 14
                    new RugbyTournament {ProviderTournamentId = 301},//six-nations
                    new RugbyTournament {ProviderTournamentId = 810},//international
                    new RugbyTournament {ProviderTournamentId = 831},//sevens 
                    new RugbyTournament {ProviderTournamentId = 850},//rugby-union-world-cup 
                };
        }
    }
}
