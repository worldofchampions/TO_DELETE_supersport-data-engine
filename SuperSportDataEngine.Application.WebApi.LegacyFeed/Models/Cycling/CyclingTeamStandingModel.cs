using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    [Serializable]
    public class CyclingTeamStandingModel
    {
        public string Gap { get; set; }
        public int Position { get; set; }
        public string Points { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Time { get; set; }
        public CyclingStandingModel.CyclingStandingType Type { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
    }
}
