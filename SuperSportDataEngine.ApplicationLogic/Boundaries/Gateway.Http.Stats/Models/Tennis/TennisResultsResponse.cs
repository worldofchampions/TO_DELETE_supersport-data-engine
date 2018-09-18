using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse
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

    public class Player2
    {
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class Seed
    {
        public string seedValue { get; set; }
        public List<Player2> players { get; set; }
    }

    public class MatchTime
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public DateTime full { get; set; }
        public string dateType { get; set; }
    }

    public class MatchTimeStatus
    {
        public int matchTimeStatusId { get; set; }
        public string name { get; set; }
        public int set { get; set; }
        public int game { get; set; }
    }

    public class MakeupDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public DateTime full { get; set; }
        public string dateType { get; set; }
    }

    public class MatchStatus
    {
        public int matchStatusId { get; set; }
        public string name { get; set; }
    }

    public class RoundType
    {
        public int roundTypeId { get; set; }
        public string name { get; set; }
    }

    public class Player3
    {
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
    }

    public class Set
    {
        public int setNumber { get; set; }
        public string result { get; set; }
        public int games { get; set; }
        public int? tiebreak { get; set; }
    }

    public class Linescore
    {
        public int setsWon { get; set; }
        public bool isServe { get; set; }
        public List<Set> sets { get; set; }
    }

    public class FirstServe
    {
        public int? serves { get; set; }
        public int? wins { get; set; }
    }

    public class SecondServe
    {
        public int? serves { get; set; }
        public int? wins { get; set; }
    }

    public class ReceivingPoints
    {
        public int? total { get; set; }
        public int? won { get; set; }
    }

    public class BreakPoints
    {
        public int total { get; set; }
        public int won { get; set; }
    }

    public class NetApproaches
    {
        public int? total { get; set; }
        public int? won { get; set; }
    }

    public class Points
    {
        public int total { get; set; }
        public int won { get; set; }
    }

    public class FirstServeSpeed
    {
        public int speedUnitId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string averageSpeed { get; set; }
    }

    public class SecondServeSpeed
    {
        public int speedUnitId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string averageSpeed { get; set; }
    }

    public class ServeSpeed
    {
        public int speedUnitId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string maximumSpeed { get; set; }
    }

    public class Serve
    {
        public List<FirstServeSpeed> firstServeSpeed { get; set; }
        public List<SecondServeSpeed> secondServeSpeed { get; set; }
        public List<ServeSpeed> serveSpeed { get; set; }
    }

    public class MatchStats
    {
        public int aces { get; set; }
        public int doubleFaults { get; set; }
        public int? errors { get; set; }
        public FirstServe firstServe { get; set; }
        public SecondServe secondServe { get; set; }
        public int? winners { get; set; }
        public ReceivingPoints receivingPoints { get; set; }
        public BreakPoints breakPoints { get; set; }
        public NetApproaches netApproaches { get; set; }
        public Points points { get; set; }
        public Serve serve { get; set; }
    }

    public class Side
    {
        public List<Player3> players { get; set; }
        public bool isWinner { get; set; }
        public bool isWithdrawal { get; set; }
        public Linescore linescore { get; set; }
        public MatchStats matchStats { get; set; }
        public int rotation { get; set; }
        public string seedValue { get; set; }
        public string reason { get; set; }
    }

    public class Match
    {
        public int matchId { get; set; }
        public List<MatchTime> matchTime { get; set; }
        public MatchTimeStatus matchTimeStatus { get; set; }
        public List<MakeupDate> makeupDate { get; set; }
        public MatchStatus matchStatus { get; set; }
        public int round { get; set; }
        public RoundType roundType { get; set; }
        public List<Side> sides { get; set; }
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
        public List<Seed> seeds { get; set; }
        public List<Match> matches { get; set; }
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

    public class TennisResultsResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
