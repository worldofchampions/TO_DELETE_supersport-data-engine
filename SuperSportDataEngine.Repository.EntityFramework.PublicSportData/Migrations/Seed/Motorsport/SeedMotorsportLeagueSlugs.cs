namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Motorsport
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public static class SeedMotorsportLeagueSlugs
    {
        public static void Seed(PublicSportDataContext dataContext)
        {
            try
            {
                var seededLeagues = GetSeededLeagues();

                foreach (var league in seededLeagues)
                {
                    var leagueInRepo =
                        dataContext.MotorsportLeagues.FirstOrDefault(l => l.ProviderLeagueId.Equals(league.ProviderLeagueId));

                    if (leagueInRepo == null) continue;

                    leagueInRepo.Slug = league.Slug;

                    dataContext.MotorsportLeagues.AddOrUpdate(leagueInRepo);
                }

                dataContext.SaveChanges();
            }
            catch (Exception exception)
            {
                //TODO: Add logging 
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<MotorsportLeague> GetSeededLeagues()
        {
            return new List<MotorsportLeague>
            {
                new MotorsportLeague {ProviderLeagueId = 1, Slug = "formula1"},
                new MotorsportLeague {ProviderLeagueId = 8, Slug = "super-bikes"},
                new MotorsportLeague {ProviderLeagueId = 10, Slug = "rallying"}
            };
        }
    }
}