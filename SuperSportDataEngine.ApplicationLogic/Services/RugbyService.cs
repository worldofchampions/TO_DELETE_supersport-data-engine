using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities;
using SuperSportDataEngine.Common.Boundaries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        private readonly ICache _cache;

        public RugbyService(ICache cache)
        {
            _cache = cache;
        }

        public async Task<List<LogEntity>> GetLogs(string tournamentName)
        {
            var cacheKey = $"rugby/{tournamentName}/logs";
            var logs = await _cache.GetAsync<List<LogEntity>>(cacheKey);

            if (logs == null)
            {
                //TODO: Get data from the DB
                logs = new List<LogEntity>();

                _cache.Add(cacheKey, logs);
            }

            return logs;
        }
    }
}