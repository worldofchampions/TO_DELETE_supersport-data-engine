namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TennisMatch
    {
        public string Type { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public int RoundId { get; set; }
        public string RoundType { get; set; }
        public int RoundOrder { get; set; }
        public int DrawOrder { get; set; }
        public int Sequence { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public string Court { get; set; }
        public int Sets { get; set; }
        public bool Started { get; set; }
        public bool Completed { get; set; }
        public int WinningSide { get; set; }
        public List<TennisSide> Sides { get; set; }
        public List<MatchVideo> Videos { get; set; }
    }
}