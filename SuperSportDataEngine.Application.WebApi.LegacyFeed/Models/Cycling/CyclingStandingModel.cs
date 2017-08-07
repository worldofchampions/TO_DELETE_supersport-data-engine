using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    [Serializable]
    public class CyclingStandingModel
    {
        public enum CyclingStandingType
        {
            Point,
            Time
        }

        public string Gap { get; set; }
        public int Position { get; set; }
        public string Points { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string Time { get; set; }
        public CyclingStandingType Type { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
    }
}
