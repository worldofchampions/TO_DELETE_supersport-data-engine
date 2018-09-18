using System;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Tennis
{
    static class SeedTennisGenderForLeagues
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                // Seed the WTA League as Female
                var wtaLeague = context.TennisLeagues.Single(l => l.Slug == "wta");
                wtaLeague.Gender = TennisGender.Female;

                // Seed the ATP League as Male
                var atpLeague = context.TennisLeagues.Single(l => l.Slug == "atp");
                atpLeague.Gender = TennisGender.Male;

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
