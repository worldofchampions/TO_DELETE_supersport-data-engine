using Newtonsoft.Json;
using SuperSportDataEngine.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using System.IO;
using System.Net;
using System.Text;
using System;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneIngestService : IStatsProzoneIngestService
    {
        public EntitiesResponse IngestReferenceData()
        {
            WebRequest request = WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/configuration/entities");
            request.Method = "GET";

            request.Headers["Authorization"] = "Basic c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==";
            request.ContentType = "application/json; charset=UTF-8";

            var entitiesResponse =
                new EntitiesResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    entitiesResponse.Entities = 
                        JsonConvert.DeserializeObject<Entities>(reader.ReadToEnd());

                    return entitiesResponse;
                }
            }
        }
    }
}
