namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Football
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;

    [Serializable]
    public class FootballSeasonModel : SeasonModel
    {
        public bool ScoredLive { get; set; }
    }
}