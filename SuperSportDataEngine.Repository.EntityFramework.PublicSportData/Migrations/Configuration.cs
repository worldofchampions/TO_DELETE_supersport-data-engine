namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
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
            context.DataProviders.AddOrUpdate(
                x => x.Code,
                new DataProvider { Code = DataProviderCodeStatsProzone, Name = "STATS - Prozone" });
        }
    }
}