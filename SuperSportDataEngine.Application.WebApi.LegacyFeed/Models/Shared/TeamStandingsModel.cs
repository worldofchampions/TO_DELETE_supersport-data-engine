namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class TeamStandingsModel : TeamModel
    {
        public int Position { get; set; }
        public string Points { get; set; }
        public string LeagueName { get; set; }
        public string LeagueURLName { get; set; }
    }
}