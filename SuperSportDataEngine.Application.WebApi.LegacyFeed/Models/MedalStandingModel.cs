using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class MedalsStanding
    {
        public string Name { get; set; }
        public List<MedalStandingModel> MedalStandings { get; set; }
    }

    [Serializable]
    public class MedalStandingModel
    {
        public int Rank { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
        public int Total { get; set; }
    }
}