using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public abstract class MatchStatisticsModel
    {
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}
