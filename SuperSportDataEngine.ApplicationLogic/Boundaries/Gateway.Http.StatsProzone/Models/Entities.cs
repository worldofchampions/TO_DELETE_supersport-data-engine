using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models
{
    public class GroundCondition
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Competition
    {
        public string CompetitionLogoURL { get; set; }
        public string CompetitionAbbrev { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Player
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Position
    {
        public string nameInt { get; set; }
        public int? id { get; set; }
    }

    public class Round
    {
        public int roundNumber { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public int seasonId { get; set; }
        public string roundName { get; set; }
        public bool isActive { get; set; }
        public int id { get; set; }
    }

    public class ScoringMethod
    {
        public string statTypeId { get; set; }
        public int scoringValue { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Season
    {
        public bool isActive { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Statistic
    {
        public string statNameInt { get; set; }
        public string name { get; set; }
        public int? id { get; set; }
    }

    public class Team
    {
        public string TeamLogoURL { get; set; }
        public string TeamAbbrev { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Venue
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class WeatherCondition
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class GameState
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Official
    {
        public string headshotUrl { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Entities
    {
        public List<GroundCondition> groundConditions { get; set; }
        public List<object> fantasyRules { get; set; }
        public List<Competition> competitions { get; set; }
        public List<Player> players { get; set; }
        public List<Position> positions { get; set; }
        public List<Round> rounds { get; set; }
        public List<ScoringMethod> scoringMethods { get; set; }
        public List<Season> seasons { get; set; }
        public List<Statistic> statistics { get; set; }
        public List<Team> teams { get; set; }
        public List<Venue> venues { get; set; }
        public List<WeatherCondition> weatherConditions { get; set; }
        public List<GameState> gameStates { get; set; }
        public List<Official> officials { get; set; }
    }
}
