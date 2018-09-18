using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse
{
    public class SurfaceType
    {
        public int surfaceId { get; set; }
        public string name { get; set; }
    }

    public class ApiResults
    {
        public List<SurfaceType> surfaceTypes { get; set; }
    }

    public class TennisSurfaceTypesResponse
    {
        public string status { get; set; }
        public int recordCount { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public double timeTaken { get; set; }
        public ApiResults apiResults { get; set; }
    }
}
