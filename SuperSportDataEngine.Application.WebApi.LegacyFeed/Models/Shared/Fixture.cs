using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class Fixture : MatchModel
    {
        public SportType Sport { get; set; }
        public int Preview { get; set; }
        public int Sorting { get; set; }
        public string video { get; set; }
    }
}