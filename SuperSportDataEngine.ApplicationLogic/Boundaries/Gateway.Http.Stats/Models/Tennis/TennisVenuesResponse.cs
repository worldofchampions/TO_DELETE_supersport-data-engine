using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse
{
    public class Country
    {
        public int countryId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class Surface
    {
        public int surfaceId { get; set; }
        public string name { get; set; }
    }

    public class State
    {
        public int stateId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class Venue
    {
        public int venueId { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public Country country { get; set; }
        public Surface surface { get; set; }
        public State state { get; set; }
    }

    public class Season
    {
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public List<Venue> venues { get; set; }
    }

    public class League
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public Season season { get; set; }
    }

    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public League league { get; set; }
    }

    public class TennisVenuesResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
