using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyEntities
{
    public class GroundCondition
    {
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Competition
    {
        [BsonElement("competition_logo_url")]
        public string CompetitionLogoURL { get; set; }
        [BsonElement("competition_abbrev")]
        public string CompetitionAbbrev { get; set; }
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Player
    {
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Position
    {
        [BsonElement("name_int")]
        public string nameInt { get; set; }
        [BsonElement("id")]
        public int? id { get; set; }
    }

    public class Round
    {
        [BsonElement("round_number")]
        public int roundNumber { get; set; }
        [BsonElement("competition_id")]
        public int competitionId { get; set; }
        [BsonElement("competition_name")]
        public string competitionName { get; set; }
        [BsonElement("season_id")]
        public int seasonId { get; set; }
        [BsonElement("round_name")]
        public string roundName { get; set; }
        [BsonElement("is_active")]
        public bool isActive { get; set; }
        [BsonElement("id")]
        public int id { get; set; }
    }

    public class ScoringMethod
    {
        [BsonElement("stat_type_id")]
        public string statTypeId { get; set; }
        [BsonElement("scoring_value")]
        public int scoringValue { get; set; }
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Season
    {
        [BsonElement("is_active")]
        public bool isActive { get; set; }
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Statistic
    {
        [BsonElement("statistic")]
        public string statNameInt { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("name")]
        public int? id { get; set; }
    }

    public class Team
    {
        [BsonElement("team_logo_url")]
        public string TeamLogoURL { get; set; }
        [BsonElement("team_abbrev")]
        public string TeamAbbrev { get; set; }
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Venue
    {
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class WeatherCondition
    {
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class GameState
    {
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Official
    {
        [BsonElement("headshot_url")]
        public string headshotUrl { get; set; }
        [BsonElement("id")]
        public int id { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class MongoRugbyEntities
    {
        [BsonElement("provider_request_time")]
        public DateTime RequestTime { get; set; }
        [BsonElement("provider_response_time")]
        public DateTime ResponseTime { get; set; }
        [BsonElement("ground_conditions")]
        public List<GroundCondition> groundConditions { get; set; }
        [BsonElement("fantasy_rules")]
        public List<object> fantasyRules { get; set; }
        [BsonElement("competitions")]
        public List<Competition> competitions { get; set; }
        [BsonElement("players")]
        public List<Player> players { get; set; }
        [BsonElement("positions")]
        public List<Position> positions { get; set; }
        [BsonElement("rounds")]
        public List<Round> rounds { get; set; }
        [BsonElement("scoring_methods")]
        public List<ScoringMethod> scoringMethods { get; set; }
        [BsonElement("seasons")]
        public List<Season> seasons { get; set; }
        [BsonElement("statistics")]
        public List<Statistic> statistics { get; set; }
        [BsonElement("teams")]
        public List<Team> teams { get; set; }
        [BsonElement("venues")]
        public List<Venue> venues { get; set; }
        [BsonElement("weather_conditions")]
        public List<WeatherCondition> weatherConditions { get; set; }
        [BsonElement("game_states")]
        public List<GameState> gameStates { get; set; }
        [BsonElement("officials")]
        public List<Official> officials { get; set; }
    }
}
