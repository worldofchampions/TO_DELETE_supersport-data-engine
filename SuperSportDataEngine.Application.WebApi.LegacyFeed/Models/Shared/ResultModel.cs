using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class Result : MatchModel
    {
        public SportType Sport { get; set; }
        public int Sorting { get; set; }
        public virtual List<ScorerModel> HomeTeamScorers { get; set; }
        public virtual List<ScorerModel> AwayTeamScorers { get; set; }
    }
}