namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed;
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
            SeedRugbyEventTypes.Seed(context);
            SeedRugbyEventTypeProviderMappings.Seed(context);
            SeedRugbyTeams.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2017.Seed(context);
        }
    }
}