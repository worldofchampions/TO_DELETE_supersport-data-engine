namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Motorsport
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

    public class SeedMotorsportCurrentSeasons
    {
        public static void Seed(PublicSportDataContext dataContext)
        {
            try
            {
                var seededSeasons = GetSeededSeasons();
                const int f1ProviderLeagueId = 1;
                foreach (var season in seededSeasons)
                {
                    var seasonInRepo =
                        dataContext.MotorsportSeasons.FirstOrDefault(s =>
                            s.ProviderSeasonId == season.ProviderSeasonId && s.MotorsportLeague.ProviderLeagueId == f1ProviderLeagueId);

                    if (seasonInRepo == null) continue;

                    seasonInRepo.IsCurrent = true;

                    dataContext.MotorsportSeasons.AddOrUpdate(seasonInRepo);
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
                new MotorsportSeason {ProviderSeasonId = 2018, IsCurrent = true}
            };
        }
    }
}
