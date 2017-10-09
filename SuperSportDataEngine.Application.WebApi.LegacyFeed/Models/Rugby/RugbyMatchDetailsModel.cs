using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby
{
    [Serializable]
    public class RugbyMatchDetailsModel : MatchDetailsModel
    {
        public bool isScoredLive { get; set; }
        public List<AlternateCommentsModel> AlternateCommentary { get; set; }
        public List<ScorerModel> TeamA_Scorers { get; set; }
        public List<ScorerModel> TeamB_Scorers { get; set; }
        public RugbyMatchStatisticsModel MatchStatisticsTeamA { get; set; }
        public RugbyMatchStatisticsModel MatchStatisticsTeamB { get; set; }
        public RugbyMatchStatisticsModel TeamAStats { get; set; }
        public RugbyMatchStatisticsModel TeamBStats { get; set; }
    }
}