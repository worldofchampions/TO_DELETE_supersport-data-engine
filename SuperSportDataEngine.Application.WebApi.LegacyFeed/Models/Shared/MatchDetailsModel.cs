namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public abstract class MatchDetailsModel
    {
        public string LeagueName { get; set; }

        public int LeagueId { get; set; }

        public string LeagueUrlName { get; set; }

        public int MatchID { get; set; }

        public bool IsPlaceholderTeamA { get; set; }

        public bool IsPlaceholderTeamB { get; set; }

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

        public int PreviewId { get; set; }

        public string MatchType { get; set; }

        public string MatchTypeAbbr { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }

        public bool MatchCompleted { get; set; }

        public List<string> Channels { get; set; }

        public List<ChannelRegions> channelRegions { get; set; }

        public virtual int? TeamAScore { get; set; }

        public virtual int? TeamBScore { get; set; }

        public int TeamAHalfTimeScore { get; set; }

        public int TeamBHalfTimeScore { get; set; }

        public int RoundNumber { get; set; }

        public string RoundName { get; set; }

        public int MatchNumber { get; set; }

        public string Attendance { get; set; }

        public int ReportId { get; set; }

        public int MatchDayBlogId { get; set; }

        public string ManOfTheMatch { get; set; }

        public int ManOfTheMatchId { get; set; }

        public string Group { get; set; }

        public List<MatchEvent> Events { get; set; }

        public virtual List<Player> Teamsheet { get; set; }

        public virtual List<Player> TeamATeamsheet { get; set; }

        public virtual List<Player> TeamBTeamsheet { get; set; }

        public virtual List<Scorer> TeamAScorers { get; set; }

        public virtual List<Scorer> TeamBScorers { get; set; }

        public virtual List<SubstituteModel> TeamASubstitutes { get; set; }

        public virtual List<SubstituteModel> TeamBSubstitutes { get; set; }

        public virtual List<CardsModel> TeamACards { get; set; }

        public virtual List<CardsModel> TeamBCards { get; set; }

        public List<OfficialModel> Officials { get; set; }

        public List<MatchVideoModel> Videos { get; set; }

        public List<MatchLiveVideoModel> LiveVideos { get; set; }

        public List<MatchEvent> Commentary { get; set; }

        public Boolean Postponed { get; set; }

        /// <summary>
        /// Time at which the match ends.
        /// </summary>
        public DateTimeOffset MatchTime { get; set; }
    }
}
