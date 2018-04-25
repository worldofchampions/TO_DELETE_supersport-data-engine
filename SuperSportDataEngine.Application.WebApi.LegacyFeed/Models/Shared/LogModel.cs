namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    public class Log
    {
        public string GroupName
        {
            get;
            set;
        }

        public string GroupShortName
        {
            get;
            set;
        }

        public string LogName
        {
            get;
            set;
        }

        public string LeagueName
        {
            get;
            set;
        }

        public string TeamShortName
        {
            get;
            set;
        }

        public string Team
        {
            get;
            set;
        }

        public string TeamID
        {
            get;
            set;
        }

        public int Position
        {
            get;
            set;
        }

        public int Played
        {
            get;
            set;
        }

        public int Won
        {
            get;
            set;
        }

        public int Drew
        {
            get;
            set;
        }

        public int Lost
        {
            get;
            set;
        }

        public double PointsFor
        {
            get;
            set;
        }

        public double PointsAgainst
        {
            get;
            set;
        }

        public double BonusPoints
        {
            get;
            set;
        }

        public double PointsDifference
        {
            //get
            //{
            //    return PointsFor - PointsAgainst;
            //}
            get;
            set;
        }

        public double Points
        {
            get;
            set;
        }

        public SportType Sport
        {
            get;
            set;
        }

        public string NetRunRate
        {
            get;
            set;
        }

        public string Batting
        {
            get;
            set;
        }

        public string Bowling
        {
            get;
            set;
        }

        public string CricketBonus
        {
            get;
            set;
        }

        public string NoResult
        {
            get;
            set;
        }

        public int rank
        {
            get;
            set;
        }

        //SuperRugby Conference Value
        public int ConferenceRank
        {
            get;
            set;
        }

        //SuperRugby Conference Value
        public int CombinedRank
        {
            get;
            set;
        }

        //SuperRugby Conference Value
        public string HomeGroup
        {
            get;
            set;
        }

        //SuperRugby Conference Value
        public int IsConference
        {
            get;
            set;
        }

        public int TriesFor
        {
            get;
            set;
        }

        public int TriesAgainst
        {
            get;
            set;
        }

        public int TriesBonusPoints
        {
            get;
            set;
        }

        public int LossBonusPoints
        {
            get;
            set;
        }
    }
}