using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
using System;
using System.Collections.Generic;
using System.Timers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public abstract class MatchDetailsModel
    {
      
        public string LeagueName { get; set; }
        public int LeagueId { get; set; }
        public string LeagueUrlName { get; set; }
        public int MatchID { get; set; }
        public string TeamAName { get; set; }
        public string TeamBName { get; set; }
        public string TeamAShortName { get; set; }
        public string TeamBShortName { get; set; }
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public DateTime KickoffDateTime { get; set; }
        public string KickoffDateTimeString { get; set; }
        public string Location { get; set; }
        public string Preview { get; set; }
        public string MatchType { get; set; }
        public string MatchTypeAbbr { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public bool MatchCompleted { get; set; }
        public List<string> Channels { get; set; }
        public List<ChannelRegions> channelRegions { get; set; }

        public virtual int TeamAScore { get; set; }
        public virtual int TeamBScore { get; set; }
        public int TeamAHalfTimeScore { get; set; }
        public int TeamBHalfTimeScore { get; set; }

        public int RoundNumber { get; set; }
        public int MatchNumber { get; set; }
        public string Attendance { get; set; }
        public int ReportId { get; set; }

        public string ManOfTheMatch { get; set; }
        public int ManOfTheMatchId { get; set; }

        public string Group { get; set; }

        public List<MatchEventModel> Events { get; set; }

        public virtual List<PlayerModel> Teamsheet { get; set; }
        public virtual List<PlayerModel> TeamATeamsheet { get; set; }
        public virtual List<PlayerModel> TeamBTeamsheet { get; set; }

        public virtual List<ScorerModel> TeamAScorers { get; set; }
        public virtual List<ScorerModel> TeamBScorers { get; set; }

        public virtual List<SubstituteModel> TeamASubstitutes { get; set; }
        public virtual List<SubstituteModel> TeamBSubstitutes { get; set; }

        public virtual List<CardsModel> TeamACards { get; set; }
        public virtual List<CardsModel> TeamBCards { get; set; }

        //public virtual List<Management> TeamAManagement { get; set; }
        //public virtual List<Management> TeamBManagement { get; set; }

        public List<OfficialModel> Officials { get; set; }
        public List<MatchVideoModel> Videos { get; set; }
        public List<MatchLiveVideoModel> LiveVideos { get; set; }
        public List<MatchEventModel> Commentary { get; set; }
        public Boolean Postponed { get; set; }
        //TODO: GET a serializer or formatter for this field
        public DateTimeOffset MatchTime { get; set; }
    }
}