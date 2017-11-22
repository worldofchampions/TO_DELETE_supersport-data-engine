namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class MarginModel
    {
        public int TeamId
        {
            get;
            set;
        }

        public string TeamName
        {
            get;
            set;
        }

        public int Points
        {
            get;
            set;
        }
    }
}