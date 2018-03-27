using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Models
{
    public class MongoMotorsportEntitiesResponse
    {
        [BsonElement("status")]
        public string status { get; set; }
        [BsonElement("record_count")]
        public int recordCount { get; set; }
        [BsonElement("start_timestamp")]
        public DateTime startTimestamp { get; set; }
        [BsonElement("end_timestamp")]
        public DateTime endTimestamp { get; set; }
        [BsonElement("time_taken")]
        public double timeTaken { get; set; }
        [BsonElement("api_result")]
        public List<MongoMotorsportApiResult> apiResults { get; set; }
    }
}
