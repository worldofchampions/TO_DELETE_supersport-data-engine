namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Constants;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SeedRugbyTournamentDefaultLogType
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var tournamentsSlugs = GetGroupedLogTournamentSlugNames();
                foreach (var slug in tournamentsSlugs)
                {
                    try
                    {
                        var tournament = context.RugbyTournaments.First(t => t.Slug == slug);
                        tournament.DefaultRugbyLogType = RugbyLogType.GroupedLogs;
                    }
                    catch (Exception exception)
                    {
                        // Sometimes the exception is only for one tournament. This must not affect seeding for others.
                        Console.WriteLine("No tournament found for slug: " + slug);
                        Console.WriteLine(exception);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<string> GetGroupedLogTournamentSlugNames()
        {
            var tournaments = new List<string>
            {
                RugbyTournamentConstants.SuperRugbySlugName,
                RugbyTournamentConstants.SupersportChallengeSlugName,
                RugbyTournamentConstants.JuniorRugbySlugName,
                RugbyTournamentConstants.NewZealandSlugName,
                RugbyTournamentConstants.Pro14,
                RugbyTournamentConstants.ChampionsCupSlugName,
                RugbyTournamentConstants.SevensSlugName,
                RugbyTournamentConstants.RugbyWorldCupSlugName
            };

            return tournaments;
        }
    }
}
