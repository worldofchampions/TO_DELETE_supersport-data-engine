namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;

    [Serializable]
    public class TennisRankings : RankingModel
    {
        public enum TennisRankingsType
        {
            Normal,
            Race
        }
    }
}