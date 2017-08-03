using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class FixtureModel : MatchModel
    {
        public SportType Sport { get; set; }
        public int Preview { get; set; }
        public int Sorting { get; set; }
        public string video { get; set; }
    }
}