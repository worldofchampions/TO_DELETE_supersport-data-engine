namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Motorsport
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public static class SeedMotorsportHistoricSeasons
    {
        public static void Seed(PublicSportDataContext dataContext)
        {
            try
            {
                var seededSeasons = GetSeededSeasons(dataContext);

                foreach (var season in seededSeasons)
                {
                    var seasonInRepo =
                        dataContext.MotorsportSeasons.FirstOrDefault(s =>
                            s.ProviderSeasonId == season.ProviderSeasonId && s.MotorsportLeague.ProviderLeagueId == season.MotorsportLeague.ProviderLeagueId);

                    if (seasonInRepo != null) continue;

                    dataContext.MotorsportSeasons.AddOrUpdate(season);
                }

                dataContext.SaveChanges();
            }
            catch (Exception exception)
            {
                //TODO: Add logging 
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<MotorsportSeason> GetSeededSeasons(PublicSportDataContext dataContext)
        {
            var seasons = new List<MotorsportSeason>();

            var f1League = dataContext.MotorsportLeagues.FirstOrDefault(l => l.ProviderLeagueId == 1);
            if (f1League != null)
            {
                seasons.Add(new MotorsportSeason { Name = "2016 f1 seeded season", ProviderSeasonId = 2016, MotorsportLeague = f1League });
                seasons.Add(new MotorsportSeason { Name = "2017 f1 seeded season", ProviderSeasonId = 2017, MotorsportLeague = f1League });
            }

            var motogpLeague = dataContext.MotorsportLeagues.FirstOrDefault(l => l.ProviderLeagueId == 11);
            if (motogpLeague != null)
            {
                seasons.Add(new MotorsportSeason { Name = "2016 motogp seeded season", ProviderSeasonId = 2016, MotorsportLeague = motogpLeague });
                seasons.Add(new MotorsportSeason { Name = "2017 mtotgp seeded season", ProviderSeasonId = 2017, MotorsportLeague = motogpLeague });
            }

            var superBikeLeague = dataContext.MotorsportLeagues.FirstOrDefault(l => l.ProviderLeagueId == 8);
            if (superBikeLeague != null)
            {
                seasons.Add(new MotorsportSeason { Name = "2016 superbike seeded season", ProviderSeasonId = 2016, MotorsportLeague = superBikeLeague });
                seasons.Add(new MotorsportSeason { Name = "2017 superbike seeded season", ProviderSeasonId = 2017, MotorsportLeague = superBikeLeague });
            }

            return seasons;
        }
    }
}
