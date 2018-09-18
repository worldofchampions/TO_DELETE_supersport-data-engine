using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.ChampionsCup;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.JuniorRugby;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.MitreCup;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Pro14;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2017;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.Sevens._2018;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.SuperRugby;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed.Rugby.LogGroups.SupersportChallenge;

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

            SeedRugbyLogGroupsForSupersportChallenge.Seed(context);
            SeedRugbyLogGroupsForJuniorRugbyWorldCup2018.Seed(context);

            SeedRugbyLogGroupsForTournamentSuperRugby2019.Seed(context);
            SeedRugbyLogGroupsForSupersportChallenge2019.Seed(context);
            SeedRugbyLogGroupsForJuniorRugbyWorldCup2019.Seed(context);
            SeedRugbyLogGroupsForChampionsCup2019.Seed(context);
        }

        private static void SeedSevens(PublicSportDataContext context)
        {
            Sevens2017Round01Dubai.Seed(context);
            Sevens2018Round03Sydney.Seed(context);
            Sevens2018Round04Hamilton.Seed(context);
            Sevens2018Round05LasVegas.Seed(context);
            Sevens2018Round06Vancouver.Seed(context);
            Sevens2018Round07HongKong.Seed(context);
            Sevens2018Round08Singapore.Seed(context);
            Sevens2018Round09London.Seed(context);
            Sevens2018Round10Paris.Seed(context);
        }
    }
}