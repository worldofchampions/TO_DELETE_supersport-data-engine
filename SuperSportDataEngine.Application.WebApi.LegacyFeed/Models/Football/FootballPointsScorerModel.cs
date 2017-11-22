namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Football
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;

    [Serializable]
    public class FootballPointsScorerModel : PersonModel
    {
        public string fullName { get; set; }
        public int Goals { get; set; }
        public int Rank { get; set; }
        public int Penalties { get; set; }
        public int OwnGoals { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public string LeagueUrlName { get; set; }
        public string LeagueName { get; set; }
    }
}