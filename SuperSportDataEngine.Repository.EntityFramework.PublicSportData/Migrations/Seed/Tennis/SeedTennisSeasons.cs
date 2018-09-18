using System;
using System.Collections.Generic;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Tennis
{
    public static class SeedTennisSeasons
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var pastYears = new List<int>(){ 2017, 2016, 2015 };
                var leagues = context.TennisLeagues.ToList();

                foreach (var league in leagues)
                {
                    foreach (var pastYear in pastYears)
                    {
                        var dbSeason = context.TennisSeasons.FirstOrDefault(s => s.ProviderSeasonId == pastYear);
                        if (dbSeason == null)
                        {
                            context.TennisSeasons.Add(new TennisSeason()
                            {
                                ProviderSeasonId = pastYear,
                                Name = pastYear + " Seeded Season",
                                DataProvider = DataProvider.CmsDataCapture,
                                StartDateUtc = DateTimeOffset.MinValue,
                                EndDateUtc = DateTimeOffset.MinValue,
                                IsActive = false,
                                IsCurrent = false,
                                TennisLeague = league
                            });
                        }
                    }
                }

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
