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

            context.RugbyEventTypeProviderMappings.AddOrUpdate(
                x => x.DataProvider,
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 0, ProviderEventName = "Substitution-In", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_200_Substitution_In) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 1, ProviderEventName = "Substitution-Out", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_201_Substitution_Out) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 2, ProviderEventName = "Cards-YC", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_10_Yellow_Card) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 3, ProviderEventName = "Cards-RC", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_11_Red_Card) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 5, ProviderEventName = "Try", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_2_Try) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 9, ProviderEventName = "Conversion-G", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_3_Conversion) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10, ProviderEventName = "Conversion-B", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_7_Missed_Conversion) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 11, ProviderEventName = "Pen Shot-G", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_4_Penalty) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 12, ProviderEventName = "Pen Shot-B", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_8_Missed_Penalty) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 13, ProviderEventName = "Drop Goal-G", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_5_Drop_Goal) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 14, ProviderEventName = "Drop Goal-B", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_9_Missed_Drop_Goal) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 61, ProviderEventName = "Penalty Try", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_6_Penalty_Try_5pt) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10097, ProviderEventName = "Substitution Out - Tactical", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_15_Substitution_Tactical) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10098, ProviderEventName = "Substitution In - Tactical", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_15_Substitution_Tactical) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10099, ProviderEventName = "Substitution Out - Injury", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_13_Substitution_Injury) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10100, ProviderEventName = "Substitution In - Injury", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_13_Substitution_Injury) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10101, ProviderEventName = "Substitution Out - Temporary", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_201_Substitution_Out) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10102, ProviderEventName = "Substitution In - Temporary", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_200_Substitution_In) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10525, ProviderEventName = "Substitution In - Temporary - Replacement", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_200_Substitution_In) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10526, ProviderEventName = "Substitution Out - Temporary - Replacement", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_201_Substitution_Out) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10527, ProviderEventName = "Substitution In - Temporary - Injury", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_13_Substitution_Injury) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10528, ProviderEventName = "Substitution Out - Temporary - Injury", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_13_Substitution_Injury) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10529, ProviderEventName = "Substitution In - Sin Bin", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_14_Substitution_Yellow_Card) },
                new RugbyEventTypeProviderMapping { DataProvider = DataProvider.StatsProzone, ProviderEventTypeId = 10530, ProviderEventName = "Penalty Try - 7 Points", RugbyEventType = context.RugbyEventTypes.Single(s => s.EventCode == SeedRugbyEventTypes.EventCode_114_Penalty_Try_7pt) }
            );

            context.SaveChanges();
        }
    }
}