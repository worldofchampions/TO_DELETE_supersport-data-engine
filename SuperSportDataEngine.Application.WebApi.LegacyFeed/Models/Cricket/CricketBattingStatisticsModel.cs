using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cricket
{
    [Serializable]
    public class CricketBattingStatisticsModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wickets { get; set; }
        public double BattingAverage { get; set; }
        public double BattingStrikeRate { get; set; }
        public int Runs { get; set; }
        public int Balls { get; set; }
        public int Innings { get; set; }
    }
}