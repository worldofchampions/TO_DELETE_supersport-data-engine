using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cricket
{
    [Serializable]
    public class CricketMatchModel : MatchModel
    {
        public string Title { get; set; }
        public ScorecardModel MatchScorecard { get; set; }
        public List<CricketPlayerModel> TeamA { get; set; }
        public List<CricketPlayerModel> TeamB { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string MatchBreak { get; set; }
        public string CricInfoStatus { get; set; }
        public string Scores1 { get; set; }
        public string Scores2 { get; set; }
        public string TossWinner { get; set; }
        public string TossDecision { get; set; }
        public string MOM { get; set; }
        public string MOS { get; set; }
        public string UmpireA { get; set; }
        public string UmpireB { get; set; }
        public string UmpireC { get; set; }
        public string Referee { get; set; }
        public bool IsResult { get; set; }
        public bool MatchCompleted { get; set; }
        public int Report { get; set; }
    }
}