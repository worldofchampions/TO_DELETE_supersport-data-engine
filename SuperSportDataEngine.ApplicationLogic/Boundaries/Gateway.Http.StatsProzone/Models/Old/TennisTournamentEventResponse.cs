using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Tennis.TennisTournamentEventResponse
{
    public class Tournament
    {
        public int tournamentId { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class TourType
    {
        public int tourTypeId { get; set; }
        public string name { get; set; }
    }

    public class StartDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public DateTime full { get; set; }
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

    public class IsDataConfirmed
    {
        public bool results { get; set; }
    }

    public class State
    {
        public int stateId { get; set; }
        public string name { get; set; }
    }

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

    public class Venue
    {
        public int venueId { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public State state { get; set; }
        public Country country { get; set; }
        public Surface surface { get; set; }
    }

    public class EventStatus
    {
        public int eventStatusId { get; set; }
        public string name { get; set; }
    }

    public class PrizeMoney
    {
        public int currencyId { get; set; }
        public string name { get; set; }
        public string money { get; set; }
    }

    public class Player
    {
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class Champion
    {
        public string championType { get; set; }
        public List<Player> players { get; set; }
    }

    public class CoverageLevel
    {
        public int coverageLevelId { get; set; }
        public string details { get; set; }
        public string name { get; set; }
    }

    public class MatchType
    {
        public int matchTypeId { get; set; }
        public string name { get; set; }
        public EventStatus eventStatus { get; set; }
        public List<PrizeMoney> prizeMoney { get; set; }
        public int draw { get; set; }
        public int roundOf16 { get; set; }
        public List<Champion> champions { get; set; }
        public CoverageLevel coverageLevel { get; set; }
    }

    public class Event
    {
        public int eventId { get; set; }
        public Tournament tournament { get; set; }
        public TourType tourType { get; set; }
        public List<StartDate> startDate { get; set; }
        public List<EndDate> endDate { get; set; }
        public IsDataConfirmed isDataConfirmed { get; set; }
        public Venue venue { get; set; }
        public List<MatchType> matchTypes { get; set; }
    }

    public class EventType
    {
        public int eventTypeId { get; set; }
        public string name { get; set; }
        public List<Event> events { get; set; }
    }

    public class Season
    {
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public List<EventType> eventType { get; set; }
    }

    public class SubLeague
    {
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public Season season { get; set; }
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

    public class TennisTournamentEventResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
