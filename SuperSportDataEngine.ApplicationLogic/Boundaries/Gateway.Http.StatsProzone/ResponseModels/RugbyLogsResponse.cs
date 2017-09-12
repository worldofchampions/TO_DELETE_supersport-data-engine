using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyLogs;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels
{
    public class RugbyLogsResponse
    {
        public RugbyLogs RugbyLogs { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}
