namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;

    public class RugbyPlayerStatsResponse
    {
        public RugbyPlayerStats RugbyPlayerStats { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}
