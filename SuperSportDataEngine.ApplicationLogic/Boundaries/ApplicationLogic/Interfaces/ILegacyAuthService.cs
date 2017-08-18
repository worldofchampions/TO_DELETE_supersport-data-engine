using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ILegacyAuthService
    {
        bool IsAuthorised(string authKey, int? siteId = null);
        bool ImportZoneSiteRecords(IEnumerable<LegacyZoneSiteEntity> models);
        bool ImportAuthFeedRecords(IEnumerable<LegacyAuthFeedConsumerEntity> models);
    }
}