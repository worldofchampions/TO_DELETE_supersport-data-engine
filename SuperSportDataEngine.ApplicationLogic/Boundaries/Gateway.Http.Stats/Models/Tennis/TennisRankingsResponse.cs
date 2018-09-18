using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse
{
    public class DateThrough
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
        public string dateType { get; set; }
    }

    public class Player
    {
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
    }

    public class Ranking
    {
        public int rank { get; set; }
        public int rankTiedWith { get; set; }
        public string stat { get; set; }
        public Player player { get; set; }
    }

    public class LeaderCategory
    {
        public int leaderCategoryId { get; set; }
        public string name { get; set; }
        public string heading { get; set; }
        public DateThrough dateThrough { get; set; }
        public List<Ranking> ranking { get; set; }
    }

    public class Season
    {
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public List<LeaderCategory> leaderCategory { get; set; }
    }

    public class SubLeague
    {
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public List<Season> seasons { get; set; }
    }

    public class League
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public SubLeague subLeague { get; set; }
    }

    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public League league { get; set; }
    }

    public class TennisRankingsResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
