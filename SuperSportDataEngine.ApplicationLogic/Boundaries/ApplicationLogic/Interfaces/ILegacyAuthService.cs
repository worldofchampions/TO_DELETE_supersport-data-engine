using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ILegacyAuthService
    {
        bool IsAuthorised(string authKey, int siteId = 0);
        Task<bool> ImportZoneSiteRecords(IEnumerable<LegacyZoneSiteEntity> models);
        Task<bool> ImportAuthFeedRecords(IEnumerable<LegacyAuthFeedConsumerEntity> models);
    }
}