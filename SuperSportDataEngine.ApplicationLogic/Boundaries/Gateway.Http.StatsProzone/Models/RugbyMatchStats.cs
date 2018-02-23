using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats
{
    public class GameInfo
    {
        public int assistantReferee1Id { get; set; }
        public int numberOfPeriods { get; set; }
        public string assistantReferee1Name { get; set; }
        public int assistantReferee2Id { get; set; }
        public string assistantReferee2Name { get; set; }
        public string gameDate { get; set; }
        public int gameNumber { get; set; }
        public int gameSeconds { get; set; }
        public string gameMinutes { get; set; }
        public string gameTime { get; set; }
        public int groundConditionId { get; set; }
        public string groundConditionName { get; set; }
        public int referee1Id { get; set; }
        public string referee1Name { get; set; }
        public DateTime startTime { get; set; }
        public DateTime startTimeUTC { get; set; }
        public int venueId { get; set; }
        public string venueName { get; set; }
        public int weatherId { get; set; }
        public string weatherName { get; set; }
    }

    public class Score
    {
        public int conversions { get; set; }
        public int points { get; set; }
        public int fieldGoals { get; set; }
        public int penaltyGoals { get; set; }
        public int tries { get; set; }
    }

    public class InOut
    {
        public string inOut { get; set; }
        public string playerFirstName { get; set; }
        public string playerId { get; set; }
        public string playerLastName { get; set; }
        public string playerName { get; set; }
    }

    public class InsAndOuts
    {
        public IList<InOut> inOut { get; set; }
    }

    public class PeriodStat
    {
        public int StatTypeID { get; set; }
        public double StatValue { get; set; }
        public string StatName { get; set; }
        public string StatNameInt { get; set; }
    }

    public class Period
    {
        public int periodId { get; set; }
        public IList<PeriodStat> periodStat { get; set; }
    }

    public class PeriodStats
    {
        public IList<Period> period { get; set; }
    }

    public class MatchStat
    {
        public int StatTypeID { get; set; }
        public double StatValue { get; set; }
        public string StatName { get; set; }
        public string StatNameInt { get; set; }
    }

    public class MatchStats
    {
        public IList<MatchStat> matchStat { get; set; }
    }

    public class PlayerStats
    {
        public PeriodStats periodStats { get; set; }
        public MatchStats matchStats { get; set; }
    }

    public class TeamPlayerComparer : IEqualityComparer<TeamPlayer>
    {
        public bool Equals(TeamPlayer x, TeamPlayer y)
        {
            return x?.playerId == y?.playerId;
        }

        public int GetHashCode(TeamPlayer obj)
        {
            return obj.playerId;
        }
    }

    public class TeamPlayer
    {
        public string playerFirstName { get; set; }
        public int playerId { get; set; }
        public string playerLastName { get; set; }
        public string playerName { get; set; }
        public string playerPosition { get; set; }
        public int playerPositionId { get; set; }
        public PlayerStats playerStats { get; set; }
        public bool playerTookTheField { get; set; }
        public int shirtNum { get; set; }
        public bool? isCaptain { get; set; }
    }

    public class TeamLineup
    {
        public InsAndOuts insAndOuts { get; set; }
        public IList<TeamPlayer> teamPlayer { get; set; }
    }

    public class TeamStats
    {
        public PeriodStats periodStats { get; set; }
        public MatchStats matchStats { get; set; }
    }

        public class TeamsMatch
    {
        public bool isHomeTeam { get; set; }
        public Score score { get; set; }
        public int team1stHalfScore { get; set; }
        public int team2ndHalfScore { get; set; }
        public string teamAbbr { get; set; }
        public int teamFinalScore { get; set; }
        public int teamFullTimeScore { get; set; }
        public int teamHalfTimeScore { get; set; }
        public int teamId { get; set; }
        public TeamLineup teamLineup { get; set; }
        public string teamName { get; set; }
        public string teamNickName { get; set; }
        public TeamStats teamStats { get; set; }
    }

    public class Teams
    {
        public IList<TeamsMatch> teamsMatch { get; set; }
    }

    public class RugbyMatchStats
    {
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public long gameId { get; set; }
        public GameInfo gameInfo { get; set; }
        public string gameState { get; set; }
        public int gameStateId { get; set; }
        public int roundId { get; set; }
        public int seasonId { get; set; }
        public Teams teams { get; set; }
    }
}
