namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using SuperSportDataEngine.ApplicationLogic.Constants;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class SeedRugbyHasLogsTournaments
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournaments = context.RugbyTournaments.ToList();

                var providerIdsForTournamentsWithNoLogs = GetProviderIdsForTournamentsWithNoLogs();

                foreach (var tournament in rugbyTournaments)
                {
                    if (providerIdsForTournamentsWithNoLogs.Any(t => t == tournament.ProviderTournamentId))
                    {
                        tournament.HasLogs = false;
                    }
                    else
                    {
                        tournament.HasLogs = true;
                    }

                    context.RugbyTournaments.AddOrUpdate(tournament);
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }

        private static List<int> GetProviderIdsForTournamentsWithNoLogs()
        {
            return new List<int>()
            {
                RugbyTournamentConstants.InternationalsProviderTournamentId,
                RugbyTournamentConstants.RugbyWorldCupProviderTournamentId
            };
        }
    }
}
