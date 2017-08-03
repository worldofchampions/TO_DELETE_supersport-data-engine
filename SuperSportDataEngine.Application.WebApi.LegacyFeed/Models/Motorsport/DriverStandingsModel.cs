using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport 
{
    [Serializable]
    public class DriverStandingsModel : DriverModel
    {
        public int Position { get; set; }
        public string Points { get; set; }
        public int Wins { get; set; }
        public string LeagueName { get; set; }
        public string LeagueURLName { get; set; }
    }
}