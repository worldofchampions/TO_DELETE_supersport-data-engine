using System;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations.Seed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class SeedRugbyEventTypes
    {
        public const int EventCode_1_None = 1;
        public const int EventCode_2_Try = 2;
        public const int EventCode_3_Conversion = 3;
        public const int EventCode_4_Penalty = 4;
        public const int EventCode_5_Drop_Goal = 5;
        public const int EventCode_6_Penalty_Try_5pt = 6;
        public const int EventCode_7_Missed_Conversion = 7;
        public const int EventCode_8_Missed_Penalty = 8;
        public const int EventCode_9_Missed_Drop_Goal = 9;
        public const int EventCode_10_Yellow_Card = 10;
        public const int EventCode_11_Red_Card = 11;
        public const int EventCode_12_Substitution_Blood = 12;
        public const int EventCode_13_Substitution_Injury = 13;
        public const int EventCode_14_Substitution_Yellow_Card = 14;
        public const int EventCode_15_Substitution_Tactical = 15;
        public const int EventCode_16_Substitution_Blood_Bin_Returning = 16;
        public const int EventCode_17_Substitution_Front_Row_Returning = 17;
        public const int EventCode_19_First_Half_End = 19;
        public const int EventCode_20_Second_Half_Start = 20;
        public const int EventCode_21_Full_Time = 21;
        public const int EventCode_22_Extra_Time_Start = 22;
        public const int EventCode_23_Extra_Time_First_Half_End = 23;
        public const int EventCode_24_Extra_Time_Second_Half_Start = 24;
        public const int EventCode_25_Extra_Time_End = 25;
        public const int EventCode_26_Substitution_Injury_Without_Replacement = 26;
        public const int EventCode_27_Yellow_Card_Returning = 27;
        public const int EventCode_28_2nd_Yellow_Card_In_Game = 28;
        public const int EventCode_29_Substitution_YC_Replacement_Returning = 29;
        public const int EventCode_30_Substitution_Red_Card_Off = 30;
        public const int EventCode_31_Substitution_Red_Card_On = 31;
        public const int EventCode_32_Drop_Goal_From_Mark = 32;
        public const int EventCode_33_Substitution_Blood_Without_Replacement = 33;
        public const int EventCode_34_First_Half_Start = 34;
        public const int EventCode_101_White_Card = 101;
        public const int EventCode_111_Substitution_Concussion = 111;
        public const int EventCode_114_Penalty_Try_7pt = 114;
        public const int EventCode_121_Substitution_Concussion_Returning = 121;
        public const int EventCode_200_Substitution_In = 200;
        public const int EventCode_201_Substitution_Out = 201;

        public static void Seed(PublicSportDataContext context)
        {
            try
            {
                if (context.RugbyEventTypes.Any())
                    return;

                context.RugbyEventTypes.AddOrUpdate(
                    x => x.EventCode,
                    new RugbyEventType { EventCode = EventCode_1_None, EventName = "None" },
                    new RugbyEventType { EventCode = EventCode_2_Try, EventName = "Try" },
                    new RugbyEventType { EventCode = EventCode_3_Conversion, EventName = "Conversion" },
                    new RugbyEventType { EventCode = EventCode_4_Penalty, EventName = "Penalty" },
                    new RugbyEventType { EventCode = EventCode_5_Drop_Goal, EventName = "Drop Goal" },
                    new RugbyEventType { EventCode = EventCode_6_Penalty_Try_5pt, EventName = "Penalty Try (5pt)" },
                    new RugbyEventType { EventCode = EventCode_7_Missed_Conversion, EventName = "Missed Conversion" },
                    new RugbyEventType { EventCode = EventCode_8_Missed_Penalty, EventName = "Missed Penalty" },
                    new RugbyEventType { EventCode = EventCode_9_Missed_Drop_Goal, EventName = "Missed Drop Goal" },
                    new RugbyEventType { EventCode = EventCode_10_Yellow_Card, EventName = "Yellow Card" },
                    new RugbyEventType { EventCode = EventCode_11_Red_Card, EventName = "Red Card" },
                    new RugbyEventType { EventCode = EventCode_12_Substitution_Blood, EventName = "Substitution (Blood)" },
                    new RugbyEventType { EventCode = EventCode_13_Substitution_Injury, EventName = "Substitution (Injury)" },
                    new RugbyEventType { EventCode = EventCode_14_Substitution_Yellow_Card, EventName = "Substitution (Yellow card)" },
                    new RugbyEventType { EventCode = EventCode_15_Substitution_Tactical, EventName = "Substitution (Tactical)" },
                    new RugbyEventType { EventCode = EventCode_16_Substitution_Blood_Bin_Returning, EventName = "Substitution (Blood bin returning)" },
                    new RugbyEventType { EventCode = EventCode_17_Substitution_Front_Row_Returning, EventName = "Substitution (Front row returning)" },
                    new RugbyEventType { EventCode = EventCode_19_First_Half_End, EventName = "First Half End" },
                    new RugbyEventType { EventCode = EventCode_20_Second_Half_Start, EventName = "Second Half Start" },
                    new RugbyEventType { EventCode = EventCode_21_Full_Time, EventName = "Full Time" },
                    new RugbyEventType { EventCode = EventCode_22_Extra_Time_Start, EventName = "Extra Time Start" },
                    new RugbyEventType { EventCode = EventCode_23_Extra_Time_First_Half_End, EventName = "Extra Time First Half End" },
                    new RugbyEventType { EventCode = EventCode_24_Extra_Time_Second_Half_Start, EventName = "Extra Time Second Half Start" },
                    new RugbyEventType { EventCode = EventCode_25_Extra_Time_End, EventName = "Extra Time End" },
                    new RugbyEventType { EventCode = EventCode_26_Substitution_Injury_Without_Replacement, EventName = "Substitution (Injury without replacement)" },
                    new RugbyEventType { EventCode = EventCode_27_Yellow_Card_Returning, EventName = "Yellow card returning" },
                    new RugbyEventType { EventCode = EventCode_28_2nd_Yellow_Card_In_Game, EventName = "2nd Yellow Card in game" },
                    new RugbyEventType { EventCode = EventCode_29_Substitution_YC_Replacement_Returning, EventName = "Substitution (YC replacement returning)" },
                    new RugbyEventType { EventCode = EventCode_30_Substitution_Red_Card_Off, EventName = "Substitution (Red Card off)" },
                    new RugbyEventType { EventCode = EventCode_31_Substitution_Red_Card_On, EventName = "Substitution (Red Card on)" },
                    new RugbyEventType { EventCode = EventCode_32_Drop_Goal_From_Mark, EventName = "Drop Goal From Mark" },
                    new RugbyEventType { EventCode = EventCode_33_Substitution_Blood_Without_Replacement, EventName = "Substitution (Blood without replacement)" },
                    new RugbyEventType { EventCode = EventCode_34_First_Half_Start, EventName = "First Half Start" },
                    new RugbyEventType { EventCode = EventCode_101_White_Card, EventName = "White Card" },
                    new RugbyEventType { EventCode = EventCode_111_Substitution_Concussion, EventName = "Substitution (Concussion)" },
                    new RugbyEventType { EventCode = EventCode_114_Penalty_Try_7pt, EventName = "Penalty Try (7pt)" },
                    new RugbyEventType { EventCode = EventCode_121_Substitution_Concussion_Returning, EventName = "Substitution (Concussion returning)" },
                    new RugbyEventType { EventCode = EventCode_200_Substitution_In, EventName = "Substitution In" },
                    new RugbyEventType { EventCode = EventCode_201_Substitution_Out, EventName = "Substitution Out" }
                );

                context.SaveChanges();
            }
            catch (Exception exception)
            {
                // TODO: Add logging.
                Console.WriteLine(exception);
                return;
            }
        }
    }
}