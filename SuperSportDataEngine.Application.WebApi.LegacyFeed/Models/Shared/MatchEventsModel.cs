namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    public static class MatchEventsModel
    {
        public enum RugbyEvents
        {
            None = 1,
            Try,
            Conversion,
            Penalty,
            Drop_Goal,
            Penalty_Try,
            Missed_Conversion,
            Missed_Penalty,
            Missed_Drop_Goal,
            Yellow_Card,
            Red_Card,
            Substitution_Blood,
            Substitution_Injury,
            Substitution_Yellow_card,
            Substitution_Tactical,
            Substitution_Blood_bin_returning,
            Substitution_Front_row_returning,
            None2,
            First_Half_End,
            Second_Half_Start,
            Full_Time,
            Extra_Time_Start,
            Extra_Time_First_Half_End,
            Extra_Time_Second_Half_Start,
            Extra_Time_End,
            Substitution_Injury_without_replacement,
            Yellow_card_returning,
            Second_Yellow_Card_in_game,
            Substitution_YC_replacement_returning,
            Substitution_Red_Card_off,
            Substitution_Red_Card_on,
            Drop_Goal_From_Mark,
            Substitution_Blood_without_replacement,
            First_Half_Start
        }

        public enum FootballEventsPA
        {
            BookingT = 1,
            Goal,
            Own_Goal,
            Golden_Goal,
            Penalty,
            Missed_Penalty,
            Saved_Penalty,
            Booking,
            Dismissal,
            Substitution,
            Own_Golden_Goal,
            Match_Status,
            ShootOut_Penalty,
            Missed_ShooutOut_Penalty,
            Saved_ShooutOut_Penalty,
            Assists
        }

        public enum FootballEvents
        {
            None = 1,
            Goal,
            Own_Goal,
            Penalty,
            Yellow_Card,
            Red_Card,
            Substitution_Injury,
            Substitution_Tactical,
            Missed_Penalty,
            Corner,
            Free_Kick,
            Assists
        }
    }
}