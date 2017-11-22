using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class Result : Match
    {
        public SportType Sport { get; set; }
        public int Sorting { get; set; }
        public virtual List<Scorer> HomeTeamScorers { get; set; }
        public virtual List<Scorer> AwayTeamScorers { get; set; }
    }
}