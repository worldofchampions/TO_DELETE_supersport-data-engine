using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels
{
    public class RugbyEventsFlowResponse
    {
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public RugbyEventsFlow RugbyEventsFlow { get; set; }
    }
}
