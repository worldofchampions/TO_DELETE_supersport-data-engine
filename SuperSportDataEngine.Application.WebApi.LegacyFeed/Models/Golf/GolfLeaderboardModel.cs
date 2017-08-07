using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Golf
{
    [Serializable]
    public class GolfLeaderboardModel
    {
        public int Id
        {
            get;
            set;
        }
        public string TournamentId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public int Round
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public DateTime Updated
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime EndDate
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }

        public List<LeaderboardItemModel> Leaderboard
        {
            get;
            set;
        }

        public GolfMatchPlayLeaderboard MatchPlayLeaderboard
        {
            get;
            set;
        }
    }
}