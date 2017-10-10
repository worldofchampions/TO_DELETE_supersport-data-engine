namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class SeedRugbyEventTypeProviderMappings
    {
        public static void Seed(PublicSportDataContext context)
        {
            if (context.RugbyEventTypeProviderMappings.Any())
                return;

            // TODO: [Davide] This is a temporary data, populate with the final data set once all the mappings have been confirmed.
            context.RugbyEventTypeProviderMappings.AddOrUpdate(
                x => x.DataProvider,
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 2, ProviderEventName = "Cards-YC", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_10_Yellow_Card) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 3, ProviderEventName = "Cards-RC", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_11_Red_Card) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 5, ProviderEventName = "Try", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_2_Try) }
            );

            context.SaveChanges();
        }
    }
}