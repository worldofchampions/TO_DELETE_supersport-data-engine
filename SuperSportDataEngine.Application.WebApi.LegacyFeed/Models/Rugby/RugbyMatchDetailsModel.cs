namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby
{
    using System;
    using System.Collections.Generic;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;

    [Serializable]
    public class RugbyMatchDetails : MatchDetailsModel
    {
        public bool isScoredLive { get; set; }

        public List<AlternateCommentsModel> AlternateCommentary { get; set; }

        public List<Scorer> TeamA_Scorers { get; set; }

        public List<Scorer> TeamB_Scorers { get; set; }

        public RugbyMatchStatisticsModel MatchStatisticsTeamA { get; set; }

        public RugbyMatchStatisticsModel MatchStatisticsTeamB { get; set; }

        public RugbyMatchStatisticsModel teamAStats { get; set; }

        public RugbyMatchStatisticsModel teamBStats { get; set; }
    }
}
