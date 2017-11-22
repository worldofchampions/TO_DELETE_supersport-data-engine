using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby
{
    [Serializable]
    public class RugbyMatchStatisticsModel : MatchStatisticsModel
    {
        public int Tries { get; set; }
        public int Conversions { get; set; }
        public int ConversionsMissed { get; set; }
        public int Penalties { get; set; }
        public int PenaltiesMissed { get; set; }
        public int DropGoals { get; set; }
        public int DropGoalsMissed { get; set; }
        public int ConversionAttempts { get; set; }
        public int PenaltyAttempts { get; set; }
        public int DropGoalAttempts { get; set; }
        public int DefendersBeaten { get; set; }
        public int Offloads { get; set; }
        public int LineOutsWon { get; set; }
        public int LineOutsLost { get; set; }
        public int PenaltiesConceded { get; set; }
        public int Tackles { get; set; }
        public int Territory { get; set; }
        public int Passes { get; set; }
        public int CleanBreaks { get; set; }
        public int ScrumsWon { get; set; }
        public int MissedTackles { get; set; }
        public int Possession { get; set; }
        public int CarriesCrossedGainLine { get; set; }
        public int ScrumsLost { get; set; }
        public int PenaltyTries { get; set; }

        private List<MatchEvent> matchEvents { get; set; }
        private int teamId { get; set; }
    }
}