using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Enums;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models
{
    public class MongoMotorsportApiResult
    {
        [BsonElement("sport_id")]
        public int sportId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("league")]
        public League league { get; set; }
        [BsonElement("uri_paths")]
        public List<UriPath> uriPaths { get; set; }
        [BsonElement("leagues")]
        public List<League> leagues { get; set; }
    }

    public class UriPath
    {
        [BsonElement("path_sequence")]
        public int pathSequence { get; set; }
        [BsonElement("path")]
        public string path { get; set; }
    }

    public class League2
    {
        [BsonElement("league_id")]
        public int leagueId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
        [BsonElement("display_name")]
        public string displayName { get; set; }
        [BsonElement("sub_league")]
        public SubLeague subLeague { get; set; }
        [BsonElement("season")]
        public Season season { get; set; }
        [BsonElement("races")]
        public List<Race> races { get; set; }
        [BsonElement("uri_paths")]
        public List<UriPath2> uriPaths { get; set; }
    }

    public class UriPath2
    {
        [BsonElement("path_sequence")]
        public int pathSequence { get; set; }
        [BsonElement("path")]
        public string path { get; set; }
    }

    public class League
    {
        [BsonElement("league_id")]
        public int leagueId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
        [BsonElement("display_name")]
        public string displayName { get; set; }
        [BsonElement("sub_league")]
        public SubLeague subLeague { get; set; }
        [BsonElement("season")]
        public Season season { get; set; }
        [BsonElement("races")]
        public List<Race> races { get; set; }
        [BsonElement("uri_paths")]
        public List<UriPath2> uriPaths { get; set; }
        [BsonElement("league")]
        public League2 league { get; set; }
        [BsonElement("seasons")]
        public List<Season> seasons { get; set; }
    }


    public class Season
    {
        [BsonElement("owners")]
        public List<Owner> owners { get; set; }
        [BsonElement("season")]
        public int season { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("is_active")]
        public bool isActive { get; set; }
        [BsonElement("event_type")]
        public List<EventType> eventType { get; set; }
        [BsonElement("standings")]
        public Standings standings { get; set; }
    }

    public class EventType
    {
        [BsonElement("event_type_id")]
        public int eventTypeId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("start_date")]
        public StartDate startDate { get; set; }
        [BsonElement("end_date")]
        public EndDate endDate { get; set; }
        [BsonElement("events")]
        public List<Event> events { get; set; }
    }

    public class EndDate
    {
        [BsonElement("year")]
        public int year { get; set; }
        [BsonElement("month")]
        public int month { get; set; }
        [BsonElement("date")]
        public int date { get; set; }
        [BsonElement("full")]
        public DateTime full { get; set; }
        [BsonElement("date_type")]
        public string dateType { get; set; }
    }

    public class Event
    {
        [BsonElement("event_id")]
        public int eventId { get; set; }
        [BsonElement("event_round")]
        public EventRound eventRound { get; set; }
        [BsonElement("start_date")]
        public List<StartDate> startDate { get; set; }
        [BsonElement("is_tba")]
        public bool isTba { get; set; }
        [BsonElement("is_data_confirmed")]
        public IsDataConfirmed isDataConfirmed { get; set; }
        [BsonElement("event_status")]
        public EventStatus eventStatus { get; set; }
        [BsonElement("race")]
        public Race race { get; set; }
        [BsonElement("venue")]
        public Venue venue { get; set; }
        [BsonElement("tv_stations")]
        public List<TvStation> tvStations { get; set; }
        [BsonElement("champions")]
        public List<Champion> champions { get; set; }
        [BsonElement("scheduled_distance")]
        public ScheduledDistance scheduledDistance { get; set; }
        [BsonElement("points")]
        public Points points { get; set; }
        [BsonElement("car_details")]
        public CarDetails carDetails { get; set; }
        [BsonElement("qualifying_events")]
        public List<QualifyingEvent> qualifyingEvents { get; set; }
        [BsonElement("practice_events")]
        public List<PracticeEvent> practiceEvents { get; set; }
        [BsonElement("coverage_level")]
        public CoverageLevel coverageLevel { get; set; }
        [BsonElement("boxscores")]
        public Boxscore boxscore { get; set; }
    }

    public class Venue
    {
        [BsonElement("venue_id")]
        public int venueId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("state")]
        public State state { get; set; }
        [BsonElement("city")]
        public string city { get; set; }
        [BsonElement("country")]
        public Country country { get; set; }
    }

    public class TvStation
    {
        [BsonElement("tv_station_id")]
        public int tvStationId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("call_letters")]
        public string callLetters { get; set; }
        [BsonElement("network_type")]
        public NetworkType networkType { get; set; }
        [BsonElement("country")]
        public Country country { get; set; }
    }

    public class NetworkType
    {
        [BsonElement("network_type_id")]
        public int networkTypeId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class CompletedDistance
    {
        [BsonElement("laps")]
        public int laps { get; set; }
    }

    public class Cautions
    {
        [BsonElement("is_victory_under_caution")]
        public bool isVictoryUnderCaution { get; set; }
    }

    public class FastestSpeed
    {
        [BsonElement("speed_unit_id")]
        public int speedUnitId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
        [BsonElement("speed")]
        public string speed { get; set; }
    }

    public class FastestTime
    {
        [BsonElement("minutes")]
        public int minutes { get; set; }
        [BsonElement("seconds")]
        public int seconds { get; set; }
        [BsonElement("milliseconds")]
        public int milliseconds { get; set; }
    }

    public class PracticeEvent
    {
        [BsonElement("sequence")]
        public int sequence { get; set; }
        [BsonElement("start_date")]
        public List<StartDate> startDate { get; set; }
        [BsonElement("is_tba")]
        public bool isTba { get; set; }
    }

    public class CoverageLevel
    {
        [BsonElement("coverage_level_id")]
        public int coverageLevelId { get; set; }
        [BsonElement("details")]
        public string details { get; set; }
        [BsonElement("name")]
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
        [BsonElement("player")]
        public Player player { get; set; }
        [BsonElement("qualifying")]
        public Qualifying qualifying { get; set; }
        [BsonElement("car")]
        public Car car { get; set; }
        [BsonElement("owner")]
        public Owner owner { get; set; }
        [BsonElement("car_status")]
        public CarStatus carStatus { get; set; }
        [BsonElement("average_speed")]
        public List<object> averageSpeed { get; set; }
        [BsonElement("car_position")]
        public CarPosition carPosition { get; set; }
        [BsonElement("laps")]
        public Laps laps { get; set; }
        [BsonElement("money_won")]
        public List<object> moneyWon { get; set; }
        [BsonElement("finishing_time")]
        public FinishingTime finishingTime { get; set; }
        [BsonElement("points")]
        public Points points { get; set; }
    }

    public class Points
    {
        [BsonElement("driver")]
        public Driver driver { get; set; }
        [BsonElement("owner")]
        public Owner owner { get; set; }
        [BsonElement("are_points_available")]
        public bool arePointsAvailable { get; set; }
    }

    public class Champion
    {
        [BsonElement("champion_type")]
        public string championType { get; set; }
        [BsonElement("player_id")]
        public int playerId { get; set; }
        [BsonElement("first_name")]
        public string firstName { get; set; }
        [BsonElement("last_name")]
        public string lastName { get; set; }
        [BsonElement("owner")]
        public Owner owner { get; set; }
        [BsonElement("car")]
        public Car car { get; set; }
    }

    public class ScheduledDistance
    {
        [BsonElement("laps")]
        public int? laps { get; set; }
        [BsonElement("miles")]
        public double? miles { get; set; }
        [BsonElement("kilometers")]
        public double? kilometers { get; set; }
    }

    public class Driver
    {
        [BsonElement("total")]
        public string total { get; set; }
        [BsonElement("bonus")]
        public string bonus { get; set; }
        [BsonElement("penalty")]
        public string penalty { get; set; }
    }

    public class FinishingTime
    {
        [BsonElement("hours")]
        public int hours { get; set; }
        [BsonElement("minutes")]
        public int minutes { get; set; }
        [BsonElement("seconds")]
        public int seconds { get; set; }
        [BsonElement("milliseconds")]
        public int milliseconds { get; set; }
    }

    public class Boxscore
    {
        [BsonElement("time_of_race")]
        public TimeOfRace timeOfRace { get; set; }
        [BsonElement("victory_margin")]
        public VictoryMargin victoryMargin { get; set; }
        [BsonElement("average_speed")]
        public List<object> averageSpeed { get; set; }
        [BsonElement("fastest_speed")]
        public List<FastestSpeed> fastestSpeed { get; set; }
        [BsonElement("fastest_time")]
        public FastestTime fastestTime { get; set; }
        [BsonElement("pole_speed")]
        public List<object> poleSpeed { get; set; }
        [BsonElement("pole_times")]
        public PoleTime poleTime { get; set; }
        [BsonElement("cautions")]
        public Cautions cautions { get; set; }
        [BsonElement("completed_distance")]
        public CompletedDistance completedDistance { get; set; }
        [BsonElement("lead_changes")]
        public List<LeadChange> leadChanges { get; set; }
        [BsonElement("results")]
        public List<Result> results { get; set; }
    }

    public class EventRound
    {
        [BsonElement("event_round_id")]
        public int eventRoundId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Race
    {
        [BsonElement("race_id")]
        public int raceId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("display_name")]
        public string displayName { get; set; }
        [BsonElement("name_full")]
        public string nameFull { get; set; }
    }

    public class IsDataConfirmed
    {
        [BsonElement("results")]

        public bool results { get; set; }
    }

    public class EventStatus
    {
        [BsonElement("event-status_id")]
        public MotorsportRaceEventStatusProvider eventStatusId { get; set; }
        [BsonElement("is_under_caution")]
        public bool isUnderCaution { get; set; }
        [BsonElement("reason")]
        public string reason { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class SubLeague
    {
        [BsonElement("owners")]
        public List<Owner> owners;
        [BsonElement("sub_league_id")]
        public int subLeagueId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
        [BsonElement("display_name")]
        public string displayName { get; set; }
        [BsonElement("season")]
        public Season season { get; set; }
        [BsonElement("players")]
        public List<Player> players { get; set; }
    }

    public class Player
    {
        [BsonElement("player_id")]

        public int playerId { get; set; }
        [BsonElement("first_name")]
        public string firstName { get; set; }
        [BsonElement("last_name")]
        public string lastName { get; set; }
        [BsonElement("team")]
        public Team team { get; set; }
        [BsonElement("owner")]
        public Owner owner { get; set; }
        [BsonElement("rank")]
        public int rank { get; set; }
        [BsonElement("points")]
        public int points { get; set; }
        [BsonElement("chase_points")]
        public object chasePoints { get; set; }
        [BsonElement("finishes")]
        public Finishes finishes { get; set; }
        [BsonElement("earnings")]
        public object earnings { get; set; }
        [BsonElement("laps")]
        public Laps laps { get; set; }
        [BsonElement("starts")]
        public int starts { get; set; }
        [BsonElement("poles")]
        public int poles { get; set; }
        [BsonElement("car")]
        public Car car { get; set; }
        [BsonElement("chassis")]
        public Chassis chassis { get; set; }
        [BsonElement("engine")]
        public Engine engine { get; set; }
        [BsonElement("tire")]
        public Tire tire { get; set; }
        [BsonElement("sponsor")]
        public string sponsor { get; set; }
        [BsonElement("points_eligible")]
        public string pointsEligible { get; set; }
        [BsonElement("height")]
        public Height height { get; set; }
        [BsonElement("weight")]
        public Weight weight { get; set; }
        [BsonElement("birth")]
        public Birth birth { get; set; }
    }

    public class Birth
    {
        [BsonElement("birth_date")]
        public BirthDate birthDate { get; set; }
        [BsonElement("city")]
        public string city { get; set; }
        [BsonElement("country")]
        public Country country { get; set; }
        [BsonElement("state")]
        public State state { get; set; }
    }

    public class BirthDate
    {
        [BsonElement("year")]
        public int year { get; set; }
        [BsonElement("month")]
        public int month { get; set; }
        [BsonElement("date")]
        public int date { get; set; }
        [BsonElement("full")]
        public string full { get; set; }
    }

    public class Country
    {
        [BsonElement("country_id")]
        public int countryId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
    }

    public class State
    {
        [BsonElement("state_id")]
        public int stateId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
    }

    public class Weight
    {
        [BsonElement("kilograms")]
        public double kilograms { get; set; }
        [BsonElement("pounds")]
        public double pounds { get; set; }
    }

    public class Height
    {
        [BsonElement("centimeters")]
        public double centimeters { get; set; }
        [BsonElement("inches")]
        public double inches { get; set; }
    }

    public class Tire
    {
        [BsonElement("tire_id")]
        public int tireId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Engine
    {
        [BsonElement("engine_id")]
        public int engineId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Chassis
    {
        [BsonElement("chassis_id")]
        public int chassisId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Qualifying
    {
        [BsonElement("exemption")]
        public Exemption exemption { get; set; }
        [BsonElement("is_wild_card")]
        public bool isWildCard { get; set; }
        [BsonElement("is_qualified")]
        public bool isQualified { get; set; }
        [BsonElement("qualifying_runs")]
        public List<QualifyingRun> qualifyingRuns { get; set; }
    }

    public class QualifyingEvent
    {
        [BsonElement("sequence")]
        public int sequence { get; set; }
        [BsonElement("start_date")]
        public List<StartDate> startDate { get; set; }
        [BsonElement("is_tba")]
        public bool isTba { get; set; }
    }

    public class QualifyingRun
    {
        [BsonElement("sequence")]
        public int sequence { get; set; }
        [BsonElement("start_date")]
        public List<StartDate> startDate { get; set; }
        [BsonElement("average_speed")]
        public List<AverageSpeed> averageSpeed { get; set; }
        [BsonElement("time")]
        public Time time { get; set; }
    }

    public class CarDetails
    {
        [BsonElement("is_restrictor_plate")]
        public bool isRestrictorPlate { get; set; }
    }

    public class StartDate
    {
        [BsonElement("year")]
        public int year { get; set; }
        [BsonElement("month")]
        public int month { get; set; }
        [BsonElement("date")]
        public int date { get; set; }
        [BsonElement("hour")]
        public int hour { get; set; }
        [BsonElement("minute")]
        public int minute { get; set; }
        [BsonElement("full")]
        public DateTime full { get; set; }
        [BsonElement("date_type")]
        public string dateType { get; set; }
    }

    public class Exemption
    {
    }

    public class LeadChange
    {
        [BsonElement("sequence")]
        public int sequence { get; set; }
        [BsonElement("player")]
        public Player player { get; set; }
        [BsonElement("start_lap")]
        public int startLap { get; set; }
        [BsonElement("end_lap")]
        public int endLap { get; set; }
        [BsonElement("owner")]
        public Owner owner { get; set; }
        [BsonElement("car")]
        public Car car { get; set; }
    }

    public class AverageSpeed
    {
        [BsonElement("speed_unit_id")]
        public int speedUnitId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("abbreviation")]
        public string abbreviation { get; set; }
        [BsonElement("speed")]
        public string speed { get; set; }
    }

    public class Time
    {
        [BsonElement("minutes")]
        public int minutes { get; set; }
        [BsonElement("seconds")]
        public int seconds { get; set; }
        [BsonElement("milliseconds")]
        public int milliseconds { get; set; }
    }

    public class Car
    {
        [BsonElement("make")]
        public Make make { get; set; }
        [BsonElement("car_number")]
        public int? carNumber { get; set; }
        [BsonElement("car_display_number")]
        public int? carDisplayNumber { get; set; }
    }

    public class Make
    {
        [BsonElement("make_id")]
        public int makeId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Laps
    {
        [BsonElement("total_led")]
        public int? totalLed { get; set; }
        [BsonElement("completed")]
        public int? completed { get; set; }
        [BsonElement("behind")]
        public int? behind { get; set; }
        [BsonElement("is_fastest")]
        public bool isFastest { get; set; }
        [BsonElement("led")]
        public List<object> led { get; set; }
    }

    public class CarPosition
    {
        [BsonElement("starting_position")]
        public int startingPosition { get; set; }
        [BsonElement("position")]
        public int position { get; set; }
    }

    public class CarStatus
    {
        [BsonElement("car_status_id")]
        public int carStatusId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
    }

    public class Finishes
    {
        [BsonElement("first")]
        public int first { get; set; }
        [BsonElement("second")]
        public int second { get; set; }
        [BsonElement("third")]
        public int third { get; set; }
        [BsonElement("top5")]
        public int top5 { get; set; }
        [BsonElement("top10")]
        public int top10 { get; set; }
        [BsonElement("top15")]
        public int top15 { get; set; }
        [BsonElement("top20")]
        public int top20 { get; set; }
        [BsonElement("did_not_finish")]
        public int didNotFinish { get; set; }
    }

    public class Owner
    {
        [BsonElement("owner_id")]
        public int ownerId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("total")]
        public string total { get; set; }
        [BsonElement("bonus")]
        public string bonus { get; set; }
        [BsonElement("penalty")]
        public string penalty { get; set; }
    }

    public class DateThrough
    {
        [BsonElement("year")]
        public int year { get; set; }
        [BsonElement("month")]
        public int month { get; set; }
        [BsonElement("date")]
        public int date { get; set; }
        [BsonElement("full")]
        public string full { get; set; }
        [BsonElement("date_type")]
        public string dateType { get; set; }
    }

    public class LastEvent
    {
        [BsonElement("event_id")]
        public int eventId { get; set; }
        [BsonElement("race")]
        public Race race { get; set; }
    }

    public class Team
    {
        [BsonElement("team_id")]
        public int teamId { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("rank")]
        public int rank { get; set; }
        [BsonElement("points")]
        public int points { get; set; }
        [BsonElement("finishes")]
        public Finishes finishes { get; set; }
        [BsonElement("earnings")]
        public object earnings { get; set; }
        [BsonElement("starts")]
        public int starts { get; set; }
        [BsonElement("poles")]
        public int poles { get; set; }
    }

    public class Standings
    {
        [BsonElement("week_through")]
        public int weekThrough { get; set; }
        [BsonElement("date_through")]
        public DateThrough dateThrough { get; set; }
        [BsonElement("last_event")]
        public LastEvent lastEvent { get; set; }
        [BsonElement("teams")]
        public List<Team> teams { get; set; }
        [BsonElement("players")]
        public List<Player> players { get; set; }
    }
}