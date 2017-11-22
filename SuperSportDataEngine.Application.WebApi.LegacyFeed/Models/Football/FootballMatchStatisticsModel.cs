namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Football
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;

    [Serializable]
    public class FootballMatchStatisticsModel : MatchStatisticsModel
    {
        public int Shots { get; set; }
        public int ShotsOn { get; set; }
        public int Fouls { get; set; }
        public int Corners { get; set; }
        public int Offsides { get; set; }
        public int TeamId { get; set; }
    }
}