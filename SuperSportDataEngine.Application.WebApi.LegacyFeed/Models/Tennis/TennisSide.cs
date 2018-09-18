using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    [Serializable]
    public class TennisSide
    {
        public int Number { get; set; }
        public List<TennisPlayer> Players { get; set; }
        public List<TennisSet> Sets { get; set; }
    }
}