using System.Collections.Generic;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    public static class SeedCmsTeamNames
    {
        public static void Seed(PublicSportDataContext context)
        {
            var teamsToUpdate = GetTeamsToUpdate();

            foreach (var team in teamsToUpdate)
            {
                var dbTeam =
                    context.RugbyTeams.FirstOrDefault(t => t.ProviderTeamId == team.ProviderTeamId);

                if(dbTeam is null) continue;

                dbTeam.NameCmsOverride = team.NameCmsOverride;
            }

            context.SaveChanges();
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
                new RugbyTeam {ProviderTeamId = 1257, NameCmsOverride = "DHL Western Province"}
            };
        }
    }
}
