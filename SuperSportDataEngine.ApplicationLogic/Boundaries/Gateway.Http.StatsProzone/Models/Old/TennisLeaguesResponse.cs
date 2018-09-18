using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Tennis.TennisLeaguesResponse
{
    public class UriPath
    {
        public int pathSequence { get; set; }
        public string path { get; set; }
    }

    public class SubLeague
    {
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
    }

    public class UriPath2
    {
        public int pathSequence { get; set; }
        public string path { get; set; }
    }

    public class League2
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public SubLeague subLeague { get; set; }
        public List<UriPath2> uriPaths { get; set; }
    }

    public class League
    {
        public League2 league { get; set; }
    }

    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public List<UriPath> uriPaths { get; set; }
        public List<League> leagues { get; set; }
    }

    public class TennisLeaguesResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
