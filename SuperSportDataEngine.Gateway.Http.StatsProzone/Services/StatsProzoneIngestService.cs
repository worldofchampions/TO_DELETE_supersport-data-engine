using Newtonsoft.Json;
using SuperSportDataEngine.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;
using System.IO;
using System.Net;
using System.Text;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneIngestService : IStatsProzoneRequestService
    {
        public Entities RequestReferenceData()
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
                    return JsonConvert.DeserializeObject<Entities>(reader.ReadToEnd());
                }
            }
        }
    }
}
