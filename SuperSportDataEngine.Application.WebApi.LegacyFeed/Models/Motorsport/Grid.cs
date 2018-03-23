namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class Grid : ResultMotorsport
    {
        public bool StartInPit { get; set; }
    }
}
