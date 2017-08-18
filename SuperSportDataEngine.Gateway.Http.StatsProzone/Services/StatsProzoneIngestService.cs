namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System.IO;
    using System.Net;
    using System.Text;
    using System;
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;

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
