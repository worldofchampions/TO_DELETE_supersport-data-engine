using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneIngestService : IStatsProzoneIngestService
    {
        public void IngestReferenceData()
        {
            // Do provider call here.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", "c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==");

            var response = client.GetStringAsync("http://rugbyunion-api.stats.com/api/ru/configuration/entities");
        }
    }
}
