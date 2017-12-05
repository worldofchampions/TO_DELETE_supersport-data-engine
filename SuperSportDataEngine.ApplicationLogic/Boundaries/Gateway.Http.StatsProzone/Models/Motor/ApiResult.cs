using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor
{
    public class ApiResult
    {
        public int sportId { get; set; }
        public string name { get; set; }
        public League league { get; set; }
        public List<UriPath> uriPaths { get; set; }
    }

    public class UriPath
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
        public List<UriPath> uriPaths { get; set; }
        public Season season { get; set; }
        public List<Race> races { get; set; }
    }

    public class Season
    {
        public int season { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
    }

    public class Race
    {
        public int raceId { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class SubLeague
    {
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
        public object team { get; set; }
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
        public int totalLed { get; set; }
        public int completed { get; set; }
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
    }
}