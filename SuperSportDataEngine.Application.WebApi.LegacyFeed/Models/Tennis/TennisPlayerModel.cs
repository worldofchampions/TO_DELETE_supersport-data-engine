using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    [Serializable]
    public class TennisPlayerModel : PersonModel
    {
        public string Country { get; set; }
        public int Seed { get; set; }
    }
}