using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    //[Serializable]
    public class RaceModel
    {
        public enum MotorSportType
        {
            Formula1,
            MotoGP
        }

        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Circuit { get; set; }
        public List<StageModel> Stages { get; set; }
        public string LeagueName { get; set; }
        public string LeagueURLName { get; set; }

    }
}