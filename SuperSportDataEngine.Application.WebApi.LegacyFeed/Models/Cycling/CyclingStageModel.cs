namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;
    using System.Collections.Generic;

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
        public List<MatchVideo> Videos { get; set; }
    }
}