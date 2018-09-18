namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;

    [Serializable]
    public class TennisPlayer : PersonModel
    {
        public string Country { get; set; }
        public int Seed { get; set; }
    }
}