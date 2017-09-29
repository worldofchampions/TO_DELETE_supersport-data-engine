using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models
{
    public class GameInfo
    {
        public int assistantReferee1Id { get; set; }
        public string assistantReferee1Name { get; set; }
        public int assistantReferee2Id { get; set; }
        public string assistantReferee2Name { get; set; }
        public string gameDate { get; set; }
        public int gameNumber { get; set; }
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
        public List<InOut> inOut { get; set; }
    }

    public class PeriodStats
    {
        public List<object> period { get; set; }
    }

    public class MatchStats
    {
        public List<object> matchStat { get; set; }
    }

    public class PlayerStats
    {
        public PeriodStats periodStats { get; set; }
        public MatchStats matchStats { get; set; }
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
        public int shirtNum { get; set; }
        public bool? isCaptain { get; set; }
    }

    public class TeamLineup
    {
        public InsAndOuts insAndOuts { get; set; }
        public List<TeamPlayer> teamPlayer { get; set; }
    }

    public class PeriodStats2
    {
        public List<object> period { get; set; }
    }

    public class MatchStats2
    {
        public List<object> matchStat { get; set; }
    }

    public class TeamStats
    {
        public PeriodStats2 periodStats { get; set; }
        public MatchStats2 matchStats { get; set; }
    }

    public class TeamsMatch
    {
        public bool isHomeTeam { get; set; }
        public Score score { get; set; }
        public string teamAbbr { get; set; }
        public int teamId { get; set; }
        public TeamLineup teamLineup { get; set; }
        public string teamName { get; set; }
        public string teamNickName { get; set; }
        public TeamStats teamStats { get; set; }
    }

    public class Teams
    {
        public List<TeamsMatch> teamsMatch { get; set; }
    }

    public class RugbyMatchStats
    {
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
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
