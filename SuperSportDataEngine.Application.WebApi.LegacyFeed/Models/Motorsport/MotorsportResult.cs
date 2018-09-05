using System.Collections.Generic;
using System.Xml.Serialization;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    [XmlRoot("motorsportResult")]
    public class MotorsportResult
    {
        public List<ResultMotorsport> RaceResult { get; set; }
        public int Id { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Circuit { get; set; }
        public string RaceEventStatus { get; set; }
    }
}