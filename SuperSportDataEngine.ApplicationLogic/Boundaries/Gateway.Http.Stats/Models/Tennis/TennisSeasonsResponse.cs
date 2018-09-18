using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis
{
    public class SubLeague
    {
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
    }

    public class StartDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
        public string dateType { get; set; }
    }

    public class EndDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
        public string dateType { get; set; }
    }

    public class EventType
    {
        public int eventTypeId { get; set; }
        public string name { get; set; }
        public StartDate startDate { get; set; }
        public EndDate endDate { get; set; }
    }

    public class Season
    {
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public List<EventType> eventType { get; set; }
    }

    public class League
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public SubLeague subLeague { get; set; }
        public List<Season> seasons { get; set; }
    }

    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public League league { get; set; }
    }

    public class TennisSeasonsResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
