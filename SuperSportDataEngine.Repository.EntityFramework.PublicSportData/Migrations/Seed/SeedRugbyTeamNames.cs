using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedRugbyTeamNames
    {
        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                var teamsToUpdate = GetTeamsToUpdate();

                foreach (var team in teamsToUpdate)
                {
                    var dbTeam =
                        context.RugbyTeams.FirstOrDefault(t => t.ProviderTeamId == team.ProviderTeamId);

                    if (dbTeam is null) continue;
                    if (!string.IsNullOrEmpty(dbTeam.NameCmsOverride)) continue;

                    dbTeam.NameCmsOverride = team.NameCmsOverride;
                    context.RugbyTeams.AddOrUpdate(dbTeam);
                }

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
                return;
            }
        }

        private static IEnumerable<RugbyTeam> GetTeamsToUpdate()
        {
            return new List<RugbyTeam>
            {
                new RugbyTeam {ProviderTeamId = 1252, NameCmsOverride = "Vodacom Blue Bulls"},
                new RugbyTeam {ProviderTeamId = 1253, NameCmsOverride = "Toyota Free State Cheetahs"},
                new RugbyTeam {ProviderTeamId = 1254, NameCmsOverride = "Xerox Golden Lions"},
                new RugbyTeam {ProviderTeamId = 1255, NameCmsOverride = "Tafel Lager Griquas"},
                new RugbyTeam {ProviderTeamId = 1256, NameCmsOverride = "Cell C Sharks"},
                new RugbyTeam {ProviderTeamId = 1263, NameCmsOverride = "Steval Pumas"},
                new RugbyTeam {ProviderTeamId = 1257, NameCmsOverride = "DHL Western Province"},
                //Sevens
                new RugbyTeam {ProviderTeamId = 1157, NameCmsOverride = "Argentina"},
                new RugbyTeam {ProviderTeamId = 1158, NameCmsOverride = "Australia"},
                new RugbyTeam {ProviderTeamId = 1159, NameCmsOverride = "Canada"},
                new RugbyTeam {ProviderTeamId = 1160, NameCmsOverride = "England"},
                new RugbyTeam {ProviderTeamId = 1161, NameCmsOverride = "Fiji"},
                new RugbyTeam {ProviderTeamId = 1162, NameCmsOverride = "France"},
                new RugbyTeam {ProviderTeamId = 1166, NameCmsOverride = "Japan"},
                new RugbyTeam {ProviderTeamId = 1168, NameCmsOverride = "New Zealand"},
                new RugbyTeam {ProviderTeamId = 1171, NameCmsOverride = "Russia"},
                new RugbyTeam {ProviderTeamId = 1172, NameCmsOverride = "Samoa"},
                new RugbyTeam {ProviderTeamId = 1173, NameCmsOverride = "Scotland"},
                new RugbyTeam {ProviderTeamId = 1174, NameCmsOverride = "South Africa"},
                new RugbyTeam {ProviderTeamId = 1175, NameCmsOverride = "Spain"},
                new RugbyTeam {ProviderTeamId = 1178, NameCmsOverride = "USA"},
                new RugbyTeam {ProviderTeamId = 1179, NameCmsOverride = "Wales"},
                new RugbyTeam {ProviderTeamId = 1384, NameCmsOverride = "Kenya"},
                new RugbyTeam {ProviderTeamId = 1387, NameCmsOverride = "Papua New Guinea"},
                new RugbyTeam {ProviderTeamId = 1396, NameCmsOverride = "Uganda"}
            };
        }
    }
}
