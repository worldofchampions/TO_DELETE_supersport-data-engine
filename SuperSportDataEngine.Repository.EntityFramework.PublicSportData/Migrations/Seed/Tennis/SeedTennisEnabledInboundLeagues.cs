using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Tennis
{
    public static class SeedTennisEnabledInboundLeagues
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var leagues = context.TennisLeagues.ToList();

                foreach (var league in leagues)
                {
                    if(ShouldEnableLeagueInbound(league.Slug))
                    {
                        league.IsDisabledInbound = false;
                    }
                }

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static bool ShouldEnableLeagueInbound(string leagueSlug)
        {
            var enabledLeagueSlugs = new List<string>() { "wta", "atp" };
            return enabledLeagueSlugs.Contains(leagueSlug.ToLower());
        }
    }
}
