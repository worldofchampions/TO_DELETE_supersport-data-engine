using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyFixtures
{
    public class Team
    {
        [BsonElement("team")]
        public string team { get; set; }
        [BsonElement("team_id")]
        public int teamId { get; set; }
        [BsonElement("team_name")]
        public string teamName { get; set; }
        [BsonElement("team_nickname")]
        public string teamNickName { get; set; }
        [BsonElement("team_abbrev")]
        public string teamAbbr { get; set; }
        [BsonElement("team_final_score")]
        public int teamFinalScore { get; set; }
        [BsonElement("is_home_team")]
        public bool isHomeTeam { get; set; }
    }

    public class GameFixture
    {
        [BsonElement("teams")]
        public List<Team> teams { get; set; }
        [BsonElement("game")]
        public string game { get; set; }
        [BsonElement("game_id")]
        public object gameId { get; set; }
        [BsonElement("game_stat_id")]
        public int gameStateId { get; set; }
        [BsonElement("gme_state_id")]
        public string gameStateName { get; set; }
        [BsonElement("game_number")]
        public int gameNumber { get; set; }
        [BsonElement("start_time")]
        public string startTime { get; set; }
        [BsonElement("start_time_utc")]
        public string startTimeUTC { get; set; }
        [BsonElement("venue_id")]
        public int venueId { get; set; }
        [BsonElement("venue_name")]
        public string venueName { get; set; }
        [BsonElement("venue_timezone")]
        public string venueTimezone { get; set; }
        [BsonElement("ground_condition_id")]
        public int groundConditionId { get; set; }
        [BsonElement("ground_condition_name")]
        public object groundConditionName { get; set; }
        [BsonElement("weather_name")]
        public object weatherName { get; set; }
        [BsonElement("broadcast_channel")]
        public object broadcastChannel { get; set; }
        [BsonElement("verbose_date")]
        public string verboseDate { get; set; }
        [BsonElement("game_seconds")]
        public int gameSeconds { get; set; }
        [BsonElement("game_minutes")]
        public string gameMinutes { get; set; }
        [BsonElement("game_time")]
        public string gameTime { get; set; }
    }

    public class RoundFixture
    {
        [BsonElement("game_fixtures")]
        public List<GameFixture> gameFixtures { get; set; }
        [BsonElement("team_byes")]
        public List<object> teamByes { get; set; }
        [BsonElement("round_id")]
        public int roundId { get; set; }
        [BsonElement("round_name")]
        public string roundName { get; set; }
        [BsonElement("round_start_date")]
        public string roundStartDate { get; set; }
        [BsonElement("round_finish_date")]
        public string roundFinishDate { get; set; }
        [BsonElement("round_abbrev")]
        public string roundAbbreviation { get; set; }
    }

    public class MongoRugbyFixtures
    {
        [BsonElement("provider_request_time")]
        public DateTime RequestTime { get; set; }
        [BsonElement("provider_response_time")]
        public DateTime ResponseTime { get; set; }
        [BsonElement("round_fixtures")]
        public List<RoundFixture> roundFixtures { get; set; }
        [BsonElement("season_id")]
        public int seasonId { get; set; }
        [BsonElement("competition_id")]
        public int competitionId { get; set; }
        [BsonElement("competition_name")]
        public string competitionName { get; set; }
        [BsonElement("season_start_date")]
        public string seasonStartDate { get; set; }
        [BsonElement("season_finish_date")]
        public string seasonFinishDate { get; set; }
    }
}
