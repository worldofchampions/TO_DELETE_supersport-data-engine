using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cricket
{
    [Serializable]
    public class CricketBowlingStatisticsModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wickets { get; set; }
        public double BowlingAverage { get; set; }
        public double BowlingStrikeRate { get; set; }
        public int Runs { get; set; }
        public int Balls { get; set; }
        public int Innings { get; set; }
    }
}
