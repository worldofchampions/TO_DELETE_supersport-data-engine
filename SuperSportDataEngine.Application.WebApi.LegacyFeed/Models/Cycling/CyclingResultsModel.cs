using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    [Serializable]
    public class CyclingResultsModel
    {
        public CyclingStageModel Stage { get; set; }
        public List<CyclingResultModel> Results { get; set; }
    }
}
