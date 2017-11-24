namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class QualifyingCriteriaModel
    {
        public string ConferenceName
        {
            get;
            set;
        }

        public int TopTeamsQualifiers
        {
            get;
            set;
        }

        public int PointsQualifiers
        {
            get;
            set;
        }

        public string ConferenceDisplayName
        {
            get;
            set;
        }

        public int ConferenceOrder
        {
            get;
            set;
        }

        public string ConferenceShortName
        {
            get;
            set;
        }

        public string PoolName
        {
            get;
            set;
        }

        public string PoolDisplayName
        {
            get;
            set;
        }

        public int PoolOrder
        {
            get;
            set;
        }

        public string PoolShortName
        {
            get;
            set;
        }
    }
}