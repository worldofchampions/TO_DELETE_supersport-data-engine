namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Golf
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;
    using System.Collections.Generic;

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