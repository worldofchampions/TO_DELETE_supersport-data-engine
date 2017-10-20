using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Football
{
    [Serializable]
    public class FootballMatchDetailsModel : MatchDetailsModel
    {
        private int teamAScore = 0;
        private int teamBScore = 0;

        public List<FootballMatchStatisticsModel> TeamsMatchStatistics { get; set; }

        private bool _isScoredLive = true;
        public bool isScoredLive { get; set; }

        public override int TeamAScore { get; set; }

        public override int TeamBScore { get; set; }

        public int TeamAPenalties { get; set; }

        public int TeamBPenalties { get; set; }

        public List<MatchEvent> Commentary { get; set; }

        public List<AlternateCommentsModel> AlternateCommentary { get; set; }

        public bool IsPAMatch { get; set; }

        public FootballMatchStatisticsModel MatchStatisticsTeamA { get; set; }

        public FootballMatchStatisticsModel MatchStatisticsTeamB { get; set; }

        public override List<Player> Teamsheet { get; set; }

        public override List<Player> TeamATeamsheet { get; set; }

        public override List<Player> TeamBTeamsheet { get; set; }

        public override List<ScorerModel> TeamAScorers { get; set; }

        public override List<ScorerModel> TeamBScorers { get; set; }

        public override List<SubstituteModel> TeamASubstitutes { get; set; }

        public override List<SubstituteModel> TeamBSubstitutes { get; set; }
        public override List<CardsModel> TeamACards { get; set; }

        public override List<CardsModel> TeamBCards { get; set; }

        private List<SubstituteModel> GetSubstitutes { get; set; }

        private List<CardsModel> GetCards { get; set; }

        private List<ScorerModel> GetScorers { get; set; }

        private bool IsTypeOfGoal { get; set; }

        private bool IsOwnGoal { get; set; }
    }

    
}