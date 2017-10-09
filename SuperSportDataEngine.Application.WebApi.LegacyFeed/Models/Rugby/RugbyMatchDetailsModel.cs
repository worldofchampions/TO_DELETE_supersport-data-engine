using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby
{
    [Serializable]
    public class RugbyMatchDetailsModel : MatchDetailsModel
    {
        //private bool _isScoredLive = true;

        //public override List<PlayerModel> Teamsheet { get; set; }

        //public override List<PlayerModel> TeamATeamsheet { get; set; }

        //public override List<PlayerModel> TeamBTeamsheet { get; set; }

        //public override List<ScorerModel> TeamAScorers { get; set; }

        //public override List<ScorerModel> TeamBScorers { get; set; }

        //public override List<SubstituteModel> TeamASubstitutes { get; set; }

        //public override List<SubstituteModel> TeamBSubstitutes { get; set; }

        //public override List<CardsModel> TeamACards { get; set; }

        //public override List<CardsModel> TeamBCards { get; set; }

        //private List<SubstituteModel> GetSubstitutes { get; set; }

        //private string GetPlayerName { get; set; }

        //private List<CardsModel> GetCards { get; set; }

        //private bool PlayerOneDetails { get; set; }
        //private List<ScorerModel> GetScorers { get; set; }

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