namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Result : Match
    {
        public SportType Sport { get; set; }
        public int Sorting { get; set; }
        public virtual List<Scorer> HomeTeamScorers { get; set; }
        public virtual List<Scorer> AwayTeamScorers { get; set; }
    }
}