using Newtonsoft.Json;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;
using System.IO;
using System.Net;
using System.Text;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneIngestService : IStatsProzoneIngestService
    {
        public void IngestReferenceData()
        {
            WebRequest request = WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/configuration/entities");
            request.Method = "GET";

            request.Headers["Authorization"] = "Basic c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==";
            request.ContentType = "application/json; charset=UTF-8";
            
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var entities = JsonConvert.DeserializeObject<Entities>(reader.ReadToEnd());
                }
            }
        }
    }
}
