using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Constants;

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
                new RugbyTournament {Slug = RugbyTournamentConstants.BritishLionsSlugName, ProviderTournamentId = 761},
                new RugbyTournament {Slug = RugbyTournamentConstants.ChampionsCupSlugName, ProviderTournamentId = 291},
                new RugbyTournament {Slug = RugbyTournamentConstants.CravenWeekSlugName, ProviderTournamentId = 129},
                new RugbyTournament {Slug = RugbyTournamentConstants.CurrieCupSlugName, ProviderTournamentId = 121},
                new RugbyTournament {Slug = RugbyTournamentConstants.InternationalSlugName, ProviderTournamentId = 810},
                new RugbyTournament {Slug = RugbyTournamentConstants.Pro14, ProviderTournamentId = 293},
                new RugbyTournament {Slug = RugbyTournamentConstants.ProD2SlugName, ProviderTournamentId = 242},
                new RugbyTournament {Slug = RugbyTournamentConstants.NationalRugbyChampionshipSlugName, ProviderTournamentId = 117}, // National Rugby Championship
                new RugbyTournament {Slug = RugbyTournamentConstants.SupersportChallengeSlugName, ProviderTournamentId = 165}, // Rugby Challenge
                new RugbyTournament {Slug = RugbyTournamentConstants.RugbyChampionshipSlugName, ProviderTournamentId = 191}, // The Rugby Championship
                new RugbyTournament {Slug = RugbyTournamentConstants.SevensSlugName, ProviderTournamentId = 831},
                new RugbyTournament {Slug = RugbyTournamentConstants.SixNationsSlugName, ProviderTournamentId = 301},
                new RugbyTournament {Slug = RugbyTournamentConstants.SuperRugbySlugName, ProviderTournamentId = 181},
                new RugbyTournament {Slug = RugbyTournamentConstants.Top14SlugName, ProviderTournamentId = 241},
                new RugbyTournament {Slug = RugbyTournamentConstants.EnglandSlugName, ProviderTournamentId = 201},
                new RugbyTournament {Slug = RugbyTournamentConstants.NewZealandSlugName, ProviderTournamentId = 101},
                new RugbyTournament {Slug = RugbyTournamentConstants.JuniorRugbySlugName, ProviderTournamentId = 791}
            };
        }
    }
}
