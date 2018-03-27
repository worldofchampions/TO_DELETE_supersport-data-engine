using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogsGrouped
{
    public class RecentFormList
    {
        [BsonElement("recent_form_1")]
        public string recentForm1 { get; set; }
        [BsonElement("recent_form_2")]
        public string recentForm2 { get; set; }
        [BsonElement("recent_form_3")]
        public string recentForm3 { get; set; }
        [BsonElement("recent_form_4")]
        public string recentForm4 { get; set; }
        [BsonElement("recent_form_5")]
        public string recentForm5 { get; set; }
    }

    public class Ladderposition
    {
        [BsonElement("position")]
        public int position { get; set; }
        [BsonElement("team_id")]
        public int teamId { get; set; }
        [BsonElement("team_name")]
        public string teamName { get; set; }
        [BsonElement("games_played")]
        public int gamesPlayed { get; set; }
        [BsonElement("wins")]
        public int wins { get; set; }
        [BsonElement("losses")]
        public int losses { get; set; }
        [BsonElement("competition_points")]
        public int competitionPoints { get; set; }
        [BsonElement("points_for")]
        public int pointsFor { get; set; }
        [BsonElement("points_against")]
        public int pointsAgainst { get; set; }
        [BsonElement("points_difference")]
        public int pointsDifference { get; set; }
        [BsonElement("home_wins")]
        public int homeWins { get; set; }
        [BsonElement("home_losses")]
        public int homeLosses { get; set; }
        [BsonElement("away_wins")]
        public int awayWins { get; set; }
        [BsonElement("away_losses")]
        public int awayLosses { get; set; }
        [BsonElement("recent_form_list")]
        public RecentFormList recentFormList { get; set; }
        [BsonElement("recent_form")]
        public string recentForm { get; set; }
        [BsonElement("season_form")]
        public string seasonForm { get; set; }
        [BsonElement("group")]
        public int? group { get; set; }
        [BsonElement("group_name")]
        public string groupName { get; set; }
        [BsonElement("tries_for")]
        public int triesFor { get; set; }
        [BsonElement("tries_conceded")]
        public int triesConceded { get; set; }
        [BsonElement("tries_against")]
        public int triesAgainst { get; set; }
        [BsonElement("goals_for")]
        public int goalsFor { get; set; }
        [BsonElement("goals_conceded")]
        public int goalsConceded { get; set; }
        [BsonElement("bonus_points")]
        public int bonusPoints { get; set; }
        [BsonElement("overall_position")]
        public int overallPosition { get; set; }
        [BsonElement("field_goals_conceded")]
        public int? fieldGoalsConceded { get; set; }
        [BsonElement("draws")]
        public int? draws { get; set; }
        [BsonElement("away_draws")]
        public int? awayDraws { get; set; }
        [BsonElement("field_goals_for")]
        public int? fieldGoalsFor { get; set; }
        [BsonElement("home_draws")]
        public int? homeDraws { get; set; }
    }

    public class OverallStandings
    {
        [BsonElement("ladder_position")]
        public List<Ladderposition> ladderposition { get; set; }
    }

    public class GroupStandings
    {
        [BsonElement("ladder_position")]
        public List<Ladderposition> ladderposition { get; set; }
    }

    public class SecondaryGroupStandings
    {
        [BsonElement("ladder_position")]
        public List<Ladderposition> ladderposition { get; set; }
    }

    public class MongoRugbyGroupedLogs
    {
        [BsonElement("request_time")]
        public DateTime RequestTime { get; set; }
        [BsonElement("response_time")]
        public DateTime ResponseTime { get; set; }
        [BsonElement("competition_id")]
        public int competitionId { get; set; }
        [BsonElement("competition_name")]
        public string competitionName { get; set; }
        [BsonElement("round_number")]
        public int roundNumber { get; set; }
        [BsonElement("overall_standings")]
        public OverallStandings overallStandings { get; set; }
        [BsonElement("group_standings")]
        public GroupStandings groupStandings { get; set; }
        [BsonElement("secondary_group_standings")]
        public SecondaryGroupStandings secondaryGroupStandings { get; set; }
        [BsonElement("ladder_positions")]
        public List<Ladderposition> ladderposition { get; set; }

        [BsonElement("season_id")]
        public int seasonId { get; set; }
    }
}
