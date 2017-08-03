using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.ViewModel
{
    public class LogsViewModel
    {
        public string GroupName { get; set; }
        public string GroupShortName { get; set; }
        public string LogName { get; set; }
        public string LeagueName { get; set; }
        public object TeamShortName { get; set; }
        public string Team { get; set; }
        public string TeamID { get; set; }
        public int Position { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drew { get; set; }
        public int Lost { get; set; }
        public int PointsFor { get; set; }
        public int PointsAgainst { get; set; }
        public int BonusPoints { get; set; }
        public int PointsDifference { get; set; }
        public int Points { get; set; }
        public int Sport { get; set; }
        public int NetRunRate { get; set; }
        public string Batting { get; set; }
        public string Bowling { get; set; }
        public string CricketBonus { get; set; }
        public string NoResult { get; set; }
        public int rank { get; set; }
        public int ConferenceRank { get; set; }
        public int CombinedRank { get; set; }
        public string HomeGroup { get; set; }
        public int IsConference { get; set; }
        public int TriesFor { get; set; }
        public int TriesAgainst { get; set; }
        public int TriesBonusPoints { get; set; }
        public int LossBonusPoints { get; set; }
    }
}