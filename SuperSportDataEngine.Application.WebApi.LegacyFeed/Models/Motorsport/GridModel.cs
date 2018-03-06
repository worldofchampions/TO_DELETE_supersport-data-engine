namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class GridModel : ResultMotorsportModel
    {
        public bool StartInPit { get; set; }
    }
}
