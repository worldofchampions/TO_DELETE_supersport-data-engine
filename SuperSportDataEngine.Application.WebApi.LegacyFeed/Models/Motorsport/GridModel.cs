using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    [Serializable]
    public class GridModel : ResultMotorsportModel
    {
        public bool StartInPit { get; set; }
    }
}