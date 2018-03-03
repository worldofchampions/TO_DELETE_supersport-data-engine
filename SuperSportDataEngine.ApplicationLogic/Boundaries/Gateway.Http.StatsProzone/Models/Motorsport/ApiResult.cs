namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport
{
    using System;
    using System.Collections.Generic;

    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public League league { get; set; }
        public List<UriPath> uriPaths { get; set; }
        public List<League> leagues { get; set; }
    }

    public class UriPath
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
        // public List<UriPath> uriPaths { get; set; }
        public Season season { get; set; }
        public List<Race> races { get; set; }
        public List<UriPath2> uriPaths { get; set; }
    }

    public class UriPath2
    {
        public int pathSequence { get; set; }
        public string path { get; set; }
    }

    public class League
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public SubLeague subLeague { get; set; }
        public Season season { get; set; }
        public List<Race> races { get; set; }
        public List<UriPath2> uriPaths { get; set; }
        public League2 league { get; set; }
        public List<Season> seasons { get; set; }
    }


    public class Season
    {
        public List<Owner> owners { get; set; }
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public List<EventType> eventType { get; set; }
        public Standings standings { get; set; }
    }

    public class EventType
    {
        public int eventTypeId { get; set; }
        public string name { get; set; }
        public StartDate startDate { get; set; }
        public EndDate endDate { get; set; }
        public List<Event> events { get; set; }
    }

    public class EndDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
        public string dateType { get; set; }
    }

    public class Event
    {
        public int eventId { get; set; }
        public EventRound eventRound { get; set; }
        public List<StartDate> startDate { get; set; }
        public bool isTba { get; set; }
        public IsDataConfirmed isDataConfirmed { get; set; }
        public EventStatus eventStatus { get; set; }
        public Race race { get; set; }
        public Venue venue { get; set; }
        public List<TvStation> tvStations { get; set; }
        public List<Champion> champions { get; set; }
        public ScheduledDistance scheduledDistance { get; set; }
        public Points points { get; set; }
        public CarDetails carDetails { get; set; }
        public List<QualifyingEvent> qualifyingEvents { get; set; }
        public List<PracticeEvent> practiceEvents { get; set; }
        public CoverageLevel coverageLevel { get; set; }
        public Boxscore boxscore { get; set; }
    }

    public class Venue
    {
        public int venueId { get; set; }
        public string name { get; set; }
        public State state { get; set; }
        public string city { get; set; }
        public Country country { get; set; }
    }

    public class TvStation
    {
        public int tvStationId { get; set; }
        public string name { get; set; }
        public string callLetters { get; set; }
        public NetworkType networkType { get; set; }
        public Country country { get; set; }
    }

    public class NetworkType
    {
        public int networkTypeId { get; set; }
        public string name { get; set; }
    }

    public class CompletedDistance
    {
        public int laps { get; set; }
    }

    public class Cautions
    {
        public bool isVictoryUnderCaution { get; set; }
    }

    public class FastestSpeed
    {
        public int speedUnitId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string speed { get; set; }
    }

    public class FastestTime
    {
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int milliseconds { get; set; }
    }

    public class PracticeEvent
    {
        public int sequence { get; set; }
        public List<StartDate> startDate { get; set; }
        public bool isTba { get; set; }
    }

    public class CoverageLevel
    {
        public int coverageLevelId { get; set; }
        public string details { get; set; }
        public string name { get; set; }
    }

    public class TimeOfRace
    {
    }

    public class VictoryMargin
    {
    }

    public class PoleTime
    {
    }

    public class Result
    {
        public Player player { get; set; }
        public Qualifying qualifying { get; set; }
        public Car car { get; set; }
        public Owner owner { get; set; }
        public CarStatus carStatus { get; set; }
        public List<object> averageSpeed { get; set; }
        public CarPosition carPosition { get; set; }
        public Laps laps { get; set; }
        public List<object> moneyWon { get; set; }
        public FinishingTime finishingTime { get; set; }
        public Points points { get; set; }
    }

    public class Points
    {
        public Driver driver { get; set; }
        public Owner owner { get; set; }
        public bool arePointsAvailable { get; set; }
    }

    public class Champion
    {
        public string championType { get; set; }
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Owner owner { get; set; }
        public Car car { get; set; }
    }

    public class ScheduledDistance
    {
        public int laps { get; set; }
        public double miles { get; set; }
        public double kilometers { get; set; }
    }

    public class Driver
    {
        public string total { get; set; }
        public string bonus { get; set; }
        public string penalty { get; set; }
    }

    public class FinishingTime
    {
        public int hours { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int milliseconds { get; set; }
    }

    public class Boxscore
    {
        public TimeOfRace timeOfRace { get; set; }
        public VictoryMargin victoryMargin { get; set; }
        public List<object> averageSpeed { get; set; }
        public List<FastestSpeed> fastestSpeed { get; set; }
        public FastestTime fastestTime { get; set; }
        public List<object> poleSpeed { get; set; }
        public PoleTime poleTime { get; set; }
        public Cautions cautions { get; set; }
        public CompletedDistance completedDistance { get; set; }
        public List<LeadChange> leadChanges { get; set; }
        public List<Result> results { get; set; }
    }

    public class EventRound
    {
        public int eventRoundId { get; set; }
        public string name { get; set; }
    }

    public class Race
    {
        public int raceId { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string nameFull { get; set; }
    }

    public class IsDataConfirmed
    {
        public bool results { get; set; }
    }

    public class EventStatus
    {
        public int eventStatusId { get; set; }
        public bool isUnderCaution { get; set; }
        public string reason { get; set; }
        public string name { get; set; }
    }

    public class SubLeague
    {
        public List<Owner> owners;
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public Season season { get; set; }
        public List<Player> players { get; set; }
    }

    public class Player
    {
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Team team { get; set; }
        public Owner owner { get; set; }
        public int rank { get; set; }
        public int points { get; set; }
        public object chasePoints { get; set; }
        public Finishes finishes { get; set; }
        public object earnings { get; set; }
        public Laps laps { get; set; }
        public int starts { get; set; }
        public int poles { get; set; }
        public Car car { get; set; }
        public Chassis chassis { get; set; }
        public Engine engine { get; set; }
        public Tire tire { get; set; }
        public string sponsor { get; set; }
        public string pointsEligible { get; set; }
        public Height height { get; set; }
        public Weight weight { get; set; }
        public Birth birth { get; set; }
    }

    public class Birth
    {
        public BirthDate birthDate { get; set; }
        public string city { get; set; }
        public Country country { get; set; }
        public State state { get; set; }
    }

    public class BirthDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
    }

    public class Country
    {
        public int countryId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class State
    {
        public int stateId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class Weight
    {
        public double kilograms { get; set; }
        public double pounds { get; set; }
    }

    public class Height
    {
        public double centimeters { get; set; }
        public double inches { get; set; }
    }

    public class Tire
    {
        public int tireId { get; set; }
        public string name { get; set; }
    }

    public class Engine
    {
        public int engineId { get; set; }
        public string name { get; set; }
    }

    public class Chassis
    {
        public int chassisId { get; set; }
        public string name { get; set; }
    }

    public class Qualifying
    {
        public Exemption exemption { get; set; }
        public bool isWildCard { get; set; }
        public bool isQualified { get; set; }
        public List<QualifyingRun> qualifyingRuns { get; set; }
    }

    public class QualifyingEvent
    {
        public int sequence { get; set; }
        public List<StartDate> startDate { get; set; }
        public bool isTba { get; set; }
    }

    public class QualifyingRun
    {
        public int sequence { get; set; }
        public List<StartDate> startDate { get; set; }
        public List<AverageSpeed> averageSpeed { get; set; }
        public Time time { get; set; }
    }

    public class CarDetails
    {
        public bool isRestrictorPlate { get; set; }
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

    public class Exemption
    {
    }

    public class LeadChange
    {
        public int sequence { get; set; }
        public Player player { get; set; }
        public int startLap { get; set; }
        public int endLap { get; set; }
        public Owner owner { get; set; }
        public Car car { get; set; }
    }

    public class AverageSpeed
    {
        public int speedUnitId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string speed { get; set; }
    }

    public class Time
    {
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int milliseconds { get; set; }
    }

    public class Car
    {
        public Make make { get; set; }
        public int? carNumber { get; set; }
        public int? carDisplayNumber { get; set; }
    }

    public class Make
    {
        public int makeId { get; set; }
        public string name { get; set; }
    }

    public class Laps
    {
        public int? totalLed { get; set; }
        public int? completed { get; set; }
        public int? behind { get; set; }
        public bool isFastest { get; set; }
        public List<object> led { get; set; }
    }

    public class CarPosition
    {
        public int startingPosition { get; set; }
        public int position { get; set; }
    }

    public class CarStatus
    {
        public int carStatusId { get; set; }
        public string name { get; set; }
    }

    public class Finishes
    {
        public int first { get; set; }
        public int second { get; set; }
        public int third { get; set; }
        public int top5 { get; set; }
        public int top10 { get; set; }
        public int top15 { get; set; }
        public int top20 { get; set; }
        public int didNotFinish { get; set; }
    }

    public class Owner
    {
        public int ownerId { get; set; }
        public string name { get; set; }
        public string total { get; set; }
        public string bonus { get; set; }
        public string penalty { get; set; }
    }

    public class DateThrough
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
        public string dateType { get; set; }
    }

    public class LastEvent
    {
        public int eventId { get; set; }
        public Race race { get; set; }
    }

    public class Team
    {
        public int teamId { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public int points { get; set; }
        public Finishes finishes { get; set; }
        public object earnings { get; set; }
        public int starts { get; set; }
        public int poles { get; set; }
    }

    public class Standings
    {
        public int weekThrough { get; set; }
        public DateThrough dateThrough { get; set; }
        public LastEvent lastEvent { get; set; }
        public List<Team> teams { get; set; }
        public List<Player> players { get; set; }
    }
}