using System;
using System.Linq;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Tennis
{
    public static class SeedEnableTennisSeasonsFor2018
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var seasons =
                    context.TennisSeasons.ToList();

                foreach (var tennisSeason in seasons)
                {
                    tennisSeason.IsCurrent = tennisSeason.ProviderSeasonId == 2018;
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
