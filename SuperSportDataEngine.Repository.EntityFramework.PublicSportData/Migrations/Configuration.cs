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
        }

        private static void SeedRugbyData(PublicSportDataContext context)
        {
            SeedRugbyTournamentSlugs.Seed(context);
            SeedRugbyTournamentNames.Seed(context);
            SeedRugbyEnabledTournaments.Seed(context);
            SeedRugbyHasLogsTournaments.Seed(context);
            SeedRugbyLiveScoredTournaments.Seed(context);
            SeedRugbyTeamNames.Seed(context);
            SeedRugbyEventTypes.Seed(context);
            SeedRugbyEventTypeProviderMappings.Seed(context);
            SeedRugbyTeams.Seed(context);
            SeedRugbyLogGroupsForTournamentSevens2017.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2017.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2018.Seed(context);
            SeedRugbyLogGroupsForChampionsCup2018.Seed(context);
            SeedRugbyLogGroupsForPro142018.Seed(context);
            SeedRugbyGroupsForMitreCup2017.Seed(context);
            SeedRugbyLogGroupsForSydneySevens2018.Seed(context);
            SeedRugbyLogGroupsForHamiltonSevens2018.Seed(context);
            SeedRugbyLogGroupsForLasVegasSevens2018.Seed(context);
        }
    }
}