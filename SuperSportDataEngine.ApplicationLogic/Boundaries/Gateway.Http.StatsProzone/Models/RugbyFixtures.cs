using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures
{
    public class Team
    {
        public string team { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamNickName { get; set; }
        public string teamAbbr { get; set; }
        public int? teamFinalScore { get; set; }
        public bool isHomeTeam { get; set; }
    }

    public class GameFixture
    {
        public List<Team> teams { get; set; }
        public string game { get; set; }
        public long gameId { get; set; }
        public int gameStateId { get; set; }
        public string gameStateName { get; set; }
        public int gameNumber { get; set; }
        public string startTime { get; set; }
        public string startTimeUTC { get; set; }
        public int? venueId { get; set; }
        public string venueName { get; set; }
        public string venueTimezone { get; set; }
        public int groundConditionId { get; set; }
        public object groundConditionName { get; set; }
        public object weatherName { get; set; }
        public object broadcastChannel { get; set; }
        public string verboseDate { get; set; }
        public int gameSeconds { get; set; }
        public string gameMinutes { get; set; }
        public string gameTime { get; set; }
    }

    public class RoundFixture
    {
        public List<GameFixture> gameFixtures { get; set; }
        public List<object> teamByes { get; set; }
        public int roundId { get; set; }
        public string roundName { get; set; }
        public string roundStartDate { get; set; }
        public string roundFinishDate { get; set; }
        public string roundAbbreviation { get; set; }
    }

    public class RugbyFixtures
    {
        public List<RoundFixture> roundFixtures { get; set; }
        public int seasonId { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public string seasonStartDate { get; set; }
        public string seasonFinishDate { get; set; }
    }
}
