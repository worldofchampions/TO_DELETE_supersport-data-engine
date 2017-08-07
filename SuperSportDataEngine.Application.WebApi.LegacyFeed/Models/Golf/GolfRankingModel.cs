using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Golf
{
    [Serializable]
    public class GolfRankingModel
    {
        public int Ordering { get; set; }
        public string Type { get; set; }
        public string Position { get; set; }
        public bool Tied { get; set; }
        public string PlayerName { get; set; }
        public string Events { get; set; }//how many tournaments played
        public string Money { get; set; }
        public string Currency { get; set; }
        public string Points { get; set; }
        public bool isSouthAfrican { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string LastTournament { get; set; }
        public string LeagueName { get; set; }
        public string LeagueURLName { get; set; }
    }
}