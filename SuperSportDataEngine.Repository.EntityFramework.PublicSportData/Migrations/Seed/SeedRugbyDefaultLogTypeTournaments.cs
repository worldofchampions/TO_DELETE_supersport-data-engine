using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyDefaultLogTypeTournaments
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var tournamentsSlugs = GetGroupedLogTournamentSlugNames();
                foreach (var tournamentsSlug in tournamentsSlugs)
                {
                    var tournament = context.RugbyTournaments.First(t => t.Slug == tournamentsSlug);
                    tournament.DefaultRugbyLogType = RugbyLogType.GroupedLogs;
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }

        private static List<string> GetGroupedLogTournamentSlugNames()
        {
            var tournaments = new List<string>
            {
                "super-rugby",
                "supersport-challenge",
                "junior-rugby",
                "new-zealand",
                "pro14",
                "champions-cup",
                "sevens"
            };

            return tournaments;
        }
    }
}
