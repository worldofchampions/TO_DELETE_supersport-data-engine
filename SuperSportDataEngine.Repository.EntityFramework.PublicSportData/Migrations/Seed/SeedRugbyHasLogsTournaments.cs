using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyHasLogsTournaments
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                const int internationalsProviderTournamentId = 810;

                var nonInternationalsDbTournaments =
                    context.RugbyTournaments.Where(t => t.ProviderTournamentId != internationalsProviderTournamentId).ToList();

                foreach (var tournament in nonInternationalsDbTournaments)
                {
                    tournament.HasLogs = true;
                    context.RugbyTournaments.AddOrUpdate(tournament);
                }

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
