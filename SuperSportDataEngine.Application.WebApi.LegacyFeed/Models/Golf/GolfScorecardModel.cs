namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Golf
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GolfScorecardModel
    {
        public string id { get; set; }
        public string name { get; set; }

        public int CurrentRound { get; set; }
        public List<GolfPlayer> Players { get; set; }
    }

    [Serializable]
    public class GolfPlayer : PersonModel
    {
        public string Country { get; set; }
        public List<GolfRound> Rounds { get; set; }
    }

    [Serializable]
    public class GolfRound
    {
        public int number { get; set; }
        public string par { get; set; }
        public int strokes { get; set; }
        public List<GolfHole> Holes { get; set; }
    }

    [Serializable]
    public class GolfHole
    {
        public int number { get; set; }
        public string par { get; set; }
        public int strokes { get; set; }
    }
}