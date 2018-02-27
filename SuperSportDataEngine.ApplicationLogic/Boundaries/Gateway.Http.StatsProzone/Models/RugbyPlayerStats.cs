using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models
{
    public class RugbyPlayerStats
    {
        public int seasonId { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public List<Player> players { get; set; }
    }

    public class Stat
    {
        public int StatTypeID { get; set; }
        public double totalValue { get; set; }
        public double averageValue { get; set; }
        public string StatName { get; set; }
        public string StatNameInt { get; set; }
    }

    public class PlayerCareerStats
    {
        public int playerId { get; set; }
        public int appearances { get; set; }
        public int startingAppearances { get; set; }
        public List<Stat> Stat { get; set; }
    }

    public class Stat2
    {
        public int StatTypeID { get; set; }
        public double totalValue { get; set; }
        public double averageValue { get; set; }
        public string StatName { get; set; }
        public string StatNameInt { get; set; }
    }

    public class PlayerSeasonStats
    {
        public int appearances { get; set; }
        public int startingAppearances { get; set; }
        public int seasonId { get; set; }
        public string seasonName { get; set; }
        public List<Stat2> Stat { get; set; }
    }

    public class Stat3
    {
        public int StatTypeID { get; set; }
        public double totalValue { get; set; }
        public double averageValue { get; set; }
        public string StatName { get; set; }
        public string StatNameInt { get; set; }
    }

    public class PlayerTeamStats
    {
        public int appearances { get; set; }
        public int startingAppearances { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public List<Stat3> Stat { get; set; }
    }

    public class Player
    {
        public object Name { get; set; }
        public PlayerCareerStats playerCareerStats { get; set; }
        public string playerFirstName { get; set; }
        public int playerId { get; set; }
        public string playerLastName { get; set; }
        public string playerName { get; set; }
        public PlayerSeasonStats playerSeasonStats { get; set; }
        public object playerShortName { get; set; }
        public PlayerTeamStats playerTeamStats { get; set; }
        public int positionId { get; set; }
        public string positionName { get; set; }
        public int shirtNum { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
    }
}
