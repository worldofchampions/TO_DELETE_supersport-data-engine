using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    [Serializable]
    public class CyclingStageModel
    {
        public enum CyclingStageType
        {
            Prologue,
            Stage,
            TeamTimeTrial,
            TimeTrial
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public string Distance { get; set; }
        public int TournamentId { get; set; }
        public CyclingStageType Type { get; set; }
        public DateTime Date { get; set; }
        public List<MatchVideoModel> Videos { get; set; }
    }
}
