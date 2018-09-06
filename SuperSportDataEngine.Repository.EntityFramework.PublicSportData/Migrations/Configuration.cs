using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.ChampionsCup;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.JuniorRugby;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.MitreCup;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Pro14;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2017;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2018;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using Context;
    using Seed;
    using System.Data.Entity.Migrations;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Motorsport;

    internal sealed class Configuration : DbMigrationsConfiguration<PublicSportDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PublicSportDataContext context)
        {
            SeedRugbyData(context);
            SeedMotorsportData(context);
        }

        private static void SeedMotorsportData(PublicSportDataContext context)
        {
            SeedMotorsportLeagueSlugs.Seed(context);
            SeedMotorsportEnabledLeagues.Seed(context);
            SeedMotorsportHistoricSeasons.Seed(context);
            SeedMotorsportCurrentSeasons.Seed(context);
        }

        private static void SeedRugbyData(PublicSportDataContext context)
        {
            SeedRugbyTournamentSlugs.Seed(context);
            SeedRugbyTournamentDefaultLogType.Seed(context);
            SeedRugbyHasLogsTournaments.Seed(context);
            SeedRugbyEventTypes.Seed(context);
            SeedRugbyEventTypeProviderMappings.Seed(context);
            SeedRugbyTeams.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2017.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2018.Seed(context);
            SeedRugbyLogGroupsForChampionsCup2018.Seed(context);
            SeedRugbyLogGroupsForPro142018.Seed(context);
            SeedRugbyLogGroupsForPro142019.Seed(context);
            SeedRugbyGroupsForMitreCup2017.Seed(context);
            SeedRugbyGroupsForMitreCup2018.Seed(context);
            SeedSevens(context);

            SeedRugbyLogGroupsForRugbyChallenge.Seed(context);
            SeedRugbyLogGroupsForJuniorRugbyWorldCup2018.Seed(context);
        }

        private static void SeedSevens(PublicSportDataContext context)
        {
            Round01Dubai.Seed(context);
            Round03Sydney.Seed(context);
            Round04Hamilton.Seed(context);
            Round05LasVegas.Seed(context);
            Round06Vancouver.Seed(context);
            Round07HongKong.Seed(context);
            Round08Singapore.Seed(context);
            Round09London.Seed(context);
            Round10Paris.Seed(context);
        }
    }
}