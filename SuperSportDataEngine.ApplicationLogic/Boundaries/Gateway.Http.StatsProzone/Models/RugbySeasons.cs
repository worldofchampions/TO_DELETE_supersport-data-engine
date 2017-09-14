using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models
{
    public class Round
    {
        public bool active { get; set; }
        public int matches { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public int roundNumber { get; set; }
        public bool? final { get; set; }
    }

    public class Club
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamAbbr { get; set; }
        public string teamLogoUrl { get; set; }
    }

    public class Clubs
    {
        public List<Club> clubs { get; set; }
    }

    public class Season
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
        public bool currentSeason { get; set; }
        public List<Round> rounds { get; set; }
        public Clubs clubs { get; set; }
    }

    public class RugbySeasons
    {
        DateTime RequestTime { get; set; }
        DateTime ResponseTime { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public List<Season> season { get; set; }
    }
}