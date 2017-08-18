using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using System.Collections.Generic;
using System.Linq;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class LegacyAuthService : ILegacyAuthService
    {
        private readonly IBaseEntityFrameworkRepository<LegacyZoneSite> _legacyZoneSiteRepository;
        private readonly IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> _legacyAuthFeedConsumerRepository;

        public LegacyAuthService(IBaseEntityFrameworkRepository<LegacyZoneSite> legacyZoneSiteRepository,
            IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> legacyAuthFeedConsumerRepository)
        {
            _legacyZoneSiteRepository = legacyZoneSiteRepository;
            _legacyAuthFeedConsumerRepository = legacyAuthFeedConsumerRepository;
        }

        public bool IsAuthorised(string authKey, int? siteId = null)
        {
            var legacyAuthFeed = _legacyAuthFeedConsumerRepository.Where(c => c.AuthKey == authKey).FirstOrDefault();
            if (legacyAuthFeed == null)
            {
                return false;
            }

            if (siteId != null)
            {
                var legacyZone = _legacyZoneSiteRepository.Where(c => c.Id == siteId).FirstOrDefault();
                return legacyZone.Feed == legacyAuthFeed.Name.Replace("  ", string.Empty).ToLowerInvariant();
            }

            return true;
        }

        public bool ImportZoneSiteRecords(IEnumerable<LegacyZoneSiteEntity> models)
        {
            //var legacyModels = 
            //_legacyAuthFeedConsumerRepository.AddRange()
            return true;
        }

        public bool ImportAuthFeedRecords(IEnumerable<LegacyAuthFeedConsumerEntity> models)
        {
            //var legacyModels = 
            //_legacyAuthFeedConsumerRepository.AddRange()
            return true;
        }
    }
}