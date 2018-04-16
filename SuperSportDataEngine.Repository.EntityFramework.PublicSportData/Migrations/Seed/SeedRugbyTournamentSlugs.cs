using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyTournamentSlugs
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var rugbyTournaments = GetSeedingSlugsForStatsTournaments();

                foreach (var tournament in rugbyTournaments)
                {
                    var dbTournament = context.RugbyTournaments.FirstOrDefault(
                        t => t.ProviderTournamentId == tournament.ProviderTournamentId
                        && t.DataProvider == DataProvider.StatsProzone
                        && t.Slug != tournament.Slug);

                    if (dbTournament == null) continue;

                    if (!string.IsNullOrEmpty(dbTournament.Slug)) continue;

                    dbTournament.Slug = tournament.Slug;

                    context.RugbyTournaments.AddOrUpdate(dbTournament);
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }

        private static IEnumerable<RugbyTournament> GetSeedingSlugsForStatsTournaments()
        {
            return new List<RugbyTournament>
            {
                new RugbyTournament {Slug = "british-lions",ProviderTournamentId = 761},
                new RugbyTournament {Slug = "champions-cup",ProviderTournamentId = 291},
                new RugbyTournament {Slug = "craven-week",ProviderTournamentId = 129},
                new RugbyTournament {Slug = "currie-cup",ProviderTournamentId = 121},
                new RugbyTournament {Slug = "international",ProviderTournamentId = 810},
                new RugbyTournament {Slug = "pro14",ProviderTournamentId = 293},
                new RugbyTournament {Slug = "pro-d2",ProviderTournamentId = 242},
                new RugbyTournament {Slug = "national-rugby-championship",ProviderTournamentId = 117}, // National Rugby Championship
                new RugbyTournament {Slug = "supersport-challenge",ProviderTournamentId = 165}, // Rugby Challenge
                new RugbyTournament {Slug = "rugby-championship",ProviderTournamentId = 191}, // The Rugby Championship
                new RugbyTournament {Slug = "sevens",ProviderTournamentId = 831},
                new RugbyTournament {Slug = "six-nations",ProviderTournamentId = 301},
                new RugbyTournament {Slug = "super-rugby",ProviderTournamentId = 181},
                new RugbyTournament {Slug = "top14",ProviderTournamentId = 241},
                new RugbyTournament {Slug = "england",ProviderTournamentId = 201},
                new RugbyTournament {Slug = "new-zealand",ProviderTournamentId = 101}
            };
        }
    }
}
