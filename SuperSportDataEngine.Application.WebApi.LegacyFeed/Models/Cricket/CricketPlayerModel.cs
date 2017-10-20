using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cricket
{
    [Serializable]
    public class CricketPlayerModel: Player
    {
        public bool IsCaptain { get; set; }
        public bool IsKeeper { get; set; }
    }
}
