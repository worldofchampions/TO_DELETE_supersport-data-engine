using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels.RugbyRoundFixturesResponse
{
    public class RugbyRoundFixturesResponse
    {
        public RugbyRoundFixtures RoundFixtures { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}
