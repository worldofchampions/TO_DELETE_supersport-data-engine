namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class TeamStandings : Team
    {
        public int Position { get; set; }
        public string Points { get; set; }
        public string LeagueName { get; set; }
        public string LeagueURLName { get; set; }
    }
}
