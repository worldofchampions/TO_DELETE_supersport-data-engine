namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Golf
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GolfTournamentModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Venue1 { get; set; }
        public string Venue2 { get; set; }
        public string Champion { get; set; }
        public string PrizeMoney { get; set; }
        public string Report { get; set; }
        public bool isMajor { get; set; }
        public string leaderboardId { get; set; }
        public List<string> channels { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<MatchVideo> Videos { get; set; }
        public List<MatchLiveVideo> LiveVideos { get; set; }
    }
}