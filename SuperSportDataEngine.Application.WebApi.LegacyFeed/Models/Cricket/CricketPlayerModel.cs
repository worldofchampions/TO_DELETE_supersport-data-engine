namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cricket
{
    using System;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;

    [Serializable]
    public class CricketPlayerModel : Player
    {
        public bool IsCaptain { get; set; }
        public bool IsKeeper { get; set; }
    }
}