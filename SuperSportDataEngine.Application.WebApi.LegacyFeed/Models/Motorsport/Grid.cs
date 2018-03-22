namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class Grid : Result
    {
        public bool StartInPit { get; set; }
    }
}
