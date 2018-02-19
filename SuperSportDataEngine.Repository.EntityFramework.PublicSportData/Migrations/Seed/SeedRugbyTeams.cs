using System;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

    public static class SeedRugbyTeams
    {
        private const int TbcProviderTeamId = 0;
        private const string TbcTeamName = "TBC";
        private const string TbcTeamNameAbbreviation = "TBC";
        private const string TbcTeamLogoUrl = null;

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                if (context.RugbyTeams.FirstOrDefault(t => t.ProviderTeamId == TbcProviderTeamId) != null)
                    return;

                context.RugbyTeams.AddOrUpdate(
                    t => t.Id,
                    new RugbyTeam
                    {
                        Name = TbcTeamName,
                        ProviderTeamId = TbcProviderTeamId,
                        LogoUrl = TbcTeamLogoUrl,
                        Abbreviation = TbcTeamNameAbbreviation
                    });

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
            }
        }
    }
}
