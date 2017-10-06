using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.MongoRugbyMatchStats
{
    public class GameInfo
    {
        [BsonElement("assistant_referee_id")]
        public int assistantReferee1Id { get; set; }
        [BsonElement("number_of_periods")]
        public int numberOfPeriods { get; set; }
        [BsonElement("assistant_referee_1_Name")]
        public string assistantReferee1Name { get; set; }
        [BsonElement("assistant_referee_2_id")]
        public int assistantReferee2Id { get; set; }
        [BsonElement("assistant_referee_2_name")]
        public string assistantReferee2Name { get; set; }
        [BsonElement("game_date")]
        public string gameDate { get; set; }
        [BsonElement("game_number")]
        public int gameNumber { get; set; }
        [BsonElement("game_seconds")]
        public int gameSeconds { get; set; }
        [BsonElement("game_minutes")]
        public string gameMinutes { get; set; }
        [BsonElement("game_time")]
        public string gameTime { get; set; }
        [BsonElement("ground_conditions_id")]
        public int groundConditionId { get; set; }
        [BsonElement("ground_conditions_name")]
        public string groundConditionName { get; set; }
        [BsonElement("referee_1_id")]
        public int referee1Id { get; set; }
        [BsonElement("referee_1_name")]
        public string referee1Name { get; set; }
        [BsonElement("start_time")]
        public DateTime startTime { get; set; }
        [BsonElement("start_time_utc")]
        public DateTime startTimeUTC { get; set; }
        [BsonElement("venue_id")]
        public int venueId { get; set; }
        [BsonElement("venue_name")]
        public string venueName { get; set; }
        [BsonElement("weather_id")]
        public int weatherId { get; set; }
        [BsonElement("weather_name")]
        public string weatherName { get; set; }
    }

    public class Score
    {
        [BsonElement("conversions")]
        public int conversions { get; set; }
        [BsonElement("points")]
        public int points { get; set; }
        [BsonElement("field_goals")]
        public int fieldGoals { get; set; }
        [BsonElement("penalty_goals")]
        public int penaltyGoals { get; set; }
        [BsonElement("tries")]
        public int tries { get; set; }
    }

    public class InOut
    {
        [BsonElement("in_out")]
        public string inOut { get; set; }
        [BsonElement("player_first_name")]
        public string playerFirstName { get; set; }
        [BsonElement("player_id")]
        public string playerId { get; set; }
        [BsonElement("player_last_name")]
        public string playerLastName { get; set; }
        [BsonElement("player_name")]
        public string playerName { get; set; }
    }

    public class InsAndOuts
    {
        [BsonElement("ins_and_outs")]
        public IList<InOut> inOut { get; set; }
    }

    public class PeriodStat
    {
        [BsonElement("stat_type_id")]
        public int StatTypeID { get; set; }
        [BsonElement("stat_value")]
        public double StatValue { get; set; }
        [BsonElement("stat_name")]
        public string StatName { get; set; }
        [BsonElement("stat_name_int")]
        public string StatNameInt { get; set; }
    }

    public class Period
    {
        [BsonElement("period_id")]
        public int periodId { get; set; }
        [BsonElement("period_stat")]
        public IList<PeriodStat> periodStat { get; set; }
    }

    public class PeriodStats
    {
        [BsonElement("period")]
        public IList<Period> period { get; set; }
    }

    public class MatchStat
    {
        [BsonElement("stat_type_id")]
        public int StatTypeID { get; set; }
        [BsonElement("stat_value")]
        public double StatValue { get; set; }
        [BsonElement("stat_name")]
        public string StatName { get; set; }
        [BsonElement("stat_name_int")]
        public string StatNameInt { get; set; }
    }

    public class MatchStats
    {
        [BsonElement("match_stat")]
        public IList<MatchStat> matchStat { get; set; }
    }

    public class PlayerStats
    {
        [BsonElement("period_stats")]
        public PeriodStats periodStats { get; set; }
        [BsonElement("match_stats")]
        public MatchStats matchStats { get; set; }
    }

    public class TeamPlayer
    {
        [BsonElement("player_first_name")]
        public string playerFirstName { get; set; }
        [BsonElement("player_id")]
        public int playerId { get; set; }
        [BsonElement("player_last_name")]
        public string playerLastName { get; set; }
        [BsonElement("player_name")]
        public string playerName { get; set; }
        [BsonElement("player_position")]
        public string playerPosition { get; set; }
        [BsonElement("player_position_id")]
        public int playerPositionId { get; set; }
        [BsonElement("player_stats")]
        public PlayerStats playerStats { get; set; }
        [BsonElement("player_took_the_field")]
        public bool playerTookTheField { get; set; }
        [BsonElement("shirt_num")]
        public int shirtNum { get; set; }
        [BsonElement("is_captain")]
        public bool? isCaptain { get; set; }
    }

    public class TeamLineup
    {
        [BsonElement("ins_and_outs")]
        public InsAndOuts insAndOuts { get; set; }
        [BsonElement("team_player")]
        public IList<TeamPlayer> teamPlayer { get; set; }
    }

    public class TeamStats
    {
        [BsonElement("period_stats")]
        public PeriodStats periodStats { get; set; }
        [BsonElement("match_stats")]
        public MatchStats matchStats { get; set; }
    }

        public class TeamsMatch
    {
        [BsonElement("is_home_team")]
        public bool isHomeTeam { get; set; }
        [BsonElement("score")]
        public Score score { get; set; }
        [BsonElement("team_1st_half_score")]
        public int team1stHalfScore { get; set; }
        [BsonElement("team_2nd_half_score")]
        public int team2ndHalfScore { get; set; }
        [BsonElement("team_abbr")]
        public string teamAbbr { get; set; }
        [BsonElement("team_final_score")]
        public int teamFinalScore { get; set; }
        [BsonElement("team_full_time_score")]
        public int teamFullTimeScore { get; set; }
        [BsonElement("team_half_time_score")]
        public int teamHalfTimeScore { get; set; }
        [BsonElement("team_id")]
        public int teamId { get; set; }
        [BsonElement("team_lineup")]
        public TeamLineup teamLineup { get; set; }
        [BsonElement("team_name")]
        public string teamName { get; set; }
        [BsonElement("team_nick_name")]
        public string teamNickName { get; set; }
        [BsonElement("team_stats")]
        public TeamStats teamStats { get; set; }
    }

    public class Teams
    {
        [BsonElement("teams_match")]
        public IList<TeamsMatch> teamsMatch { get; set; }
    }

    public class MongoRugbyMatchStats
    {
        [BsonElement("request_time")]
        public DateTime RequestTime { get; set; }
        [BsonElement("response_time")]
        public DateTime ResponseTime { get; set; }
        [BsonElement("competition_id")]
        public int competitionId { get; set; }
        [BsonElement("competition_name")]
        public string competitionName { get; set; }
        [BsonElement("game_id")]
        public long gameId { get; set; }
        [BsonElement("game_info")]
        public GameInfo gameInfo { get; set; }
        [BsonElement("game_state")]
        public string gameState { get; set; }
        [BsonElement("game_state_id")]
        public int gameStateId { get; set; }
        [BsonElement("round_id")]
        public int roundId { get; set; }
        [BsonElement("season_id")]
        public int seasonId { get; set; }
        [BsonElement("teams")]
        public Teams teams { get; set; }
    }
}
