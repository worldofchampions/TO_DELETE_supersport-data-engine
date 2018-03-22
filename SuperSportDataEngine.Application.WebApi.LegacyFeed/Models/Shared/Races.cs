namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Races
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int TournamentId { get; set; }
        public int ResultId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Date { get; set; }
        public DateTime EndDate { get; set; }
        public string report { get; set; }
        public string winner { get; set; }
        public List<string> channels { get; set; }
        public List<MatchVideoModel> Videos { get; set; }
        public List<MatchLiveVideoModel> LiveVideos { get; set; }
    }
}
