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
            }
        }

        private static IEnumerable<RugbyTeam> GetTeamsToUpdate()
        {
            return new List<RugbyTeam>
            {
                new RugbyTeam {ProviderTeamId = 1252, NameCmsOverride = "Vodacom Blue Bulls"},
                new RugbyTeam {ProviderTeamId = 1216, NameCmsOverride = "Down Touch Griffons"},
                new RugbyTeam {ProviderTeamId = 1519, NameCmsOverride = "Windhoek Draught Welwitschias"},
                new RugbyTeam {ProviderTeamId = 1260, NameCmsOverride = "EP Elephants"},
                new RugbyTeam {ProviderTeamId = 1253, NameCmsOverride = "Toyota Free State XV"},
                new RugbyTeam {ProviderTeamId = 1254, NameCmsOverride = "Xerox Golden Lions XV"},
                new RugbyTeam {ProviderTeamId = 1255, NameCmsOverride = "Tafel Lager Griquas"},
                new RugbyTeam {ProviderTeamId = 1256, NameCmsOverride = "Cell C Sharks XV"},
                new RugbyTeam {ProviderTeamId = 1263, NameCmsOverride = "iCOLLEGE Pumas"},
                new RugbyTeam {ProviderTeamId = 1257, NameCmsOverride = "DHL Western Province"},
                //Sevens
                new RugbyTeam {ProviderTeamId = 1157, NameCmsOverride = "Argentina"},
                new RugbyTeam {ProviderTeamId = 1158, NameCmsOverride = "Australia"},
                new RugbyTeam {ProviderTeamId = 1159, NameCmsOverride = "Canada"},
                new RugbyTeam {ProviderTeamId = 1160, NameCmsOverride = "England"},
                new RugbyTeam {ProviderTeamId = 1161, NameCmsOverride = "Fiji"},
                new RugbyTeam {ProviderTeamId = 1162, NameCmsOverride = "France"},
                new RugbyTeam {ProviderTeamId = 1163, NameCmsOverride = "Georgia"},
                new RugbyTeam {ProviderTeamId = 1164, NameCmsOverride = "Italy"},
                new RugbyTeam {ProviderTeamId = 1165, NameCmsOverride = "Ireland"},
                new RugbyTeam {ProviderTeamId = 1166, NameCmsOverride = "Japan"},
                new RugbyTeam {ProviderTeamId = 1167, NameCmsOverride = "Namibia"},
                new RugbyTeam {ProviderTeamId = 1168, NameCmsOverride = "New Zealand"},
                new RugbyTeam {ProviderTeamId = 1169, NameCmsOverride = "Portugal"},
                new RugbyTeam {ProviderTeamId = 1170, NameCmsOverride = "Romania"},
                new RugbyTeam {ProviderTeamId = 1171, NameCmsOverride = "Russia"},
                new RugbyTeam {ProviderTeamId = 1172, NameCmsOverride = "Samoa"},
                new RugbyTeam {ProviderTeamId = 1173, NameCmsOverride = "Scotland"},
                new RugbyTeam {ProviderTeamId = 1174, NameCmsOverride = "South Africa"},
                new RugbyTeam {ProviderTeamId = 1175, NameCmsOverride = "Spain"},
                new RugbyTeam {ProviderTeamId = 1176, NameCmsOverride = "Tonga"},
                new RugbyTeam {ProviderTeamId = 1177, NameCmsOverride = "Uruguay"},
                new RugbyTeam {ProviderTeamId = 1178, NameCmsOverride = "USA"},
                new RugbyTeam {ProviderTeamId = 1179, NameCmsOverride = "Wales"},
                new RugbyTeam {ProviderTeamId = 1384, NameCmsOverride = "Kenya"},
                new RugbyTeam {ProviderTeamId = 1385, NameCmsOverride = "South Korea"},
                new RugbyTeam {ProviderTeamId = 1386, NameCmsOverride = "Zimbabwe"},
                new RugbyTeam {ProviderTeamId = 1387, NameCmsOverride = "Papua New Guinea"},
                new RugbyTeam {ProviderTeamId = 1388, NameCmsOverride = "Tunisia"},
                new RugbyTeam {ProviderTeamId = 1389, NameCmsOverride = "Morocco"},
                new RugbyTeam {ProviderTeamId = 1390, NameCmsOverride = "Hong Kong"},
                new RugbyTeam {ProviderTeamId = 1392, NameCmsOverride = "China"},
                new RugbyTeam {ProviderTeamId = 1393, NameCmsOverride = "Niue"},
                new RugbyTeam {ProviderTeamId = 1394, NameCmsOverride = "Germany"},
                new RugbyTeam {ProviderTeamId = 1395, NameCmsOverride = "Mexico"},
                new RugbyTeam {ProviderTeamId = 1396, NameCmsOverride = "Uganda"},
                new RugbyTeam {ProviderTeamId = 1485, NameCmsOverride = "Zimbabwe"},
                new RugbyTeam {ProviderTeamId = 1515, NameCmsOverride = "America Samoa"},
                new RugbyTeam {ProviderTeamId = 1516, NameCmsOverride = "Brazil"},
                new RugbyTeam {ProviderTeamId = 1570, NameCmsOverride = "Belgium"},
                new RugbyTeam {ProviderTeamId = 1936, NameCmsOverride = "Chile"}
            };
        }
    }
}
