using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels
{
    public class RugbyMatchStatsResponse
    {
        public RugbyMatchStats RugbyMatchStats { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}
