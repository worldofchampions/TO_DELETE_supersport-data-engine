using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    [XmlRoot("motorsportGrid")]
    public class MotorsportGrid
    {
        public List<Grid> RaceGrid { get; set; }
        public int Id { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Circuit { get; set; }
    }
}