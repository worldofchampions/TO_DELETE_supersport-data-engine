using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse
{
    public class Season
    {
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
    }

    public class SubLeague
    {
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public Season season { get; set; }
    }

    public class Tournament
    {
        public int tournamentId { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class League
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public SubLeague subLeague { get; set; }
        public List<Tournament> tournaments { get; set; }
    }

    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public League league { get; set; }
    }

    public class TennisLeagueTournamentsResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
