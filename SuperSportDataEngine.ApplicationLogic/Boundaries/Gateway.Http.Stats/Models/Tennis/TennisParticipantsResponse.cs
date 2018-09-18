using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse
{

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

    public class Residence
    {
        public string city { get; set; }
        public Country country { get; set; }
        public State state { get; set; }
    }

    public class Handedness
    {
        public int handId { get; set; }
        public string name { get; set; }
    }

    public class Height
    {
        public double centimeters { get; set; }
        public double inches { get; set; }
    }

    public class Weight
    {
        public double kilograms { get; set; }
        public double pounds { get; set; }
    }

    public class BirthDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public string full { get; set; }
    }

    public class Country2
    {
        public int countryId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class State2
    {
        public int stateId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class Birth
    {
        public BirthDate birthDate { get; set; }
        public string city { get; set; }
        public Country2 country { get; set; }
        public State2 state { get; set; }
    }

    public class Nationality
    {
        public int countryId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class Experience
    {
        public int yearFirst { get; set; }
    }

    public class Shoe
    {
        public int shoeId { get; set; }
        public string name { get; set; }
    }

    public class Clothing
    {
        public int clothingId { get; set; }
        public string name { get; set; }
    }

    public class Player
    {
        public Residence residence { get; set; }
        public int playerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Handedness handedness { get; set; }
        public string gender { get; set; }
        public Height height { get; set; }
        public Weight weight { get; set; }
        public Birth birth { get; set; }
        public Nationality nationality { get; set; }
        public Experience experience { get; set; }
        public Shoe shoe { get; set; }
        public Clothing clothing { get; set; }
    }

    public class SubLeague
    {
        public int subLeagueId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public string displayName { get; set; }
        public List<Player> players { get; set; }
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

    public class TennisParticipantsResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public List<ApiResult> apiResults { get; set; }
    }
}
