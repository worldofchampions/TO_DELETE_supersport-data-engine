using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    [Serializable]
    public class TennisSetModel
    {
        public int MatchId { get; set; }
        public int Side { get; set; }
        public int Number { get; set; }
        public int Games { get; set; }
        public int TieBreak { get; set; }
    }
}