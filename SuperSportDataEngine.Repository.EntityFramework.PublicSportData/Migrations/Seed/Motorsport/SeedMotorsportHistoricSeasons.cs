using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Motorsport
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public static class SeedMotorsportHistoricSeasons
    {
        public static void Seed(PublicSportDataContext dataContext)
        {
            try
            {
                var f1League = dataContext.MotorsportLeagues.FirstOrDefault(l => l.ProviderLeagueId == 1);

                if (f1League != null)
                {
                    var seededSeasons = GetSeededSeasons();

                    foreach (var season in seededSeasons)
                    {
                        var seasonInRepo =
                            dataContext.MotorsportSeasons.FirstOrDefault(s =>
                                s.ProviderSeasonId == season.ProviderSeasonId && s.MotorsportLeague.Id == f1League.Id);

                        if (seasonInRepo != null) continue;

                        season.MotorsportLeague = f1League;

                        dataContext.MotorsportSeasons.AddOrUpdate(season);
                    }
                }

                dataContext.SaveChanges();
            }
            catch (Exception exception)
            {
                //TODO: Add logging 
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<MotorsportSeason> GetSeededSeasons()
        {
            return new List<MotorsportSeason>
            {
                new MotorsportSeason {ProviderSeasonId = 2016, IsActive = false, IsCurrent = false, Name = "2016 Seeded season", DataProvider = DataProvider.Stats},
                new MotorsportSeason {ProviderSeasonId = 2017, IsActive = false, IsCurrent = false, Name = "2017 Seeded season", DataProvider = DataProvider.Stats}
            };
        }
    }
}
