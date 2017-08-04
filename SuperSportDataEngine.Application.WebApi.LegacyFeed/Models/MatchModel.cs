using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class MatchModel
    {
        public string LeagueName { get; set; }
        public string LeagueShortName { get; set; }
        public string LeagueUrlName { get; set; }
        public int LeagueId { get; set; }
        public int MatchID { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeamShortName { get; set; }
        public string AwayTeamShortName { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public string HomeTeamScore { get; set; }
        public string AwayTeamScore { get; set; }
        public DateTime MatchDateTime { get; set; }
        public string MatchDateTimeString { get; set; }
        public DateTime MatchEndDateTime { get; set; }
        public string MatchEndDateTimeString { get; set; }
        public string Location { get; set; }
        public int Report { get; set; }
        public int HomeTeamPenalties { get; set; }
        public int AwayTeamPenalties { get; set; }
        public bool Postponed { get; set; }
        public bool WalkOver { get; set; }
        public string MatchNumber { get; set; }
        public string MatchComment { get; set; }
        public string MatchType { get; set; }
        public string MatchTypeAbbr { get; set; }
        public string Scorecard { get; set; }
        public string Status { get; set; }
        public string MatchResultShort { get; set; }
        public string MatchBreak { get; set; }
        public bool Result { get; set; }
        public List<string> Channels { get; set; }
        public List<ChannelRegions> channelRegions { get; set; }
        public bool IsFeatured { get; set; }
        public bool International { get; set; }
        public int Round { get; set; }
        public string RoundName { get; set; }
        public int Stage { get; set; }
        public string StageName { get; set; }
        public List<MatchVideoModel> Videos { get; set; }
        public List<MatchLiveVideoModel> LiveVideos { get; set; }
    }
}