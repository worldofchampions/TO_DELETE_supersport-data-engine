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
            SeedRugbyEventTypes.Seed(context);
            SeedRugbyEventTypeProviderMappings.Seed(context);
            SeedRugbyTeams.Seed(context);
            SeedRugbyLogGroupsForTournamentSuperRugby2017.Seed(context);
            SeedRugbyLogGroupsForTournamentSevens2017.Seed(context);
        }
    }
}