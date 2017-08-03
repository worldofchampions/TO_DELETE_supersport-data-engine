using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    [Serializable]
    public class TennisTournamentModel
    {
        public enum TennisType
        {
            ATP,
            WTA
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Currency { get; set; }
        public string Location { get; set; }
        public string Surface { get; set; }
        public string PrizeMoney { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}