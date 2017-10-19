using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow
{
    public class StatScoringEvent
    {
        public string gameState { get; set; }
        public int gameStateId { get; set; }
        public string gameTime { get; set; }
        public int gameSeconds { get; set; }
        public string GameMinutes { get; set; }
        public int playerId { get; set; }
        public string playerName { get; set; }
        public string scoreMethod { get; set; }
        public string statEvent { get; set; }
        public int statId { get; set; }
        public double statValue { get; set; }
        public int vidRef { get; set; }
        public int homeScore { get; set; }
        public int phaseNum { get; set; }
        public int? awayScore { get; set; }
    }

    public class Team2
    {
        public int conversions { get; set; }
        public int fieldGoals { get; set; }
        public bool isHomeTeam { get; set; }
        public int penaltyGoals { get; set; }
        public int points { get; set; }
        public List<StatScoringEvent> statScoringEvent { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamAbbrv { get; set; }
        public int tries { get; set; }
        public int period1Tries { get; set; }
        public int period2Tries { get; set; }
        public int period2Conversions { get; set; }
        public int period1PenaltyGoals { get; set; }
        public int period2PenaltyGoals { get; set; }
    }

    public class Team
    {
        public List<Team2> team { get; set; }
    }

    public class ScoreEvent
    {
        public List<Team> teams { get; set; }
    }

    public class ScoreFlow
    {
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

    public class RugbyEventsFlow
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
