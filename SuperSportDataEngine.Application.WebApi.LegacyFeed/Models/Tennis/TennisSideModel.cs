using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis
{
    [Serializable]
    public class TennisSideModel
    {
        public int Number { get; set; }
        public List<TennisPlayerModel> Players { get; set; }
        public List<TennisSetModel> Sets { get; set; }
    }
}