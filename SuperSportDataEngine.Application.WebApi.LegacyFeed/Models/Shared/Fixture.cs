namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class Fixture : Match
    {
        public SportType Sport { get; set; }
        public int Preview { get; set; }
        public int Sorting { get; set; }
        public string video { get; set; }
    }
}