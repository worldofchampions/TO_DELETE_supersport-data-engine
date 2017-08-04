using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    [Serializable]
    public class TennisRankingsModel : RankingModel
    {
        public enum TennisRankingsType
        {
            Normal,
            Race
        }
    }
}