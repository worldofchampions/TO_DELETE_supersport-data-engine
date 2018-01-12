using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyHasLogsTournaments
    {
        public static void Seed(PublicSportDataContext context)
        {
            const int internationalsProviderTournamentId = 810;

            var nonInternationalsBbTournaments =
                context.RugbyTournaments.Where(t => t.ProviderTournamentId != internationalsProviderTournamentId);

            foreach (var tournament in nonInternationalsBbTournaments)
            {
                tournament.HasLogs = true;
                context.RugbyTournaments.AddOrUpdate(tournament);
            }

            context.SaveChanges();
        }
    }
}
