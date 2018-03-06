namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Motorsport
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

    public static class SeedMotorsportEnabledLeagues
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

                    leagueInRepo.IsEnabled = league.IsEnabled;

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
                new MotorsportLeague {ProviderLeagueId = 1, IsEnabled = true},
            };
        }
    }
}
