using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class RankingModel: PersonModel
    {
        public int Rank { get; set; }
        public string Points { get; set; }
        public int Movement { get; set; }
        public DateTime TimeStamp { get; set; }
        public string LeagueName { get; set; }
        public string LeagueURLName { get; set; }
    }
}
