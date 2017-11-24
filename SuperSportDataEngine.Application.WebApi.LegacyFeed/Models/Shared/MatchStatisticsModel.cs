namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public abstract class MatchStatisticsModel
    {
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}