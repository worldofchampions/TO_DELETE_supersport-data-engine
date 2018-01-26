namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using Context;
    using Seed;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<PublicSportDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PublicSportDataContext context)
        {
            SeedRugbyTournamentSlugs.Seed(context);
            SeedRugbyTournamentNames.Seed(context);
            SeedRugbyEnabledTournaments.Seed(context);
            SeedRugbyHasLogsTournaments.Seed(context);
            SeedRugbyLiveScoredTournaments.Seed(context);
            SeedCmsTeamNames.Seed(context);
            SeedRugbyEventTypes.Seed(context);
            SeedRugbyEventTypeProviderMappings.Seed(context);
            SeedRugbyTeams.Seed(context);
            SeedRugbyLogGroupsForTournamentSevens2017.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2017.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2018.Seed(context);
            SeedRugbyLogGroupsForChampionsCup2018.Seed(context);
            SeedRugbyLogGroupsForPro142018.Seed(context);
            SeedRugbyGroupsForMitreCup2017.Seed(context);
            SeedRugbyLogGroupsForSevens2018.Seed(context);
        }
    }
}