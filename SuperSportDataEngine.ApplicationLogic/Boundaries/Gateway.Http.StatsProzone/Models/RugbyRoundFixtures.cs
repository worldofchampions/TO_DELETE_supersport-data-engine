using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models
{
    public class Team
    {
        public string team { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamNickName { get; set; }
        public string teamAbbr { get; set; }
        public int teamPosition { get; set; }
        public int teamFinalScore { get; set; }
        public bool isHomeTeam { get; set; }
    }

    public class GameFixture
    {
        public int group { get; set; }
        public string groupName { get; set; }
        public List<Team> teams { get; set; }
        public string game { get; set; }
        public object gameId { get; set; }
        public int gameStateId { get; set; }
        public string gameStateName { get; set; }
        public int gameNumber { get; set; }
        public object gameHashTag { get; set; }
        public DateTime startTime { get; set; }
        public DateTime startTimeUTC { get; set; }
        public int venueId { get; set; }
        public string venueName { get; set; }
        public string venueTimezone { get; set; }
        public object venueLatitude { get; set; }
        public object venueLongitude { get; set; }
        public int groundConditionId { get; set; }
        public object groundConditionName { get; set; }
        public int weatherId { get; set; }
        public object weatherName { get; set; }
        public object broadcastChannel { get; set; }
        public DateTime broadcastStartTimeUtc { get; set; }
        public DateTime broadcastStartTime { get; set; }
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
        public DateTime roundStartDate { get; set; }
        public DateTime roundFinishDate { get; set; }
        public string roundAbbreviation { get; set; }
    }

    public class RugbyRoundFixtures
    {
        public List<RoundFixture> roundFixtures { get; set; }
        public int seasonId { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public DateTime seasonStartDate { get; set; }
        public DateTime seasonFinishDate { get; set; }
    }
}
