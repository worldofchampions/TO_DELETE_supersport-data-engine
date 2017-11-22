using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Football
{
    [Serializable]
    public class FootballSeasonModel : SeasonModel
    {
        public bool ScoredLive { get; set; }
    }
}
