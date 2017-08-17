using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels
{
    public class EntitiesResponse
    {
        public Entities Entities { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
