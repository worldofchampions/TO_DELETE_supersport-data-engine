using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cricket
{
    [Serializable]
    public class CricketRankingModel
    {
        public int Ordering { get; set; }
        public string Type { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Matches { get; set; }
        public string Points { get; set; }
        public string Rating { get; set; }
        public string Best { get; set; }
    }
}