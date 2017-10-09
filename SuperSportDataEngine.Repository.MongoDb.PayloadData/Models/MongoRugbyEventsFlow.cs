using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.MongoRugbyEventsFlow
{
    public class StatScoringEvent
    {
        [BsonElement("game_state")]
        public string gameState { get; set; }
        [BsonElement("game_state_id")]
        public int gameStateId { get; set; }
        [BsonElement("game_time")]
        public string gameTime { get; set; }
        [BsonElement("game_seconds")]
        public int gameSeconds { get; set; }
        [BsonElement("game_minutes")]
        public string GameMinutes { get; set; }
        [BsonElement("player_id")]
        public int playerId { get; set; }
        [BsonElement("player_name")]
        public string playerName { get; set; }
        [BsonElement("score_method")]
        public string scoreMethod { get; set; }
        [BsonElement("stat_event")]
        public string statEvent { get; set; }
        [BsonElement("stat_id")]
        public int statId { get; set; }
        [BsonElement("stat_value")]
        public double statValue { get; set; }
        [BsonElement("vid_ref")]
        public int vidRef { get; set; }
        [BsonElement("home_score")]
        public int homeScore { get; set; }
        [BsonElement("phase_num")]
        public int phaseNum { get; set; }
        [BsonElement("away_score")]
        public int? awayScore { get; set; }
    }

    public class Team2
    {
        [BsonElement("conversions")]
        public int conversions { get; set; }
        [BsonElement("field_goals")]
        public int fieldGoals { get; set; }
        [BsonElement("is_home_team")]
        public bool isHomeTeam { get; set; }
        [BsonElement("penalty_goals")]
        public int penaltyGoals { get; set; }
        [BsonElement("points")]
        public int points { get; set; }
        [BsonElement("stat_scoring_event")]
        public List<StatScoringEvent> statScoringEvent { get; set; }
        [BsonElement("team_id")]
        public int teamId { get; set; }
        [BsonElement("team_name")]
        public string teamName { get; set; }
        [BsonElement("team_abbrv")]
        public string teamAbbrv { get; set; }
        [BsonElement("tries")]
        public int tries { get; set; }
        [BsonElement("period_1_tries")]
        public int period1Tries { get; set; }
        [BsonElement("period_2_tries")]
        public int period2Tries { get; set; }
        [BsonElement("period_2_conversions")]
        public int period2Conversions { get; set; }
        [BsonElement("period_1_penalty_goals")]
        public int period1PenaltyGoals { get; set; }
        [BsonElement("period_2_penalty_goals")]
        public int period2PenaltyGoals { get; set; }
    }

    public class Team
    {
        [BsonElement("team")]
        public List<Team2> team { get; set; }
    }

    public class ScoreEvent
    {
        [BsonElement("teams")]
        public List<Team> teams { get; set; }
    }

    public class ScoreFlow
    {
        [BsonElement("score_event")]
        public ScoreEvent scoreEvent { get; set; }
    }

    public class StatPenaltyEvent
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public int statId { get; set; }
        public int statValue { get; set; }
        public int gameSeconds { get; set; }
        public string gameState { get; set; }
        public int gameStateId { get; set; }
        public string penaltyDescription { get; set; }
        public int vidRef { get; set; }
        public string commentary { get; set; }
        public bool? isHomeTeam { get; set; }
    }

    public class PenaltyEvent
    {
        public List<StatPenaltyEvent> statPenaltyEvent { get; set; }
    }

    public class PenaltyFlow
    {
        public PenaltyEvent penaltyEvent { get; set; }
    }

    public class StatErrorEvent
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public int playerId { get; set; }
        public object playerName { get; set; }
        public bool isHomeTeam { get; set; }
        public int statId { get; set; }
        public int statValue { get; set; }
        public string gameTime { get; set; }
        public int gameSeconds { get; set; }
        public string GameMinutes { get; set; }
        public string gameState { get; set; }
        public int gameStateId { get; set; }
        public int vidRef { get; set; }
        public string commentary { get; set; }
        public int phaseNum { get; set; }
    }

    public class ErrorEvent
    {
        public List<StatErrorEvent> statErrorEvent { get; set; }
    }

    public class ErrorFlow
    {
        public ErrorEvent errorEvent { get; set; }
    }

    public class InterchangeFlow
    {
    }

    public class CommentaryEvent
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public int playerId { get; set; }
        public object playerName { get; set; }
        public bool isHomeTeam { get; set; }
        public int statId { get; set; }
        public string gameTime { get; set; }
        public int gameSeconds { get; set; }
        public string GameMinutes { get; set; }
        public string commentary { get; set; }
        public int? homeScore { get; set; }
        public int? awayScore { get; set; }
    }

    public class CommentaryFlow
    {
        public List<CommentaryEvent> commentaryEvent { get; set; }
    }

    public class MongoRugbyEventsFlow
    {
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public long gameId { get; set; }
        public int gameStateId { get; set; }
        public string gameStateDesc { get; set; }
        public int roundId { get; set; }
        public ScoreFlow scoreFlow { get; set; }
        public PenaltyFlow penaltyFlow { get; set; }
        public ErrorFlow errorFlow { get; set; }
        public InterchangeFlow interchangeFlow { get; set; }
        public CommentaryFlow commentaryFlow { get; set; }
        public int seasonId { get; set; }
    }
}
