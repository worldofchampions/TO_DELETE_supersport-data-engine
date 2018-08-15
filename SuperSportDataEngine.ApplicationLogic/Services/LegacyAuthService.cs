using System;
using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class LegacyAuthService : ILegacyAuthService
    {
        private readonly ILoggingService _loggingService;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public LegacyAuthService(
            ILoggingService loggingService,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _loggingService = loggingService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task<bool> IsAuthorised(string authKey, int siteId = 0)
        {
            int authoriseAttempts = 0;
            BeginAuthorise:

            authoriseAttempts++;
            try
            {
                // Get the Auth key from the DB.
                var legacyAuthFeed = 
                        _systemSportDataUnitOfWork
                            .LegacyAuthFeedConsumers
                            .FirstOrDefault(c => c.AuthKey == authKey && c.Active);

                // Auth key doesnt exist.

                if (legacyAuthFeed == null)
                {
                    return false;
                }
                
                if (siteId != 0)
                {
                    // TODO: Temporary auth override until ZoneSite data is seeded.
                    return true;

                    var legacyZone = _systemSportDataUnitOfWork.LegacyZoneSites.Where(c => c.Id == siteId).FirstOrDefault();
                    if (legacyZone == null)
                    {
                        return false;
                    }
                    return legacyZone.Feed == legacyAuthFeed.Name.Replace("  ", string.Empty).ToLowerInvariant();
                }

                // Is authorised.
                return true;
            }
            catch (Exception)
            {
                var maxAttempts = int.Parse(ConfigurationManager.AppSettings["MaximumAuthorisationAttempts"]);
                if (authoriseAttempts > maxAttempts)
                {
                    return false;
                }

                goto BeginAuthorise;
            }
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