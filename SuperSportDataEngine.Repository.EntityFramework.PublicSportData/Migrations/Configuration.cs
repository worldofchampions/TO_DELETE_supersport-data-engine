namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context.PublicSportDataContext>
    {
        public const string DataProviderCodeStatsProzone = "stats_prozone";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context.PublicSportDataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}