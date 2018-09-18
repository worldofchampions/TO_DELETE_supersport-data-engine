using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Tennis
{
    public static class SeedTennisTournamentSlugsForGrandSlams
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var newSlugNames = GetNewTournamentSlugPairs();
                foreach (var newSlugName in newSlugNames)
                {
                    var tournament =
                        context.TennisTournaments.FirstOrDefault(t => 
                            t.ProviderTournamentId == newSlugName.providerTournamentId);

                    if (tournament == null)
                        continue;

                    tournament.Slug = newSlugName.newTournamentSlug;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static IEnumerable<(int providerTournamentId, string newTournamentSlug)> GetNewTournamentSlugPairs()
        {
            return new List<(int, string)>
            {
                ( 1, "roland-garros" ),
                ( 2, "australian-open" ),
                ( 11, "wimbledon" ),
                ( 13, "us-open" )
            };
        }
    }
}
