using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class LegacyAuthService : ILegacyAuthService
    {
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public LegacyAuthService(
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public bool IsAuthorised(string authKey, int siteId = 0)
        {
            var legacyAuthFeed = _systemSportDataUnitOfWork.LegacyAuthFeedConsumers.Where(c => c.AuthKey == authKey && c.Active).FirstOrDefault();
            if (legacyAuthFeed == null)
            {
                return false;
            }

            if (siteId != 0)
            {
                var legacyZone = _systemSportDataUnitOfWork.LegacyZoneSites.Where(c => c.Id == siteId).FirstOrDefault();
                if (legacyZone == null)
                {
                    return false;
                }
                return legacyZone.Feed == legacyAuthFeed.Name.Replace("  ", string.Empty).ToLowerInvariant();
            }

            return true;
        }

        public async Task<bool> ImportZoneSiteRecords(IEnumerable<LegacyZoneSiteEntity> models)
        {
            var legacyModels = models.Select(entity => Mapper.Map<LegacyZoneSite>(entity));
            _systemSportDataUnitOfWork.LegacyZoneSites.AddRange(new HashSet<LegacyZoneSite>(legacyModels));
            await _systemSportDataUnitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ImportAuthFeedRecords(IEnumerable<LegacyAuthFeedConsumerEntity> models)
        {
            var legacyModels = models.Select(entity => LegacyAuthFeedConsumerMapper.MapToModel(entity));
            _systemSportDataUnitOfWork.LegacyAuthFeedConsumers.AddRange(new HashSet<LegacyAuthFeedConsumer>(legacyModels));
            await _systemSportDataUnitOfWork.SaveChangesAsync();

            return true;
        }
    }
}