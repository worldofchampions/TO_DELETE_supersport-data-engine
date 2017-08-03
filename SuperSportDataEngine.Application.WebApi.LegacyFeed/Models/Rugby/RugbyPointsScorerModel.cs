using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby
{
    [Serializable]
    public class RugbyPointsScorerModel : PersonModel
    {
        public int Tries { get; set; }
        public int Conversions { get; set; }
        public int Penalties { get; set; }
        public int DropGoals { get; set; }
        public int TotalPoints { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public string LeagueUrlName { get; set; }
        public string LeagueName { get; set; }
        public int Rank { get; set; }
    }
}