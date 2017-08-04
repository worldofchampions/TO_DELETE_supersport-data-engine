using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Golf
{
    [Serializable]
    public class GolfMatchPlayLeaderboard
    {
        public GolfMatchPlayTeams Teams { get; set; }
        public List<GolfMatchPlayRound> Rounds { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayTeams
    {
        public GolfMatchPlayTeam Team1 { get; set; }
        public GolfMatchPlayTeam Team2 { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayTeam
    {
        public string Name { get; set; }
        public string Score { get; set; }
        public string ShortName { get; set; }
        public string Colour { get; set; }
        public string Logo { get; set; }
        public List<GolfMatchPlayTeamRound> Rounds { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayTeamRound
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Score { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayRound
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public List<GolfMatchPlayBracket> Brackets { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayBracket
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public List<GolfMatchPlayMatch> Matches { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayMatch
    {
        public int MatchNumber { get; set; }
        public DateTime TeeTime { get; set; }
        public int HolesPlayed { get; set; }
        public string Score { get; set; }
        public string MatchStatus { get; set; }
        public List<GolfMatchPlayPlayer> Team1 { get; set; }
        public List<GolfMatchPlayPlayer> Team2 { get; set; }
    }

    [Serializable]
    public class GolfMatchPlayPlayer
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Country { get; set; }
        public string Score { get; set; }
        public bool IsWinner { get; set; }
    }
}