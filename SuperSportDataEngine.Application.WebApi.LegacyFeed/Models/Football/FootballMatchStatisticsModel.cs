using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Football
{
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