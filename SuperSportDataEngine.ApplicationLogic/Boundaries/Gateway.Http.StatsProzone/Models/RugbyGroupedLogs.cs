using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs
{
    public class RecentFormList
    {
        public string recentForm1 { get; set; }
        public string recentForm2 { get; set; }
        public string recentForm3 { get; set; }
        public string recentForm4 { get; set; }
        public string recentForm5 { get; set; }
    }

    public class Ladderposition
    {
        public int position { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public int gamesPlayed { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int competitionPoints { get; set; }
        public int pointsFor { get; set; }
        public int pointsAgainst { get; set; }
        public int pointsDifference { get; set; }
        public int homeWins { get; set; }
        public int homeLosses { get; set; }
        public int awayWins { get; set; }
        public int awayLosses { get; set; }
        public RecentFormList recentFormList { get; set; }
        public string recentForm { get; set; }
        public string seasonForm { get; set; }
        public int group { get; set; }
        public string groupName { get; set; }
        public int triesFor { get; set; }
        public int triesConceded { get; set; }
        public int triesAgainst { get; set; }
        public int goalsFor { get; set; }
        public int goalsConceded { get; set; }
        public int bonusPoints { get; set; }
        public int overallPosition { get; set; }
        public int? fieldGoalsConceded { get; set; }
        public int? draws { get; set; }
        public int? awayDraws { get; set; }
        public int? fieldGoalsFor { get; set; }
        public int? homeDraws { get; set; }
    }

    public class OverallStandings
    {
        public List<Ladderposition> ladderposition { get; set; }
    }

    public class GroupStandings
    {
        public List<Ladderposition> ladderposition { get; set; }
    }

    public class SecondaryGroupStandings
    {
        public List<Ladderposition> ladderposition { get; set; }
    }

    public class RugbyGroupedLogs
    {
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public int competitionId { get; set; }
        public string competitionName { get; set; }
        public int roundNumber { get; set; }
        public OverallStandings overallStandings { get; set; }
        public GroupStandings groupStandings { get; set; }
        public SecondaryGroupStandings secondaryGroupStandings { get; set; }
        public int seasonId { get; set; }
    }
}
