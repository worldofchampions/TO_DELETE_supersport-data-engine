namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class Scorer : PersonModel
    {
        public string Type { get; set; }
        public int EventId { get; set; }
        public int Time { get; set; }
    }
}